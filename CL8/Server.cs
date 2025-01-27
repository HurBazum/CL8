using System.Net.Sockets;
using System.Net;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace CL8
{
    public class Server(int port)
    {
        private int _port = port;
        private Socket _listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private IPEndPoint _endPoint = new(IPAddress.Any, port);
        public CancellationTokenSource _cts = new();
        private ReaderWriterLockSlim _lock = new();
        private ConcurrentDictionary<EndPoint, ConnectedClient> _sockets = [];
        private Stack<MessageObject> _messages = [];
        private Encoding _encoder = Encoding.UTF8;

        public void Start()
        {
            _listener.Bind(_endPoint);
            _listener.Listen(10);
            Console.WriteLine("Сервер запущен. Ожидание подключений. . .");
        }

        public async Task GetConnectionsAsync()
        {
            while(_cts.IsCancellationRequested == false)
            {
                var client = await _listener.AcceptAsync();
                
                var cc = new ConnectedClient(client);

                _sockets.TryAdd(client.RemoteEndPoint, cc);
                Console.WriteLine($"{client.RemoteEndPoint}");
                ThreadPool.QueueUserWorkItem(async o => await ProccessClientAsync(cc));
            }
        }

        private async Task ProccessClientAsync(ConnectedClient cc)
        {
            CancellationTokenSource streamCts = new();
            while(streamCts.IsCancellationRequested == false && cc.Socket.Connected)
            {
                try
                {
                    var buffer = new byte[1024];
                    int recBytes = await cc.Socket.ReceiveAsync(buffer);                    
                    var request = _encoder.GetString(buffer, 0, recBytes);
                    var received = JsonSerializer.Deserialize<MessageObject>(request);
                    byte[] responseBuffer;
                    switch(received.Type)
                    {
                        case MessageType.Disconnect:
                            string n = cc.Socket.RemoteEndPoint.ToString();
                            _sockets.Remove(cc.Socket.RemoteEndPoint, out cc);
                            await SayDisconnect(cc);
                            if(_sockets.Count == 0)
                            {
                                _cts.Cancel();
                                Console.WriteLine($"{n} отключился");
                                Console.WriteLine("Сервер остановлен");
                            }
                            else
                            {
                                streamCts.Cancel();
                                Console.WriteLine($"{n} отключился");
                            }
                            break;
                        case MessageType.Other:
                            await SendResponseToCLientAsync("Запрос получен", cc);
                            break;
                        case MessageType.Register:
                            cc.SetLogin(received.Content);
                            await SendResponseToCLientAsync(Guid.NewGuid().ToString(), cc);
                            break;
                        case MessageType.Chat:
                            received.EndPoint = cc.Socket.RemoteEndPoint;
                            received.Content = string.Concat(cc.Login, ": ", received.Content);
                            _lock.EnterWriteLock();
                            _messages.Push(received);
                            _lock.ExitWriteLock();
                            break;
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private async Task SendResponseToCLientAsync(string response, ConnectedClient client)
        {
            var responseBuffer = _encoder.GetBytes(response);
            await client.Stream.WriteAsync(responseBuffer.AsMemory(0, responseBuffer.Length));
            await client.Stream.FlushAsync();
        }

        public async Task BroadcastAsync()
        {
            while(_listener.IsBound)
            {
                if(_messages.Count > 0)
                {
                    var msg = _messages.Peek();
                    byte[] buffer = _encoder.GetBytes(msg.Content);
                    foreach(var client in _sockets.Values)
                    {
                        if(client.Socket.RemoteEndPoint != msg.EndPoint)
                        {
                            await client.Stream.WriteAsync(buffer.AsMemory(0, buffer.Length));
                            await client.Stream.FlushAsync();
                        }
                    }
                    _messages.Pop();
                }
                else
                {
                    continue;
                }
            }
        }

        private async Task SayDisconnect(ConnectedClient cc)
        {
            var responseBuffer = Encoding.UTF8.GetBytes("Запрос на отключение получен");
            await cc.Stream.WriteAsync(responseBuffer.AsMemory(0, responseBuffer.Length));
            await cc.Stream.FlushAsync();

            await Task.Delay(1000);
            cc.Socket.Shutdown(SocketShutdown.Both);

            cc.Stream.Close();

            cc.Stream.Dispose();
            cc.Socket.Close();
        }
    }
}