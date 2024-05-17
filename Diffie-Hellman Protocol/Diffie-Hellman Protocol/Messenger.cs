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
            for (int i = 0; i < chatIds.Count; i++)
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
            if (richTextBox1.Text != "")
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

            // Создаем радиокнопки в колонке "Радио"
            AddRadioButtons();
        }

        private void AddRadioButtons()
        {
            listView1.Visible = false;
            button3.Visible =  false;
            // Создаем FlowLayoutPanel
            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel
            {
                AutoScroll = true, // Включает прокрутку при необходимости
                FlowDirection = FlowDirection.TopDown, // Радиокнопки располагаются вертикально
                WrapContents = true, // Не переносить элементы на следующую строку
                Width = listView1.Width - 5,
                Height = listView1.Height - 30, // Высота с учетом кнопки
                BorderStyle = BorderStyle.FixedSingle // Добавляем границу панели
            };
            this.Controls.Add(flowLayoutPanel);
            RadioButton selectedRadioButton = null;
            foreach (string user in users)
            {
                var radioButton = new RadioButton
                {
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleRight // Выравниваем текст по правому краю
                };

                radioButton.CheckedChanged += (sender, e) =>
                {
                    if (radioButton.Checked)
                    {
                        // Снимаем выделение с предыдущей выбранной радиокнопки
                        if (selectedRadioButton != null && selectedRadioButton != radioButton)
                            selectedRadioButton.Checked = false;

                        // Устанавливаем текущую радиокнопку как выбранную
                        selectedRadioButton = radioButton;
                    }
                };
                var label = new Label
                {
                    Text = user,
                    AutoSize = true
                };
                label.Click += (sender, e) =>
                {
                    radioButton.Checked = true; // Выбираем радиокнопку
                };

                // Создаем TableLayoutPanel для каждой радиокнопки и текста
                TableLayoutPanel tableLayoutPanel = new TableLayoutPanel
                {
                    ColumnCount = 2, // Два столбца для текста и радиокнопки
                    RowCount = 1,
                    Dock = DockStyle.Top,
                    AutoSize = true,
                    RightToLeft = RightToLeft.No, // Оставляем стандартное направление
                    Padding = new Padding(1) // Отступ для границы
                };

                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, flowLayoutPanel.Width - 35));
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 25));

                // Добавляем label и radioButton в TableLayoutPanel
                tableLayoutPanel.Controls.Add(label, 0, 0);
                tableLayoutPanel.Controls.Add(radioButton, 1, 0);

                // Добавляем TableLayoutPanel в FlowLayoutPanel
                flowLayoutPanel.Controls.Add(tableLayoutPanel);
            }

            System.Windows.Forms.Button createChatButton = new System.Windows.Forms.Button
            {
                Text = "Создать новый чат",
                Width = flowLayoutPanel.Width, // Ширина кнопки равна ширине FlowLayoutPanel
                Height = 30, // Высота кнопки
            };
            createChatButton.Show();
            createChatButton.BringToFront();
            createChatButton.Location = new Point(0, flowLayoutPanel.Height);

        }
    }
}
