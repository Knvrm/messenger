using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diffie_Hellman_Protocol
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Server mainForm = new Server();
            Client secondForm = new Client();

            //mainForm.Show();
            secondForm.Show();

            Application.Run(secondForm); // Используем mainForm в качестве основной формы

            // После закрытия mainForm выполним выход из приложения
            Application.ApplicationExit += (sender, e) =>
            {    
            };
        }
    }
}
