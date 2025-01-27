using CL8;

Client client = new();

await client.ConnectToServerAsync();

await client.RegisterAsync();

Thread sender = new(async o => await client.SendMessageAsync());

sender.Start();

await client.ReceiveMessageAsync();

Console.ReadLine();