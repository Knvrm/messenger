﻿using System;
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
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
                MessageBox.Show("Не все поля заполнены", "Ошибка регистрации", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (textBox2.Text != textBox3.Text)
                MessageBox.Show("Пароли не совпдают", "Ошибка регистрации", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                if(!IsPasswordSecure(textBox2.Text))
                    MessageBox.Show("Пароль должен содержать не менее 8 символов и хотя бы одну цифру.", "Ошибка регистрации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    Send(stream, "REGISTRATION");
                    Send(stream, textBox1.Text);
                    string reply = ReceiveString(stream);
                    if (reply == "USER_EXIST")
                        MessageBox.Show("Пользователь с таким логином уже существует", "Ошибка регистрации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        Send(stream, textBox2.Text);
                        string text = ReceiveString(stream);
                        if (text == "SUCCESFUL_REGISTRATION")
                            MessageBox.Show("Вы успешно зарегистрировались", "Регистрация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show("Что-то пошло не так", "Ошибка регистрации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
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
        public static bool IsPasswordSecure(string password)
        {
            // Проверка длины пароля
            if (password.Length < 8)
            {
                return false;
            }

            // Проверка наличия цифр в пароле
            if (!password.Any(char.IsDigit))
            {
                return false;
            }


            return true;
        }
    }
}
