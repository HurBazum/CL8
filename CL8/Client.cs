using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace CL8
{
    public class Client
    {
        private static Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private int _port = 8888;
        public IPAddress _address = IPAddress.Parse("192.168.1.2");
        private CancellationTokenSource _cts = new();
        private int _lines = 4;
        private ClientObject _userData = new();
        private bool _registered = false;
        private ReaderWriterLockSlim _lock = new();
        private readonly Encoding _encoder = Encoding.UTF8;
        public async Task ConnectToServerAsync()
        {
            try
            {
                await _socket.ConnectAsync(_address, _port);
            }
            catch(SocketException e)
            {
                Console.WriteLine(e.Message);
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


        public async Task RegisterAsync()
        {
            Console.Write("Введите свой логин: ");
            while(_registered == false)
            {
                var pos = Console.GetCursorPosition();
                string login = Console.ReadLine();
                if(string.IsNullOrEmpty(login))
                {
                    Console.SetCursorPosition(pos.Left, pos.Top);
                }
                else
                {
                    MessageObject msg = new() { Type = MessageType.Register, Content = login };

                    var dataString = JsonSerializer.Serialize(msg, typeof(MessageObject));

                    var buffer = _encoder.GetBytes(dataString);
                    await _socket.SendAsync(buffer.AsMemory(0, buffer.Length));

                    await Task.Delay(10);

                    byte[] recBytes = new byte[1024];
                    var bytes = await _socket.ReceiveAsync(recBytes);
                    string response = _encoder.GetString(recBytes, 0, bytes);

                    _userData.Login = login;
                    _userData.Id = response;
                    _registered = true;
                }
            }
        }
        
        public async Task DisconnectAsync()
        {
            MessageObject disconnectMessage = new() { Type = MessageType.Disconnect };

            var buffer = _encoder.GetBytes(JsonSerializer.Serialize(disconnectMessage));
            await _socket.SendAsync(buffer.AsMemory(0, buffer.Length));
            _cts.Cancel();
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        public async Task SendMessageAsync()
        {
            Console.Write("Введите сообщение: ");
            while(_cts.IsCancellationRequested == false)
            {
                _lock.EnterWriteLock();
                var pos = Console.GetCursorPosition();

                string msg = Console.ReadLine();
                if(string.IsNullOrEmpty(msg))
                {
                    await DisconnectAsync();
                    return;
                }
                else
                {
                    Console.SetCursorPosition(pos.Left, pos.Top);
                    Console.Write(MakeWhiteSpaces(msg));
                    Console.SetCursorPosition(pos.Left, pos.Top);
                    MessageObject mo = new() { Content = msg };
                    if(mo.Content.StartsWith('!'))
                    {
                        mo.Type = MessageType.Chat;
                    }
                    else
                    {
                        mo.Type = MessageType.Other;
                    }

                    var sendBuffer = _encoder.GetBytes(JsonSerializer.Serialize(mo, typeof(MessageObject)));
                    await _socket.SendAsync(sendBuffer.AsMemory(0, sendBuffer.Length));
                    _lock.ExitWriteLock();
                }
            }
        }

        public async Task ReceiveMessageAsync()
        {
            while(_cts.IsCancellationRequested != true)
            {
                try
                {
                    await Task.Delay(10);
                    var recBuffer = new byte[1024];
                    await _socket.ReceiveAsync(recBuffer);
                    string response = _encoder.GetString(recBuffer);

                    var pos = Console.GetCursorPosition();
                    Console.SetCursorPosition(0, _lines);
                    _lines++;
                    Console.WriteLine(response);
                    Console.SetCursorPosition(pos.Left, pos.Top);
                }
                catch(SocketException ex)
                {
                    Console.WriteLine(ex.Message);
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
    }
}