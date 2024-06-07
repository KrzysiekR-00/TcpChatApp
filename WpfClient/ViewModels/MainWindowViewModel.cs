using System.Windows.Input;
using WpfClient.Models;

namespace WpfClient.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
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

        private async Task Initialize()
        {
            await _chatClient.Initialize();
            _conversation = await _chatClient.Receive();
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
