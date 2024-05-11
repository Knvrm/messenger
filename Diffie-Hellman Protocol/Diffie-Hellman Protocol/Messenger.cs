using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Diffie_Hellman_Protocol.NetworkStreamManager;

namespace Diffie_Hellman_Protocol
{
    public partial class Messenger : Form
    {
        int userId;
        NetworkStream stream;
        Dictionary<string, int> chats = new Dictionary<string, int>();
        int curChatId;
        public Messenger(int userId, NetworkStream stream)
        {
            InitializeComponent();
            this.userId = userId;
            this.stream = stream;
        }

        private void Messenger_Load(object sender, EventArgs e)
        {
            Send(stream, "GET_CHATS");
            List<string> chatIds = new List<string>(ReceiveString(stream).Split(' '));
            List<string> chatNames = new List<string>(ReceiveString(stream).Split(' '));
            listView1.Items.Clear();
            for(int i = 0; i < chatIds.Count; i++)
                chats.Add(chatNames[i], Convert.ToInt32(chatIds[i]));
            listView1.Columns.Add("Ваши чаты", listView1.Width - 5);
            listView1.Columns[0].TextAlign = HorizontalAlignment.Center;


            // Добавление элементов в ListView
            foreach (string chatName in chatNames)
            {
                listView1.Items.Add(chatName);
                
            }

            //Console.WriteLine(chats);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                // Получение выбранного элемента
                ListViewItem selectedItem = listView1.SelectedItems[0];

                // Получение данных из выбранного элемента
                string itemText = selectedItem.Text;

                curChatId = chats[itemText];
                
                Send(stream, "GET_CHAT_MESSAGES");
                Send(stream, curChatId);
                byte[] data = Receive(stream);
                int NumberOfMessages = BitConverter.ToInt32(data, 0);
                richTextBox2.Clear();
                for(int i = 0; i < NumberOfMessages; i++)
                {
                    string senderName = ReceiveString(stream);
                    richTextBox2.Text += senderName + '\n';
                    string message = ReceiveString(stream);
                    richTextBox2.Text += message + '\n';
                }
                //Console.WriteLine(curChatId);
            }
        }
    }
}
