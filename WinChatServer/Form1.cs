using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Collections;

namespace WinChatServer
{
    public partial class Form1 : Form
    {
        public static Hashtable clientsList = new Hashtable();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
          //listBox1.DataSource = clientsList.    
        }
        public static void broadcast(string msg, string uName, bool flag)
        {
            foreach (DictionaryEntry Item in clientsList)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)Item.Value;
                NetworkStream broadcastStream = broadcastSocket.GetStream();
                Byte[] broadcastBytes = null;

                if (flag == true)
                {
                    broadcastBytes = Encoding.ASCII.GetBytes(uName + " says : " + msg);
                }
                else
                {
                    broadcastBytes = Encoding.ASCII.GetBytes(msg);
                }

                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            clsConnection.Con();  
            Thread stThread = new Thread(ServerStart);
            stThread.Start();
        }

        private void ServerStart()
        {
            TcpListener serverSocket = new TcpListener(5000);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;

            serverSocket.Start();
            Msg("Chat Server Started ....");
            counter = 0;
            while ((true))
            {
                counter += 1;
                clientSocket = serverSocket.AcceptTcpClient();


                string dataFromClient = null;

                NetworkStream networkStream = clientSocket.GetStream();
                byte[] bytesFrom = new byte[(int)clientSocket.ReceiveBufferSize];
                networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                clientsList.Add(dataFromClient, clientSocket);
                AddClient(dataFromClient);

                broadcast(dataFromClient + " Joined ", dataFromClient, false);

                //lstConversation.Items.Add();
                Msg(dataFromClient + " Joined chat room "); 
                startClient(clientSocket, dataFromClient);
                Application.DoEvents();
            }

            clientSocket.Close();
            serverSocket.Stop();
        }
        private delegate void lstConversationCallDelegate(string text);
        private void Msg(string text)
        {
            if (this.lstConversation.InvokeRequired)
            {
                lstConversationCallDelegate d = new lstConversationCallDelegate(Msg);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.lstConversation.Items.Add(text);
            }            
        }
        private delegate void lstClientsCallDelegate(string text);
        private void AddClient(string text)
        {
            if (this.lstClients.InvokeRequired)
            {
                lstClientsCallDelegate d = new lstClientsCallDelegate(AddClient);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.lstClients.Items.Add(text);
            }
        }
        private void RemoveClient(string text)
        {
            if (this.lstClients.InvokeRequired)
            {
                lstClientsCallDelegate d = new lstClientsCallDelegate(RemoveClient);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.lstClients.Items.Remove(text);
            }
        }
        //end broadcast function

        public void startClient(TcpClient inClientSocket, string clineNo)
        {
            ThreadStart starter = () => doChat(inClientSocket, clineNo);
            Thread ctThread = new Thread(starter);
            ctThread.Start();
        }

        private void doChat(TcpClient clientSocket, string clNo)
        {
            int requestCount = 0;
            byte[] bytesFrom;
            string dataFromClient = null;
            string rCount = null;
            requestCount = 0;

            while ((clientSocket.Connected))
            {
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    bytesFrom = new byte[(int)clientSocket.ReceiveBufferSize];
                    networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));                    
                    Msg("From client - " + clNo + " : " + dataFromClient);
                    ///////////////////
                    //TcpClient broadcastSocket;
                    //broadcastSocket = (TcpClient)Item.Value;
                    // NetworkStream broadcastStream = broadcastSocket.GetStream();
                    string[] abc = dataFromClient.Split(new string[] { "::"},StringSplitOptions.None );
                    if (abc[0] == "GETPROC")
                    {
                        dataFromClient = Query.getData1(abc[1]);  
                    }
                    Byte[] broadcastBytes = null;
                    broadcastBytes = Encoding.ASCII.GetBytes(dataFromClient);
                    networkStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                    //broadcastStream.Flush();
                    ////////////////////
                    rCount = Convert.ToString(requestCount);

                   // broadcast(dataFromClient, clNo, true);
                }
                catch (Exception ex)
                {
                    Msg (ex.Message .ToString());
                    break;
                }
            }
            clientsList.Remove(clNo);
            RemoveClient(clNo);
        }
    }
     
}
