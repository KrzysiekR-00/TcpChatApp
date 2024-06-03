using System.Net;
using System.Net.Sockets;
using System.Text;

var ipEndPoint = new IPEndPoint(IPAddress.Any, 13);

{
    TcpListener listener = new(ipEndPoint);

    try
    {
        listener.Start();

        using TcpClient handler = await listener.AcceptTcpClientAsync();
        await using NetworkStream stream = handler.GetStream();

        var message = "Tcp message " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff");
        var dateTimeBytes = Encoding.UTF8.GetBytes(message);
        await stream.WriteAsync(dateTimeBytes);

        Console.WriteLine("Message sent");
        //Console.WriteLine("Message sent:");
        //Console.WriteLine(message);
    }
    finally
    {
        listener.Stop();
    }
}

//

Console.WriteLine("");

//

{
    int listenPort = 11000;
    UdpClient listener = new UdpClient(listenPort);
    IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

    try
    {
        //while (true)
        {
            Console.WriteLine("Waiting for broadcast");
            byte[] bytes = listener.Receive(ref groupEP);

            Console.WriteLine(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + " - Message received:");
            Console.WriteLine($"Received broadcast from {groupEP}:");
            Console.WriteLine(Encoding.UTF8.GetString(bytes, 0, bytes.Length));
        }
    }
    catch (SocketException e)
    {
        Console.WriteLine(e);
    }
    finally
    {
        listener.Close();
    }
}

//

Console.ReadKey();