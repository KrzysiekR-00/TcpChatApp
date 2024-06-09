using System.IO;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

var ipEndPoint = new IPEndPoint(IPAddress.Any, 13000);
TcpListener listener = new(ipEndPoint);

List<NetworkStream> connectedClients = new List<NetworkStream>();

//try
{
    listener.Start();

    //using TcpClient handler = await listener.AcceptTcpClientAsync();

    AcceptClient();

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

Console.WriteLine("\nPress any key to quit...");
var key = Console.ReadKey();
//Console.WriteLine($"\nkey: {key}");

listener.Stop();

void AcceptClient()
{
    listener.BeginAcceptTcpClient(OnClientConnect, null);
}

void OnClientConnect(IAsyncResult asyn)
{
    TcpClient clientSocket = listener.EndAcceptTcpClient(asyn);

    HandleConnectedClient(clientSocket);

    AcceptClient();
}

async void HandleConnectedClient(TcpClient handler)
{
    

    await using NetworkStream stream = handler.GetStream();

    connectedClients.Add(stream);

    var message = $"{handler.Client.RemoteEndPoint} connected at {DateTime.Now}";
    var bytes = Encoding.UTF8.GetBytes(message);
    await stream.WriteAsync(bytes);
    Console.WriteLine($"Sent message:\r\n{message}");

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
                Console.WriteLine($"{DateTime.Now} {handler.Client.RemoteEndPoint} client disconnected");
                break;
            }
        }

        var messageReceived = Encoding.UTF8.GetString(buffer, 0, received);
        //Console.WriteLine($"{DateTime.Now} Message received:\r\n{messageReceived}");

        var messageToSend = DateTime.Now + " " + handler.Client.RemoteEndPoint + ": " + messageReceived;
        Console.WriteLine(messageToSend);
        var bytes2 = Encoding.UTF8.GetBytes(messageToSend);
        SendToAllConnectedClients(bytes2);
        //await stream.WriteAsync(bytes2);
    }
}

async void SendToAllConnectedClients(byte[] bytes)
{
    foreach (var client in connectedClients)
    {
        //await using NetworkStream stream = client.GetStream();
        await client.WriteAsync(bytes);
    }
}