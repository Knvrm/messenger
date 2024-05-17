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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Diffie_Hellman_Protocol
{
    public partial class Messenger : Form
    {
        int userId;
        NetworkStream stream;
        Dictionary<string, int> chats = new Dictionary<string, int>();
        int curChatId;
        AES aes;
        private List<string> users = new List<string>();
        private List<string> selectedUsers;
        List<string> previousChatNames = new List<string>();
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
            listView1.Columns.Add("Ваши чаты:", listView1.Width - 5);
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

        private void button3_Click(object sender, EventArgs e)
        {
            
            previousChatNames.Clear();
            foreach (ListViewItem item in listView1.Items)
            {
                previousChatNames.Add(item.Text); 
            }

            listView1.Items.Clear();

            listView1.Columns.Add("Новый чат", listView1.Width - 5);

            SendEncryptedText(stream, "GET_ALL_USER_NAMES");
            string[] temp = ReceiveEncryptedText(stream, aes.Key).Split(' ');
            for (int i = 0; i < temp.Length; i++)
            {
                users.Add(temp[i]);
                var item = new ListViewItem("");
                listView1.Items.Add(item);
            }
            listView1.Enabled = false;

            // Создаем радиокнопки в колонке "Радио"
            AddRadioButtons();
        }

        private void AddRadioButtons()
        {
            int count = 0;
            foreach (ListViewItem item in listView1.Items)
            {
                var radioButton = new RadioButton();
                radioButton.Checked = false;
                radioButton.CheckedChanged += (sender, e) =>
                {
                    // Обработка события изменения состояния радиокнопки
                    // В данном случае, вы можете сохранить выбранный пользователем элемент ListView
                };

                // Добавляем радиокнопку в ListView
                listView1.Controls.Add(radioButton);
                radioButton.Parent = listView1;
                radioButton.Text = users[count];
                count++;
/*                radioButton.Dock = DockStyle.Right;
                radioButton.Width = listView1.Width - 5;*/
                radioButton.BringToFront();
                // Устанавливаем радиокнопку в нужную ячейку ListView
                radioButton.Location = listView1.GetItemRect(listView1.Items.IndexOf(item)).Location;
            }
        }

    }
}
