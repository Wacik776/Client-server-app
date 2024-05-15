using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using ClientOs;

namespace Server1
{
    static class Program
    {

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            bool onlyInstance;
            Mutex mtx = new Mutex(true, "Server1", out onlyInstance);
            if (onlyInstance)
                Application.Run(new Form1());
            else
                MessageBox.Show("Приложение уже запущено", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            mtx.ReleaseMutex();
            mtx.Close();

        }
    }
}

