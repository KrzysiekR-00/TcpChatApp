using System.Net.Sockets;
using System.Net;
using System.Text;

var ipEndPoint = new IPEndPoint(IPAddress.Any, 13);
TcpListener listener = new(ipEndPoint);

try
{
    listener.Start();

    using TcpClient handler = await listener.AcceptTcpClientAsync();
    await using NetworkStream stream = handler.GetStream();

    var message = "Message " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff");
    var dateTimeBytes = Encoding.UTF8.GetBytes(message);
    await stream.WriteAsync(dateTimeBytes);

    Console.WriteLine("Message sent:");
    Console.WriteLine(message);
}
finally
{
    listener.Stop();
}

Console.ReadKey();