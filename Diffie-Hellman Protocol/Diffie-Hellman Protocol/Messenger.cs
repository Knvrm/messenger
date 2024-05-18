using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using static Diffie_Hellman_Protocol.NetworkStreamManager;

namespace Diffie_Hellman_Protocol
{
    public partial class Messenger : Form
    {
        int userId;
        string nickname;
        NetworkStream stream;
        Dictionary<string, int> chats = new Dictionary<string, int>();
        int curChatId;
        AESAdapter aes;
        private List<string> users = new List<string>();
        public Messenger(int userId, string nickname, NetworkStream stream, AESAdapter aes)
        {
            InitializeComponent();
            this.userId = userId;
            this.stream = stream;
            this.aes = aes;
            this.nickname = nickname;
        }

        private void Messenger_Load(object sender, EventArgs e)
        {
            UpdateChats();
            //Console.WriteLine(chats);
        }
        public void UpdateChats()
        {
            listView1.Clear();
            SendEncryptedText(stream, "GET_CHATS");
            string text = ReceiveEncryptedText(stream, aes.Key);
            if (text != "CHATS_NOT_FOUND")
            {
                List<string> chatIds = new List<string>(text.Split(' '));
                List<string> chatNames = new List<string>(ReceiveEncryptedText(stream, aes.Key).Split(' '));
                listView1.Items.Clear();
                chats.Clear();
                for (int i = 0; i < chatIds.Count; i++)
                {
                    chatNames[i] = chatNames[i].Replace($"{nickname}_", "");
                    chatNames[i] = chatNames[i].Replace($"_{nickname}", "");
                    chats.Add(chatNames[i], Convert.ToInt32(chatIds[i]));
                }
                    
                listView1.Columns.Add("Ваши чаты:", listView1.Width - 5);
                listView1.Columns[0].TextAlign = HorizontalAlignment.Center;

                // Добавление элементов в ListView
                foreach (string chatName in chatNames)
                {
                    listView1.Items.Add(chatName);
                }
            }
        }

        public void SendEncryptedText(NetworkStream stream, string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            Aes myAes = Aes.Create();
            byte[] encrypted = AESAdapter.EncryptStringToBytes_Aes(text, aes.Key, myAes.IV);

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
            SendEncryptedText(stream, chatId.ToString());
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
            listView1.Items.Clear();
            users.Clear();
            SendEncryptedText(stream, "GET_ALL_USER_NAMES");
            string[] temp = ReceiveEncryptedText(stream, aes.Key).Split(' ');
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] != nickname)
                {
                    users.Add(temp[i]);
                    var item = new ListViewItem("");
                    listView1.Items.Add(item);
                }
            }

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
                Width = listView1.Width - 7,
                Height = listView1.Height, // Высота с учетом кнопки
                BorderStyle = BorderStyle.FixedSingle // Добавляем границу панели
            };
            this.Controls.Add(flowLayoutPanel);
            RadioButton selectedRadioButton = null;
            string selectedChatName = String.Empty;
            foreach (string user in users)
            {
                var radioButton = new RadioButton
                {
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleRight // Выравниваем текст по правому краю
                };
                var label = new Label
                {
                    Text = user,
                    AutoSize = true
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
                        selectedChatName = label.Text;
                    }
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
                    Padding = new Padding(1), // Отступ для границы
                    Width = listView1.Width - 10,
                    Height = listView1.Height - 30
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
                Width = flowLayoutPanel.Width - 10, // Ширина кнопки равна ширине FlowLayoutPanel
                Height = 30, // Высота кнопки
                Location = new Point(0, flowLayoutPanel.Height - Height)
            };
            createChatButton.Click += (sender, e) =>
            {
                if(selectedChatName != String.Empty)
                {
                    /*Console.WriteLine(selectedChatName);
                    foreach(var item in chats)
                    {
                        Console.WriteLine(item.Key + " " + item.Value);
                    }*/
                    if (!chats.Keys.Contains<string>(selectedChatName))
                    {
                        SendEncryptedText(stream, "ADD_CHAT");
                        SendEncryptedText(stream, userId.ToString());
                        SendEncryptedText(stream, selectedChatName);
                        string text = ReceiveEncryptedText(stream, aes.Key);
                        if(text == "SUCCESFUL_ADD_CHAT")
                            Console.WriteLine("Чат успешно создан");
                        else
                            MessageBox.Show("Произошла ошибка при создании чата.", "Ошибка создания чата", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    foreach (Control control in flowLayoutPanel.Controls)
                    {
                        // Если элемент является TableLayoutPanel, удаляем его и все его дочерние элементы
                        if (control is TableLayoutPanel tableLayoutPanel)
                        {
                            tableLayoutPanel.Controls.Clear();
                            tableLayoutPanel.Dispose(); // Освобождаем ресурсы
                        }
                    }

                    flowLayoutPanel.Controls.Clear(); // Удаляем все дочерние элементы FlowLayoutPanel
                    flowLayoutPanel.Dispose(); // Освобождаем ресурсы

                    // Удаляем кнопку создания чата
                    createChatButton.Dispose();
                    listView1.Visible = true;
                    button3.Visible = true;
                    UpdateChats();
                    foreach (ListViewItem item in listView1.Items)
                    {
                        if (item.Text == selectedChatName)
                        {
                            item.Selected = true;
                            break; // Прерываем цикл после первого найденного элемента
                        }
                    }
                    UpdateChatMessages(chats[selectedChatName]);
                }

            };
            createChatButton.BringToFront();
            createChatButton.Location = new Point(0, listView1.Height - Height);
            flowLayoutPanel.Controls.Add(createChatButton);
        }
    }
}
