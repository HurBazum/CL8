using CL8;
using System.Runtime.CompilerServices;


Server server = new(8888);

server.Start();

Thread t = new(async o => await server.BroadcastAsync(server._cts.Token));
t.Start();


await server.GetConnectionsAsync(server._cts.Token);
Console.ReadLine();
