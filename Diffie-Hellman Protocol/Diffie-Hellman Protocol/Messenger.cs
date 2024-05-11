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
        string chatIds;
        List<string> chatNames;
        public Messenger(int userId, NetworkStream stream)
        {
            InitializeComponent();
            this.userId = userId;
            this.stream = stream;
        }

        private void Messenger_Load(object sender, EventArgs e)
        {
            Send(stream, "GET_CHATS");
            chatIds = ReceiveString(stream);
            string text = ReceiveString(stream);
            chatNames = new List<string>(text.Split(' '));
            listView1.Items.Clear();
            
            listView1.Columns.Add("Ваши чаты", 130);
            listView1.Columns[0].TextAlign = HorizontalAlignment.Center;


            // Добавление элементов в ListView
            foreach (string chatName in chatNames)
            {
                listView1.Items.Add(chatName);
            }
            //Console.WriteLine(text);
        }
    }
}
