using System.Net;
using System.Net.Sockets;
using System.Text;

IPEndPoint iPEndPoint = IPEndPoint.Parse("127.0.0.1");
iPEndPoint.Port = 13000;

using TcpClient client = new();
await client.ConnectAsync(iPEndPoint);
await using NetworkStream stream = client.GetStream();

var buffer = new byte[1_024];
int received = await stream.ReadAsync(buffer);

var message = Encoding.UTF8.GetString(buffer, 0, received);
Console.WriteLine($"{DateTime.Now} Message received:\r\n{message}");

while (true)
{
    Console.WriteLine("\nSend message:");
    var messageToSend = Console.ReadLine();
    if (string.IsNullOrEmpty(messageToSend)) break;
    
    var bytes = Encoding.UTF8.GetBytes(messageToSend);
    await stream.WriteAsync(bytes);
}

Console.WriteLine("\nPress any key to quit...");
var key = Console.ReadKey();