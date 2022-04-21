using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace ClientForTCPChat
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private string name;
        private string ip;
        private int port;
        public Form1()
        {
            InitializeComponent();
            client = new TcpClient();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ip = textBox1.Text;
                port = int.Parse(textBox2.Text);
                name = textBox3.Text;
            }
            catch
            {
                MessageBox.Show("Неправильно введены данные!");
            }
            
            try
            {
                client.Connect(ip, port);
                stream = client.GetStream();

                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start();
                textBox4.Text += "Начинайте чатиться!" + Environment.NewLine;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Что-то пошло не так!" + Environment.NewLine + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var data = Encoding.Unicode.GetBytes(name + ": " + textBox5.Text + Environment.NewLine);
            stream.Write(data, 0, data.Length);
            textBox5.Text = "";
        }

        private void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    textBox4.Text += message + Environment.NewLine;
                }
                catch
                {
                    MessageBox.Show("Соединение закрыто!");
                    Disconnect();
                    break;
                }
            }
        }

        private void Disconnect()
        {
            stream.Close();
            client.Close();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Disconnect();
        }
    }
}
