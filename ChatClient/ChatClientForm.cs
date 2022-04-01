using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class ChatClientForm : Form
    {
        ChatClient _chatClient;
        public ChatClientForm()
        {
            InitializeComponent();
        }

        private void ChatClientForm_Load(object sender, EventArgs e)
        {
            disconnectButton.Enabled = false;
            sendButton.Enabled = false;


        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (_chatClient == null)
            {
                if (string.IsNullOrEmpty(clientTextBox.Text) && string.IsNullOrEmpty(serverTextBox.Text))
                    return;
                // vars ...
                _chatClient = new ChatClient(int.Parse(serverTextBox.Text), int.Parse(clientTextBox.Text), listBox1 , this);
                nameTextBox.Enabled = false;
                clientTextBox.Enabled = false;
                serverTextBox.Enabled = false;
            }

            disconnectButton.Enabled = true;
            sendButton.Enabled = true;
            connectButton.Enabled = false;
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            _chatClient.Send($"{nameTextBox.Text}: {messageTextBox.Text}");
            messageTextBox.Clear();
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            nameTextBox.Clear();
            nameTextBox.Enabled = true;
            clientTextBox.Clear();
            clientTextBox.Enabled = true;
            serverTextBox.Clear();
            serverTextBox.Enabled = true;
        }
    }
}
