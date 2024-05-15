using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;

namespace ClientOs
{
    public partial class Form1 : Form
    {
        public static int port = 9305;
        public static int numb = 1;
        public static bool inloop = false;
        public static string lastMSG = " ";
        public static string forset = "";
        public static string uslov = "";


        public static string address2 = "127.0.0.1";
        public static string address1 = "127.0.0.2";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)//connect 2 ser
        {
            
            ConServеtwo();
        }

        public void ConServеtwo()
        {
            port = 9305;
            try
            {
                IPEndPoint ipPoint1 = new IPEndPoint(IPAddress.Parse(address2), port);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // подключаемся к удаленному хосту
                socket.Connect(ipPoint1);
                string message = "GetPort!" + numb;
                byte[] dataBuf = Encoding.Unicode.GetBytes(message);
                socket.Send(dataBuf);

                // получаем ответ
                dataBuf = new byte[256]; // буфер для ответа
                StringBuilder strBuilder = new StringBuilder();
                int bytes = 0; // количество полученных байт

                do
                {
                    bytes = socket.Receive(dataBuf, dataBuf.Length, 0);
                    strBuilder.Append(Encoding.Unicode.GetString(dataBuf, 0, bytes));
                }
                while (socket.Available > 0);

                port = int.Parse(strBuilder.ToString());
                // закрываем сокет
                mainTextBox1.Text += Environment.NewLine + "Подключение открылось на сокете : " + address2 + ":" + strBuilder.ToString() + "\n";
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                mainTextBox1.Text += ex.Message + "\n";
            }

        }

        public void ViklSer2()//отключение от 2 сервера
        {
            
            port = 9305;
            try
            {
                IPEndPoint ipPointForFirstServ = new IPEndPoint(IPAddress.Parse(address2), port);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // подключаемся к удаленному хосту
                socket.Connect(ipPointForFirstServ);

                string message = "brkpt!" + numb;
                byte[] data = Encoding.Unicode.GetBytes(message);
                socket.Send(data);

                // получаем ответ
                data = new byte[1024]; // буфер для ответа
                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байт
                
                do
                {

                    bytes = socket.Receive(data, data.Length, 0);
                    
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (socket.Available > 0);
                mainTextBox1.Text += Environment.NewLine + "Подключение завершилось на сокете : " + address2 + ":" + builder.ToString() + "\n";
                // закрываем сокет
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                mainTextBox1.Text += ex.Message;
            }
        }

        private void Button2_Click(object sender, EventArgs e)//connect 1 ser
        {
            ConServFirst();
        }

        public void ConServFirst()
        {
            
            port = 9305;
            try
            {
                IPEndPoint ipPoint1 = new IPEndPoint(IPAddress.Parse(address1), port);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // подключаемся к удаленному хосту
                socket.Connect(ipPoint1);
                string message = "GetPort!" + numb;
                byte[] data = Encoding.Unicode.GetBytes(message);
                socket.Send(data);

                // получаем ответ
                data = new byte[256]; // буфер для ответа
                StringBuilder strBuilder = new StringBuilder();
                int bytes = 0; // количество полученных байт

                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    strBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (socket.Available > 0);

                port = int.Parse(strBuilder.ToString());
                // закрываем сокет
                mainTextBox1.Text += Environment.NewLine + "Подключение открылось на сокете : " +address1 + ":" + strBuilder.ToString() + "\n";
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                mainTextBox1.Text += ex.Message + "\n";
            }

        }

        public void ViklServ1() //закрытие подключения
        {
            
            port = 9305;
            try
            {
                IPEndPoint ipPoint1 = new IPEndPoint(IPAddress.Parse(address1), port);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // подключаемся к удаленному хосту
                socket.Connect(ipPoint1);

                string message = "brkpt!" + numb;
                byte[] data = Encoding.Unicode.GetBytes(message);
                socket.Send(data);

                // получаем ответ
                data = new byte[256]; // буфер для ответа
                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байт

                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (socket.Available > 0);
                mainTextBox1.Text += Environment.NewLine + "Подключение завершилось на сокете : " + address1+":" + builder.ToString() + "\n";
                // закрываем сокет
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                mainTextBox1.Text += ex.Message + "\n";
            }
        }



        private void Button3_Click(object sender, EventArgs e)//send 2
        {
            if (address2 == "127.0.0.1")
            {
                try
                {
                    Serv2(address2, port);
                    if (uslov.Equals("1байт") || uslov.Equals("1килобайт") || uslov.Equals("1мегабайт") || uslov.Equals("1гигабайт") || lastMSG.Equals(forset)) { }
                    else
                    {
                        mainTextBox1.Text += forset;
                        lastMSG = forset;
                    }
                }
                catch (Exception ex)
                {
                    mainTextBox1.Text += Environment.NewLine + "Вы не подключены к серверу 2!";
                }
            }
            else
            {
                mainTextBox1.Text += Environment.NewLine + "Вы не подключены к серверу 2!";
            }
        }

        public void Serv2(string address, int port1)
        {
            if (address2 == "127.0.0.1" )
            {
                IPEndPoint ipPoint1 = new IPEndPoint(IPAddress.Parse(address), port1);
                Socket socket1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                byte[] data;
                int bytes, siz;
                StringBuilder builder = new StringBuilder();

                // подключаемся к удаленному хосту
                socket1.Connect(ipPoint1);
                if (takeTextBox2.Text != "")
                {
                    siz = takeTextBox2.Text switch
                    {

                        "байт" => 1,
                        "килобайт" => 2,
                        "мегабайт" => 3,
                        "гигабайт" => 4,
                        "1байт" => 5,
                        "1килобайт" => 6,
                        "1мегабайт" => 7,
                        "1гигабайт" => 8,
                        _ => 1,
                    };
                    data = Encoding.Unicode.GetBytes(siz + ":");
                    socket1.Send(data);

                    if (siz >= 5)
                    {
                        forset = "";
                        data = new byte[3024]; // буфер для ответа

                        do
                        {
                            bytes = socket1.Receive(data, data.Length, 0);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));

                        }
                        while (socket1.Available > 0);
                        var dataForMem = builder.ToString().Split('!');
                        if (dataForMem.Length == 5)
                        {

                            mainTextBox1.Text += Environment.NewLine + "Количество свободных байтов файла подкачки: " + dataForMem[0] + ". Объем используемой физической памяти в единицах, переданных клиентом : " + dataForMem[1] + ";" + "\n";
                            System.Threading.Thread.Sleep(2000);
                            mainTextBox1.Text += Environment.NewLine + "Новое значение - Количество свободных байтов файла подкачки: " + dataForMem[3] + ". Объем используемой физической памяти в единицах, переданных клиентом : " + dataForMem[4] + ";" + "\n";
                            uslov = takeTextBox2.Text;
                        }
                        else
                        {

                            mainTextBox1.Text += Environment.NewLine + "Значения не изменились";
                            

                        }

                    }
                    else
                    {
                        data = new byte[3024]; // буфер для ответа

                        do
                        {
                            bytes = socket1.Receive(data, data.Length, 0);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));

                        }
                        while (socket1.Available > 0);
                        var dataForMem = builder.ToString().Split('!');

                       

                        forset = Environment.NewLine + "Количество свободных байтов файла подкачки: " + dataForMem[0] + ". Объем используемой физической памяти в единицах, переданных клиентом : " + dataForMem[1] + "; " + "\n";
                        uslov = takeTextBox2.Text;
                    }
                }
                else
                {
                    data = Encoding.Unicode.GetBytes(1 + ":");
                    socket1.Send(data);
                    data = new byte[3024]; // буфер для ответа

                    do
                    {
                        bytes = socket1.Receive(data, data.Length, 0);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));

                    }
                    while (socket1.Available > 0);
                    var dataForMem = builder.ToString().Split('!');

                   

                    forset = Environment.NewLine + "Количество свободных байтов файла подкачки: " + dataForMem[0] + ". Объем используемой физической памяти в единицах, переданных клиентом : " + dataForMem[1] + "; " + "\n";
                    uslov = takeTextBox2.Text;
                }

                // закрываем сокет
                socket1.Shutdown(SocketShutdown.Both);
                socket1.Close();
            }
            else
            {
                mainTextBox1.Text += Environment.NewLine + "Вы не подключены к серверу 2!";
            }
        }
        private void Button4_Click(object sender, EventArgs e)//send 1
        {
            if (address1 == "127.0.0.2" )
            {
                try
                {
                    Serv1(address1, port);
                }
                catch (Exception ex)
                {
                    mainTextBox1.Text += Environment.NewLine + "Вы не подключены к серверу 1!";
                }
            }
            else {
                mainTextBox1.Text += Environment.NewLine + "Вы не подключены к серверу 1!";
            }
        }

        private void Serv1(string address, int port)
        {
            IPEndPoint ipPoint1 = new IPEndPoint(IPAddress.Parse(address), port);
            Socket socket1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            
            byte[] data;
            int bytes;
            StringBuilder builder = new StringBuilder();
            int h = int.Parse(this.Size.Height.ToString());
            int w = int.Parse(this.Size.Width.ToString());
            // подключаемся к удаленному хосту
            socket1.Connect(ipPoint1);


            data = Encoding.Unicode.GetBytes(h + ":" + w);
            socket1.Send(data);

            data = new byte[3024]; // буфер для ответа
            
            do
            {
                bytes = socket1.Receive(data, data.Length, 0);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));

            }
            while (socket1.Available > 0);
            var dataForMem = builder.ToString().Split('!');


            mainTextBox1.Text += Environment.NewLine+ "Размер клиентской области " + dataForMem[0] + " x " + dataForMem[1] + "\nНазвание видеоадаптера -" + dataForMem[2] + " " + DateTime.Now.ToString() + "; " + "\n";



            // закрываем сокет
            socket1.Shutdown(SocketShutdown.Both);
            socket1.Close();
        }

        private void Button5_Click(object sender, EventArgs e)//stop2
        {
            ViklSer2();
        }

        private void Button6_Click(object sender, EventArgs e)//stop1
        {
            ViklServ1();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                Random rnd = new Random();
                numb = rnd.Next(0, 100000);
            }
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                timer1.Enabled = true;
            }
            else
            {
                timer1.Enabled = false;
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (address2 == "127.0.0.1")
            {
                try
                {
                    Serv2(address2, port);
                }
                catch (Exception ex)
                {
                    mainTextBox1.Text += Environment.NewLine + "";
                }
                
                
                if (!forset.Equals(lastMSG))
                {
                    mainTextBox1.Text += forset;
                    lastMSG = forset;

                }
                else
                {
                    mainTextBox1.Text += Environment.NewLine + "Нет изменений";
                }

           

            }
            else
            {
                mainTextBox1.Text += Environment.NewLine + "Cервер 2 не запущен";
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            mainTextBox1.Text = "";
        }

    }
}

