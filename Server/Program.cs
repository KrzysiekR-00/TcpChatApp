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
    catch (SocketException ex)
    {
        Console.WriteLine($"Socket exception: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unexpected exception: {ex.Message}");
    }
    finally
    {
        listener.Close();
    }
}

//

Console.ReadKey();