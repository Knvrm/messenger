using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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
        AES aes;
        public Messenger(int userId, NetworkStream stream, AES aes)
        {
            InitializeComponent();
            this.userId = userId;
            this.stream = stream;
            this.aes = aes;
        }

        private void Messenger_Load(object sender, EventArgs e)
        {
            SendEncryptedText(stream, "GET_CHATS");
            List<string> chatIds = new List<string>(ReceiveEncryptedText(stream, aes.Key).Split(' '));
            List<string> chatNames = new List<string>(ReceiveEncryptedText(stream, aes.Key).Split(' '));
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

        public void SendEncryptedText(NetworkStream stream, string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            Aes myAes = Aes.Create();
            byte[] encrypted = AES.EncryptStringToBytes_Aes(text, aes.Key, myAes.IV);

            NetworkStreamManager.SendEncryptedText(stream, encrypted, myAes.IV);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(richTextBox1.Text != "")
            {
                SendEncryptedText(stream, "SEND_MESSAGE");
                string message = $"{curChatId.ToString()} {userId.ToString()} {richTextBox1.Text}";
                SendEncryptedText(stream, message);
                if (ReceiveEncryptedText(stream, aes.Key) == "SUCCESFUL_SEND")
                {
                    Console.WriteLine("Сообщение отправлено");
                    UpdateChatMessages(curChatId);
                }
                else
                    Console.WriteLine("Сообщение не отправлено");
            }
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
                UpdateChatMessages(curChatId);
                //Console.WriteLine(curChatId);
                richTextBox1.Clear();
            }
        }

        public void UpdateChatMessages(int chatId)
        {
            SendEncryptedText(stream, "GET_CHAT_MESSAGES");
            SendEncryptedText(stream, curChatId.ToString());
            int NumberOfMessages = Int32.Parse(ReceiveEncryptedText(stream, aes.Key));
            richTextBox2.Clear();
            for (int i = 0; i < NumberOfMessages; i++)
            {
                string senderName = ReceiveEncryptedText(stream, aes.Key);
                richTextBox2.Text += senderName + ": \n   ";
                string message = ReceiveEncryptedText(stream, aes.Key);
                richTextBox2.Text += message + "\n";
            }
        }

        private void Messenger_FormClosing(object sender, FormClosingEventArgs e)
        {
            List<Form> formsToClose = new List<Form>();

            // Добавляем скрытые формы во временную коллекцию
            foreach (Form form in Application.OpenForms)
            {
                if (!form.Visible)
                {
                    formsToClose.Add(form);
                }
            }
            // Закрываем формы из временной коллекции
            foreach (Form form in formsToClose)
            {
                form.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Добавляем скрытые формы во временную коллекцию
            foreach (Form form in Application.OpenForms)
            {
                if (!form.Visible)
                {
                    form.Visible = true;
                }
            }
            this.Close();
        }
    }
}
