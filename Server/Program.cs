using System.Net;
using System.Net.Sockets;
using System.Text;

var ipEndPoint = new IPEndPoint(IPAddress.Any, 13000);
TcpListener listener = new(ipEndPoint);

List<NetworkStream> connectedClients = [];

//try
{
    listener.Start();

    Console.WriteLine(DateTime.Now + " - server started");

    //using TcpClient handler = await listener.AcceptTcpClientAsync();

    AcceptNextClient();

    //await using NetworkStream stream = handler.GetStream();

    //var message = $"{handler.Client.RemoteEndPoint} connected at {DateTime.Now}";
    //var dateTimeBytes = Encoding.UTF8.GetBytes(message);
    //await stream.WriteAsync(dateTimeBytes);

    //Console.WriteLine($"Sent message:\r\n{message}");
}
//finally
{
    //listener.Stop();
}

ShowMenu();

//bool exit = false;

//while (!exit)
//{
//    Console.WriteLine("\r\nHelp:");
//    Console.WriteLine("Press Q to quit.");
//    Console.WriteLine("Press L to show connected clients.");

//    var key = Console.ReadKey().Key;

//    switch (key)
//    {
//        case ConsoleKey.L:
//            ShowConnectedClientsList();
//            break;
//        case ConsoleKey.Q:
//            exit = true;
//            break;
//    }
//}

listener.Stop();

return 0;

void ShowMenu()
{
    Console.WriteLine("\r\nHelp:");
    Console.WriteLine("Press Q to quit.");
    Console.WriteLine("Press L to show connected clients.");

    var key = Console.ReadKey().Key;

    switch (key)
    {
        case ConsoleKey.Q:
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

void AcceptNextClient()
{
    listener.BeginAcceptTcpClient(OnClientConnect, null);
}

void OnClientConnect(IAsyncResult asyn)
{
    TcpClient clientSocket = listener.EndAcceptTcpClient(asyn);

    HandleConnectedClient(clientSocket);

    AcceptNextClient();
}

async void HandleConnectedClient(TcpClient connectedClient)
{
    await using NetworkStream stream = connectedClient.GetStream();

    connectedClients.Add(stream);

    var connectedMessage = DateTime.Now + " - " + connectedClient.Client.RemoteEndPoint + " - connected";
    SendToAllConnectedClients(connectedMessage);

    //ReadNextMessage();

    while (true)
    {
        var buffer = new byte[1_024];
        int received = 0;

        try
        {
            received = await stream.ReadAsync(buffer);
        }
        catch
        {
            if (received > 0)
            {
                throw new NotImplementedException();
            }
            else
            {
                connectedClients.Remove(stream);

                var disconnectedMessage = DateTime.Now + " - " + connectedClient.Client.RemoteEndPoint + " - disconnected";
                SendToAllConnectedClients(disconnectedMessage);

                break;
            }
        }

        var messageReceived = Encoding.UTF8.GetString(buffer, 0, received);

        var messageToSend = DateTime.Now + " - " + connectedClient.Client.RemoteEndPoint + " - " + messageReceived;
        SendToAllConnectedClients(messageToSend);
    }
}

//void ReadNextMessage(NetworkStream stream)
//{
//    var buffer = new byte[1_024];
//    stream.BeginRead(buffer, 0, buffer.Length, HandleMessage, null);
//}

//void HandleMessage(IAsyncResult asyncResult)
//{

//}

async void SendToAllConnectedClients(string messageToSend)
{
    Console.WriteLine("\r\nSend to all connected clients:\r\n" + messageToSend);

    var bytes = Encoding.UTF8.GetBytes(messageToSend);

    foreach (var client in connectedClients)
    {
        await client.WriteAsync(bytes);
    }
}

void ShowConnectedClientsList()
{
    Console.WriteLine("\r\nConnected clients: " + connectedClients.Count);
    foreach (var client in connectedClients)
    {
        Console.WriteLine(client.Socket.RemoteEndPoint);
    }
}