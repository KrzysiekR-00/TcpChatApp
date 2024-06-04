using System.Net;
using System.Net.Sockets;
using System.Text;

var ipEndPoint = new IPEndPoint(IPAddress.Any, 13);

TcpListener listener = new(ipEndPoint);

try
{
    listener.Start();

    using TcpClient handler = await listener.AcceptTcpClientAsync();
    await using NetworkStream stream = handler.GetStream();

    var buffer = new byte[1_024];
    int received = await stream.ReadAsync(buffer);

    var message = Encoding.UTF8.GetString(buffer, 0, received);
    Console.WriteLine(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + " - Message received:");
    Console.WriteLine(message);
}
finally
{
    listener.Stop();
}

Console.ReadKey();