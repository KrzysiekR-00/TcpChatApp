using System.Net;
using System.Net.Sockets;
using System.Text;

var ipEndPoint = new IPEndPoint(IPAddress.Any, 13000);
TcpListener listener = new(ipEndPoint);

//try
{
    listener.Start();

    using TcpClient handler = await listener.AcceptTcpClientAsync();
    await using NetworkStream stream = handler.GetStream();

    var message = $"{handler.Client.RemoteEndPoint} connected at {DateTime.Now}";
    var dateTimeBytes = Encoding.UTF8.GetBytes(message);
    await stream.WriteAsync(dateTimeBytes);

    Console.WriteLine($"Sent message:\r\n{message}");
}
//finally
{
    listener.Stop();
}

Console.WriteLine("\nHit enter to continue...");
Console.Read();