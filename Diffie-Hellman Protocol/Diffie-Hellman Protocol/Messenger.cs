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
        Dictionary<int, string> chats = new Dictionary<int, string>();
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
                chats.Add(Convert.ToInt32(chatIds[i]), chatNames[i]);
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

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            //curChatId = Item
        }
    }
}
