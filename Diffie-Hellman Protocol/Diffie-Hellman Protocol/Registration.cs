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
using System.Windows.Forms.VisualStyles;
using static Diffie_Hellman_Protocol.NetworkStreamManager;

namespace Diffie_Hellman_Protocol
{
    public partial class Registration : Form
    {
        NetworkStream stream;
        public Registration(NetworkStream stream)
        {
            InitializeComponent();
            this.stream = stream;
        }

        private void Registration_Load(object sender, EventArgs e)
        {
            

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (richTextBox1.Text == "" || richTextBox2.Text == "" || richTextBox3.Text == "")
                MessageBox.Show("Не все поля заполнены", "Ошибка регистрации", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (richTextBox2.Text != richTextBox3.Text)
                MessageBox.Show("Пароли не совпдают", "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                Send(stream, "REGISTRATION");
                Send(stream, richTextBox1.Text);
                Send(stream, richTextBox2.Text);
                string text = ReceiveString(stream);
                if(text == "SUCCESFUL_REGISTRATION")
                    MessageBox.Show("Вы успешно зарегистрировались", "Регистрация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Что-то пошло не так", "Регистрация", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (!form.Visible)
                {
                    form.Visible = true;
                }
            }
            Close();
        }
    }
}
