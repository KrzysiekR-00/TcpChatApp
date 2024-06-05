using System.Net;
using System.Net.Sockets;
using System.Text;

var ipEndPoint = IPEndPoint.Parse("127.0.0.1");
ipEndPoint.Port = 11_000;

using Socket client = new(
    ipEndPoint.AddressFamily,
    SocketType.Stream,
    ProtocolType.Tcp);

await client.ConnectAsync(ipEndPoint);
while (true)
{
    // Send message.
    var message = Console.ReadLine();
    if (string.IsNullOrEmpty(message)) break;

    var messageBytes = Encoding.UTF8.GetBytes(message + "<|EOM|>");
    _ = await client.SendAsync(messageBytes, SocketFlags.None);
    Console.WriteLine($"Socket client sent message: \"{message}\"");

    // Receive ack.
    var buffer = new byte[1_024];
    var received = await client.ReceiveAsync(buffer, SocketFlags.None);
    var response = Encoding.UTF8.GetString(buffer, 0, received);
    if (response == "<|ACK|>")
    {
        Console.WriteLine(
            $"Socket client received acknowledgment: \"{response}\"");
        //break;
    }
}

client.Shutdown(SocketShutdown.Both);