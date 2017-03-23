using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Domain;

namespace ClientUI.Views
{
    /// <summary>
    /// Interaction logic for Chat.xaml
    /// </summary>
    public partial class Chat : UserControl
    {
        ClientHandler client;
        User user;

        public Chat()
        {
            InitializeComponent();
            chatScrollBar.ScrollToBottom();
            client = new ClientHandler(this);
            user = new User("Unknown");
            chatMessage.IsEnabled = false;
        }

        private void HandleKeyPress()
        {
            Message message = new Message();
            message.User = user;

            if (!string.IsNullOrWhiteSpace(chatMessage.Text))
            {
                message.Content = chatMessage.Text;
                SendMessage(message);
                chatMessage.Text = "";
            }
        }

        private void SendMessage(Message message)
        {
            client.SendMessage(message);
        }

        public void RecieveMessage(Message message)
        {
            string formattet = string.Format("{0}: {1}", message.User.Name, message.Content);
            chatWindow.Children.Add(MessageBlock(formattet));
        }

        private TextBlock MessageBlock(string message)
        {
            TextBlock tb = new TextBlock();
            tb.Text = message;
            tb.TextWrapping = TextWrapping.Wrap;

            return tb;
        }

        private void chatMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                HandleKeyPress();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(nameBox.Text) && nameBox.Text.Length > 2)
            {
                user.Name = nameBox.Text;
                client.Connect(user);
                connectButton.IsEnabled = false;
                chatMessage.IsEnabled = true;
            }
        }
    }
}
