using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace schat
{
    public partial class Form1 : Form
    {
        static TcpClient client;
        bool isConnected = false;
        const int PORT = 9999;
        Thread acceptMessagesThread;
        public Form1()
        {
            InitializeComponent();
        }

        //send the message to the server
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (isConnected)
                {
                    sendMessageToServer(Utility.currentusername + " : " + textBox1.Text.Trim(), client);
                }
                else
                    MessageBox.Show("Nu esti conectat la server");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ai primit kick sau te-ai deconectat de la server");
            }
        }

        //connect to the server
        private void button2_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                try
                {
                    client = new TcpClient();
                    client.Connect("192.168.0.243", PORT);
                    sendMessageToServer(Utility.currentusername, client);//trimitem intai username-ul l server
                    acceptMessagesThread = new Thread(() => getMessagesFromServer(client));
                    acceptMessagesThread.Start();
                    label1.Text = "Status : conectat";
                    isConnected = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Nu se poate realiza conectarea la server");
                }
            }
            else
            {
                MessageBox.Show("Esti deja conectat la server");
            }  
        }

        //disconnect from the server
        private void button3_Click(object sender, EventArgs e)
        {
            if (isConnected)
            {
                try
                {
                    label1.Text = "Status : deconectat";
                    acceptMessagesThread.Suspend();
                    deconnectClient();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Esti deja deconectat");
            }
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }

        void getMessagesFromServer(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            while (true)
            {
                Byte[] data = new Byte[256];
                Int32 bytes = stream.Read(data, 0, 256);
                String responseData = Encoding.UTF8.GetString(data, 0, bytes);
                if(responseData == "server inchis" || responseData == "kick")//deconectam userul
                {
                    label1.Invoke((MethodInvoker)delegate
                    {
                       label1.Text = "Status : deconectat";
                    });
                    deconnectClient();
                    Thread.CurrentThread.Suspend();
                }
                //accesez din thread-ul principal richtextbox-ul
                //chiar daca sunt in alt thread
                richTextBox1.Invoke((MethodInvoker) delegate
                {
                  richTextBox1.AppendText(responseData + "\n");
                });
            }
        }

        void sendMessageToServer(string msg, TcpClient client)
        {
            string message = msg;
            // Translate the passed message into ASCII and store it as a Byte array.
            byte[] data = Encoding.ASCII.GetBytes(message);
            // Get a client stream for reading and writing.
            NetworkStream stream = client.GetStream();
            // Send the message to the connected TcpServer.
            stream.Write(data, 0, data.Length);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > Utility.MAXBUFFERSIZE)
            {
                button1.Enabled = false;
                MessageBox.Show("Poti trimite un mesaj de maxim : " + Utility.MAXBUFFERSIZE.ToString() + " caractere.");
            }
            else
                button1.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Bun venit " + Utility.currentusername;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (isConnected)
            {
                MessageBox.Show("Vei fi deconectat de la server");
                isConnected = false;
                deconnectClient();
            }
            for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
            {
                Application.OpenForms[i].Close();
            }
        }

        private void deconnectClient()
        {
            sendMessageToServer("/outserver", client);
            //deconectam user-ul 
            client.GetStream().Dispose();
            client.Close();
            isConnected = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }
    }
}
