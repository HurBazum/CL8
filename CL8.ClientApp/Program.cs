using CL8;

Client client = new();

CancellationTokenSource cts = new();

await client.ConnectToServerAsync();

await client.RegisterAsync(CancellationToken.None);

Thread sender = new(async o => await client.SendMessageAsync(cts.Token));

sender.Start();

await client.ReceiveMessagesAsync(cts.Token);

Console.ReadLine();