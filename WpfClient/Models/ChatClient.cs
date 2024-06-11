using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WpfClient.Models
{
    internal class ChatClient : IDisposable
    {
        private TcpClient client;
        private NetworkStream stream;

        internal bool IsInitialized { get; private set; }

        internal Action<string> OnMessageReceived;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                stream.Dispose();
                client.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal ChatClient()
        {

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

            while (true)
            {
                var buffer = new byte[1_024];
                int received = await stream.ReadAsync(buffer);
                var message = Encoding.UTF8.GetString(buffer, 0, received);
                OnMessageReceived?.Invoke(message);
            }
        }

        internal async Task Send(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(bytes);
        }
    }
}
