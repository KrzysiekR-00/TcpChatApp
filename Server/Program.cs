using System.Net;
using System.Net.Sockets;
using System.Text;

var ipEndPoint = new IPEndPoint(IPAddress.Any, 13000);
TcpListener listener = new(ipEndPoint);

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

    var message = $"{handler.Client.RemoteEndPoint} connected at {DateTime.Now}";
    var dateTimeBytes = Encoding.UTF8.GetBytes(message);
    await stream.WriteAsync(dateTimeBytes);
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
                Console.WriteLine($"{DateTime.Now} Client disconnected");
                break;
            }
        }

        var messageReceived = Encoding.UTF8.GetString(buffer, 0, received);
        Console.WriteLine($"{DateTime.Now} Message received:\r\n{messageReceived}");
    }
}