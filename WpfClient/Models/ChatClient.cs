using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WpfClient.Models
{
    internal class ChatClient : IDisposable
    {
        private TcpClient client;
        private NetworkStream stream;

        private ChatClient()
        {
            client = null!;
            stream = null!;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal static async Task<ChatClient> CreateAndInitialize(Action<string> onMessageReceived)
        {
            var client = new ChatClient();
            await client.Initialize(onMessageReceived);
            return client;
        }

        internal async Task Send(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(bytes);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                stream.Dispose();
                client.Dispose();
            }
        }

        private async Task Initialize(Action<string> onMessageReceived)
        {
            IPEndPoint iPEndPoint = IPEndPoint.Parse("127.0.0.1");
            iPEndPoint.Port = 13000;

            client = new();
            await client.ConnectAsync(iPEndPoint);

            stream = client.GetStream();

            _ = Task.Run(() => StartReceivingMessages(onMessageReceived));
        }

        private async Task StartReceivingMessages(Action<string> onMessageReceived)
        {
            bool isConnected = true;
            while (isConnected)
            {
                var buffer = new byte[1_024];
                int received = 0;

                try
                {
                    received = await stream.ReadAsync(buffer);
                }
                catch
                {
                    isConnected = false;
                    break;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, received);
                onMessageReceived?.Invoke(message);
            }
        }
    }
}
