using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatServer
{
    public partial class Form1 : Form
    {
        ChatServer _chatServer;

        public TextBox ChatTextBox;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _chatServer = new ChatServer(this);
            stopButton.Enabled = false;
            ChatTextBox = textBox2;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            _chatServer.PORT = int.Parse(textBox1.Text);
            _chatServer.Recive();
            startButton.Enabled = false;
            stopButton.Enabled = true;
            textBox2.Text = $"{DateTime.Now.ToLocalTime()}: Server start";
        }

        private void stopButton_Click(object sender, EventArgs e)
        {

        }
    }
}
