using System.Net;
using System.Net.Sockets;
using System.Text;

var ipEndPoint = IPEndPoint.Parse("127.0.0.1");
ipEndPoint.Port = 13;

using TcpClient client = new();
await client.ConnectAsync(ipEndPoint);
await using NetworkStream stream = client.GetStream();

Console.WriteLine("Message:");
var message = Console.ReadLine();
if (message == null) return;

var bytes = Encoding.UTF8.GetBytes(message);
await stream.WriteAsync(bytes);

Console.WriteLine("Message sent");