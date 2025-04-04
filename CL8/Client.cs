using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace CL8
{
    public class Client
    {
        private readonly Socket _socket;
        private readonly int _port;
        private readonly IPAddress _address;
        private readonly Encoding _encoder = Encoding.UTF8;

        private CancellationTokenSource _cts = new();
        private int _lines = 5;
        private ClientObject _userData = new();
        private bool _registered = false;
        private ReaderWriterLockSlim _lock = new();

        public Client(int port = 8888, string address = "127.0.0.1")
        {
            _port = port;
            _address = IPAddress.Parse(address);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }


        public async Task<bool> ConnectToServerAsync()
        {
            try
            {
                await _socket.ConnectAsync(_address, _port);
                Console.WriteLine("Подключение к серверу установлено");
                return true;
            }
            catch(SocketException e)
            {
                Console.WriteLine($"Ошибка подключения: {e.Message}");
                return false;
            }
        }

        public bool RegisterStatus()
        {
            if(!string.IsNullOrEmpty(_userData.Id) && !string.IsNullOrEmpty(_userData.Login))
            {
                return true;
            }

            return false;
        }


        public async Task RegisterAsync(CancellationToken ct)
        {
            Console.Write("Введите свой логин: ");
            while(!_registered && !ct.IsCancellationRequested)
            {
                var pos = Console.GetCursorPosition();
                string login = Console.ReadLine();
                if(string.IsNullOrEmpty(login))
                {
                    Console.SetCursorPosition(pos.Left, pos.Top);
                    continue;
                }
                
                MessageObject msg = new() { Type = MessageType.Register, Content = login };
                await SendMessageAsync(msg, ct);
                
                string response = await ReceiveMessageAsync(ct);

                if(!string.IsNullOrEmpty(response))
                {
                    _userData.Login = login;
                    _userData.Id = response;
                    _registered = true;
                    PrintMessage($"Регистрация прошла успешно. ID: {_userData.Id}\nДля выхода из чата отправьте пустое сообщение");
                }
            }
        }
        
        public async Task DisconnectAsync(CancellationToken ct)
        {
            try
            {
                MessageObject disconnectMessage = new() { Type = MessageType.Disconnect, Content = "stop" };

                await SendMessageAsync(disconnectMessage, ct);

                await Task.Delay(1000, ct);

                var response = await ReceiveMessageAsync(ct);

                PrintMessage(response);
                
                _cts.Cancel();
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Ошибка отключения: {ex.Message}");
            }
            finally
            {
                _socket.Close();
                _socket.Dispose();
            }
        }

        public async Task SendMessageAsync(CancellationToken ct)
        {
            Console.Write("Введите сообщение: ");
            while(!ct.IsCancellationRequested)
            {
                var pos = Console.GetCursorPosition();

                string msg = Console.ReadLine();
                if(string.IsNullOrEmpty(msg))
                {
                    await DisconnectAsync(ct);
                    return;
                }
                
                Console.SetCursorPosition(pos.Left, pos.Top);
                Console.Write(MakeWhiteSpaces(msg));
                Console.SetCursorPosition(pos.Left, pos.Top);

                MessageObject mo = new()
                {
                    Content = msg,
                    Type = msg.StartsWith("!") ? MessageType.Chat : MessageType.Other
                };

                await SendMessageAsync(mo, ct);
            }
        }

        public async Task ReceiveMessagesAsync(CancellationToken ct)
        {
            while(!ct.IsCancellationRequested)
            {
                try
                {
                    string response = await ReceiveMessageAsync(ct);
                    
                    // сервер отключился
                    if(string.IsNullOrEmpty(response))
                    {
                        break;
                    }

                    PrintMessage(response);
                }
                catch(SocketException ex)
                {
                    PrintMessage($"Ошибка приёма: {ex.Message}");
                    break;
                }
                catch(Exception ex)
                {
                    PrintMessage($"Ошибка: {ex.Message}");
                    break;
                }
            }
        }

        private string MakeWhiteSpaces(string s)
        {
            string result = string.Empty;

            for(int i = 0; i < s.Length; i++)
            {
                result += " ";
            }

            return result;
        }

        private void PrintMessage(string message)
        {
            var pos = Console.GetCursorPosition();
            Console.SetCursorPosition(0, _lines);
            _lines++;
            Console.WriteLine(message);
            Console.SetCursorPosition(pos.Left, pos.Top);
        }

        private async Task SendMessageAsync(MessageObject msg, CancellationToken ct)
        {
            var json = JsonSerializer.Serialize(msg);
            byte[] msgBytes = _encoder.GetBytes(json);
            byte[] lengthBytes = BitConverter.GetBytes(msgBytes.Length);

            await _socket.SendAsync(lengthBytes, ct);
            await _socket.SendAsync(msgBytes, ct);
        }

        private async Task<string> ReceiveMessageAsync(CancellationToken ct)
        {
            byte[] lengthBuffer = new byte[4];
            int bytesRead = await ReceiveExactAsync(lengthBuffer, 4, ct);
            if(bytesRead == 0)
            {
                return string.Empty;
            }

            int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
            byte[] messageBuffer = new byte[messageLength];
            bytesRead = await ReceiveExactAsync(messageBuffer, messageLength, ct);

            return bytesRead > 0 ? _encoder.GetString(messageBuffer, 0, bytesRead) : string.Empty;
        }

        private async Task<int> ReceiveExactAsync(byte[] buffer, int length, CancellationToken ct)
        {
            int totalRead = 0;
            if(totalRead < length)
            {
                int read = await _socket.ReceiveAsync(buffer.AsMemory(totalRead, length - totalRead), ct);
                if(read == 0)
                {
                    return 0;
                }
                totalRead += read;
            }
            return totalRead;
        }
    }

    /*
     byte[] response = new byte[256];
            var recLen = _socket.ReceiveAsync(response.AsMemory(0, response.Length));

            PrintMessage(_encoder.GetString(response));
    */
}