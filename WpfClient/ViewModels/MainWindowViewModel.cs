using System.Windows.Input;
using WpfClient.Models;

namespace WpfClient.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private ChatClient _chatClient;

        private string _conversation;
        private string _messageToSend;

        public string Conversation
        {
            get => _conversation;
            private set
            {
                _conversation = value;
                OnPropertyChanged(nameof(Conversation));
            }
        }

        public string MessageToSend
        {
            get => _messageToSend;
            set
            {
                _messageToSend = value;
                OnPropertyChanged(nameof(MessageToSend));
            }
        }

        public ICommand SendMessageCommand { get; private set; }

        internal MainWindowViewModel()
        {
            _chatClient = null!;
            _conversation = string.Empty;
            _messageToSend = string.Empty;
            SendMessageCommand = new RelayCommand(_ => SendMessage());

            Task.Run(() => Initialize());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _chatClient.Dispose();
            }
        }

        private async Task Initialize()
        {
            _chatClient = await ChatClient.CreateAndInitialize(MessageReceived);
        }

        private void MessageReceived(string message)
        {
            Conversation += "\r\n" + message;
        }

        private void SendMessage()
        {
            Task.Run(async () =>
            {
                var messageToSend = MessageToSend;
                MessageToSend = string.Empty;
                await _chatClient.Send(messageToSend);
            });
        }
    }
}
