using System.Net;
using System.Net.Sockets;
using System.Text;

{
    var ipEndPoint = IPEndPoint.Parse("127.0.0.1");
    ipEndPoint.Port = 13;

    using TcpClient client = new();
    await client.ConnectAsync(ipEndPoint);
    await using NetworkStream stream = client.GetStream();

    var buffer = new byte[1_024];
    int received = await stream.ReadAsync(buffer);

    var message = Encoding.UTF8.GetString(buffer, 0, received);
    Console.WriteLine(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + " - Message received:");
    Console.WriteLine(message);
}

//

await Task.Delay(500);
Console.WriteLine("");

//

{
    Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

    IPAddress broadcast = IPAddress.Parse("127.0.0.1");

    string message = "Udp messsage " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff");
    byte[] sendbuf = Encoding.UTF8.GetBytes(message);
    IPEndPoint ep = new IPEndPoint(broadcast, 11000);

    s.SendTo(sendbuf, ep);

    Console.WriteLine("Message sent");
}

//

Console.ReadKey();