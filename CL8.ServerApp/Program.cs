using CL8;

Server server = new(8888);

server.Start();

Thread t = new(async o => await server.BroadcastAsync());
t.Start();

await server.GetConnectionsAsync();
Console.ReadLine();