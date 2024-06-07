using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WpfClient.Models
{
    internal class ChatClient
    {
        private TcpClient client;
        private NetworkStream stream;

        internal bool IsInitialized { get; private set; }

        internal ChatClient() 
        {
        
        }

        ~ChatClient()
        {
            stream.Dispose();
            client.Dispose();
        }

        internal async Task Initialize()
        {
            IPEndPoint iPEndPoint = IPEndPoint.Parse("127.0.0.1");
            iPEndPoint.Port = 13000;

            //using TcpClient client = new();
            client = new();
            await client.ConnectAsync(iPEndPoint);
            //await using NetworkStream stream = client.GetStream();
            
            stream = client.GetStream();

            IsInitialized = true;
        }

        internal async Task<string> Receive()
        {
            var buffer = new byte[1_024];
            int received = await stream.ReadAsync(buffer);

            //var message = Encoding.UTF8.GetString(buffer, 0, received);
            //Console.WriteLine($"{DateTime.Now} Message received:\r\n{message}");

            return Encoding.UTF8.GetString(buffer, 0, received);
        }

        internal async Task Send(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(bytes);
        }
    }
}
