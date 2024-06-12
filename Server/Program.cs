using System.Net;
using System.Net.Sockets;
using System.Text;

var ipEndPoint = new IPEndPoint(IPAddress.Any, 13000);
TcpListener listener = new(ipEndPoint);

List<NetworkStream> connectedClients = [];

listener.Start();

Console.WriteLine(DateTime.Now + " - server started");

_ = Task.Run(() => AcceptClients());

ShowMenu();

return 0;

async Task AcceptClients()
{
    while (listener.Server.IsBound)
    {
        var client = await listener.AcceptTcpClientAsync();

        _ = Task.Run(() => HandleConnectedClient(client));
    }
}

async Task HandleConnectedClient(TcpClient connectedClient)
{
    await using NetworkStream stream = connectedClient.GetStream();

    connectedClients.Add(stream);

    var connectedMessage = DateTime.Now + " - " + connectedClient.Client.RemoteEndPoint + " - connected";
    await SendToAllConnectedClients(connectedMessage);

    await StartReceiingMessages(stream, connectedClient);
}

async Task StartReceiingMessages(NetworkStream stream, TcpClient connectedClient)
{
    while (listener.Server.IsBound)
    {
        var buffer = new byte[1_024];
        int received = 0;

        try
        {
            received = await stream.ReadAsync(buffer);
        }
        catch
        {
            connectedClients.Remove(stream);

            var disconnectedMessage = DateTime.Now + " - " + connectedClient.Client.RemoteEndPoint + " - disconnected";
            await SendToAllConnectedClients(disconnectedMessage);

            break;
        }

        var messageReceived = Encoding.UTF8.GetString(buffer, 0, received);

        var messageToSend = DateTime.Now + " - " + connectedClient.Client.RemoteEndPoint + " - " + messageReceived;
        await SendToAllConnectedClients(messageToSend);
    }
}

async Task SendToAllConnectedClients(string messageToSend)
{
    Console.WriteLine("\r\nSend to all connected clients:\r\n" + messageToSend);

    var bytes = Encoding.UTF8.GetBytes(messageToSend);

    foreach (var client in connectedClients)
    {
        await client.WriteAsync(bytes);
    }
}

void ShowMenu()
{
    Console.WriteLine("\r\nHelp:");
    Console.WriteLine("Press Q to quit.");
    Console.WriteLine("Press L to show connected clients.");

    var key = Console.ReadKey().Key;

    switch (key)
    {
        case ConsoleKey.Q:
            Close();
            break;
        case ConsoleKey.L:
            ShowConnectedClientsList();
            ShowMenu();
            break;
        default:
            ShowMenu();
            break;
    }
}

void Close()
{
    listener.Stop();
}

void ShowConnectedClientsList()
{
    Console.WriteLine("\r\nConnected clients: " + connectedClients.Count);
    foreach (var client in connectedClients)
    {
        Console.WriteLine(client.Socket.RemoteEndPoint);
    }
}