using System.Net.Sockets;
using System.Net;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace CL8
{
    public class Server
    {
        private int _port;
        private Socket _listener;
        private IPEndPoint _endPoint;
        public CancellationTokenSource _cts = new();
        private ReaderWriterLockSlim _lock = new();
        private ConcurrentDictionary<EndPoint, ConnectedClient> _sockets = [];
        private ConcurrentStack<MessageObject> _messages = [];
        private Encoding _encoder = Encoding.UTF8;


        // 
        public Server(int port)
        {
            if(port < 1 || port > 65535)
            {
                throw new ArgumentException("Порт должен быть в диапазоне от 1 до 65535");
            }
            _port = port;
            _listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _endPoint = new(IPAddress.Any, port);
        }

        public void Start()
        {
            _listener.Bind(_endPoint);
            _listener.Listen(10);
            Console.WriteLine("Сервер запущен. Ожидание подключений. . .");
        }

        public async Task GetConnectionsAsync(CancellationToken cancellationToken)
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var client = await _listener.AcceptAsync(cancellationToken);

                    var cc = new ConnectedClient(client);

                    _sockets.TryAdd(client.RemoteEndPoint, cc);
                    Console.WriteLine($"{client.RemoteEndPoint} подключился");
                    _ = Task.Run(() => ProccessClientAsync(cc, cancellationToken), cancellationToken);
                }
                catch(OperationCanceledException)
                {
                    Console.WriteLine("Приём подключений остановлен");
                    break;
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Ошибка подключения: {ex.Message}");
                }
            }
        }

        private async Task ProccessClientAsync(ConnectedClient cc, CancellationToken ct)
        {
            CancellationTokenSource streamCts = new();
            while(streamCts.IsCancellationRequested == false && cc.Socket.Connected)
            {
                try
                {
                    var lengthBuffer = new byte[4];

                    int bytesRead = await ReadExactAsync(cc.Socket, lengthBuffer, 4, ct);                    
                    
                    // клиент оключился
                    if(bytesRead == 0)
                    {
                        await HandleDisconnectAsync(cc, ct);
                        break;
                    }
                    
                    // проверка длины сообщения
                    int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
                    if(messageLength <= 0 || messageLength > 1024 *1024)
                    {
                        Console.WriteLine("Некорректная длина сообщения");
                        continue;
                    }

                    byte[] messageBuffer = new byte[messageLength];
                    bytesRead = await ReadExactAsync(cc.Socket, messageBuffer, messageLength, ct);

                    if(bytesRead == 0)
                    {
                        await HandleDisconnectAsync(cc, ct);
                        break;
                    }

                    var request = _encoder.GetString(messageBuffer, 0, bytesRead);
                    var received = JsonSerializer.Deserialize<MessageObject>(request);

                    await ProccessMessageAsync(cc, received, ct);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Ошибка обработки клиента {cc.Socket.RemoteEndPoint}: {ex.Message}");
                }
            }
        }

        private async Task ProccessMessageAsync(ConnectedClient cc, MessageObject mo, CancellationToken ct)
        {
            switch(mo.Type)
            {
                case MessageType.Disconnect:
                    await HandleDisconnectAsync(cc, ct);
                    break;
                case MessageType.Other:
                    await SendResponseToCLientAsync("request was received. This is server's response.", cc, ct);
                    break;
                case MessageType.Register:
                    await HandleRegisterCLientAsync(cc, mo, ct);
                    break;
                case MessageType.Chat:
                    await HandleChatAsync(cc, mo);
                    break;
            }
        }

        private async Task SendResponseToCLientAsync(string response, ConnectedClient client, CancellationToken ct)
        {
            var responseLength = BitConverter.GetBytes(response.Length);
            var responseBuffer = _encoder.GetBytes(response);
            await client.Stream.WriteAsync(responseLength, ct);
            await client.Stream.WriteAsync(responseBuffer, ct);
            await client.Stream.FlushAsync(ct);
        }

        public async Task BroadcastAsync(CancellationToken ct)
        {
            while(_listener.IsBound && !ct.IsCancellationRequested)
            {
                try
                {
                    if(_messages.TryPop(out var msg))
                    {
                        //byte[] buffer = _encoder.GetBytes(msg.Content);
                        foreach(var client in _sockets.Values)
                        {
                            if(client.Socket.RemoteEndPoint != msg.EndPoint)
                            {
                                try
                                {
                                    /*await client.Stream.WriteAsync(buffer.AsMemory(0, buffer.Length));
                                    await client.Stream.FlushAsync();*/

                                    await SendResponseToCLientAsync(msg.Content, client, ct);
                                }
                                catch(Exception ex)
                                {
                                    Console.WriteLine($"Ошибка отправки {client.Socket.RemoteEndPoint}: {ex.Message}");
                                }
                            }
                        }
                    }
                    else
                    {
                        // чтобы не гонять CPU
                        await Task.Delay(100, ct);
                    }
                }
                catch(OperationCanceledException)
                {
                    Console.WriteLine("Рассылка сообщений остановлена.");
                    break;
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Ошибка рассылки: {ex.Message}");
                }
            }
        }

        private async Task SayDisconnect(ConnectedClient cc, CancellationToken ct)
        {
            var disconnectResponse = "shutdown request was accepted";

            await SendResponseToCLientAsync(disconnectResponse, cc, ct);

            await Task.Delay(1000, ct);

            cc.Socket.Shutdown(SocketShutdown.Both);

            cc.Stream.Close();

            cc.Stream.Dispose();
            cc.Socket.Close();
        }

        private async Task HandleDisconnectAsync(ConnectedClient cc, CancellationToken ct)
        {
            string endpoint = cc.Socket.RemoteEndPoint.ToString();
            _sockets.TryRemove(cc.Socket.RemoteEndPoint, out _);
            await SayDisconnect(cc, ct);
            Console.WriteLine($"{endpoint} disconnected");
            if(_sockets.IsEmpty)
            {
                _cts.Cancel();
            }
        }

        private async Task HandleRegisterCLientAsync(ConnectedClient cc, MessageObject mo, CancellationToken ct)
        {
            cc.SetLogin(mo.Content);
            await SendResponseToCLientAsync(Guid.NewGuid().ToString(), cc, ct);
        }

        private async Task HandleChatAsync(ConnectedClient cc, MessageObject mo)
        {
            mo.EndPoint = cc.Socket.RemoteEndPoint;
            mo.Content = $"{cc.Login}: {mo.Content}";
            _lock.EnterWriteLock();
            try
            {
                _messages.Push(mo);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        private async Task<int> ReadExactAsync(Socket socket, byte[] buffer, int length, CancellationToken ct)
        {
            int totalRead = 0;
            while(totalRead < length)
            {
                int read = await socket.ReceiveAsync(buffer.AsMemory(totalRead, length - totalRead), ct);
                if(read == 0)
                {
                    return 0;
                }
                totalRead += read;
            }
            return totalRead;
        }
    }
}