using System.Windows.Input;
using WpfClient.Models;

namespace WpfClient.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private ChatClient _chatClient = new();

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
            _conversation = string.Empty;
            _messageToSend = string.Empty;
            SendMessageCommand = new RelayCommand(_ => SendMessage());

            Initialize();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _chatClient.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private async Task Initialize()
        {
            _chatClient.OnMessageReceived += MessageReceived;
            await _chatClient.Initialize();
            //Conversation = await _chatClient.Receive();
        }

        private void MessageReceived(string message)
        {
            Conversation += "\r\n" + message;
        }

        private void SendMessage()
        {
            //Conversation += MessageToSend + "\r\n";
            //MessageToSend = string.Empty;

            _chatClient.Send(MessageToSend);
            MessageToSend = string.Empty;
        }
    }
}
