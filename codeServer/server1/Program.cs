 using System;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Management;

namespace Server1
{
    class Program
    {
        static Dictionary<int, int> busyports = new Dictionary<int, int>();

        static int port = 9306; // порт для приема входящих запросов
        static int maxconnections = 5;
        static void Main(string[] args)
        {
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length < 2)
            {
                Thread myThread1 = new Thread(start);
                myThread1.Start();

                for (int i = 0; i < maxconnections; i++)
                {
                    Thread myThread = new Thread(new ParameterizedThreadStart(podkl));
                    myThread.Start(port + i);
                }
            }
        }


        static void start()
        {
            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.2"), 9305);

            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);
                //DateTime.Now.ToShortTimeString()

                while (true)
                {
                    Socket handler = listenSocket.Accept();
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[256]; // буфер для получаемых данных

                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);
                    string message = "";

                    if (builder.ToString().Split('!')[0].Equals("GetPort"))
                    {
                        for (int i = 0; i < maxconnections; i++)
                        {
                            if (!busyports.ContainsValue(9306 + i))
                            {
                                if (busyports.ContainsKey(int.Parse(builder.ToString().Split('!')[1])))
                                {
                                    int value = 0;
                                    busyports.TryGetValue(int.Parse(builder.ToString().Split('!')[1]), out value);
                                    message = value.ToString();
                                    data = Encoding.Unicode.GetBytes(message);
                                    handler.Send(data);
                                    handler.Shutdown(SocketShutdown.Both);
                                    handler.Close();
                                    break;
                                }
                                busyports.Add(int.Parse(builder.ToString().Split('!')[1]), 9306 + i);
                                message = (9306 + i).ToString();
                                data = Encoding.Unicode.GetBytes(message);
                                handler.Send(data);
                                handler.Shutdown(SocketShutdown.Both);
                                handler.Close();
                                break;
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            int value = 0;
                            busyports.TryGetValue(int.Parse(builder.ToString().Split('!')[1]), out value);
                            message = value.ToString();
                            busyports.Remove(int.Parse(builder.ToString().Split('!')[1]));
                            data = Encoding.Unicode.GetBytes(message);
                            handler.Send(data);
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                        }
                        catch
                        {
                            message = "error";
                            data = Encoding.Unicode.GetBytes(message);
                            handler.Send(data);
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        static List<string> GetHardwareInfo(string WIN32_Class, string ClassItemField)
        {
            List<string> result = new List<string>();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM " + WIN32_Class);

            try
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    result.Add(obj[ClassItemField].ToString().Trim());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }
        static void podkl(object x)
        {
            int port1 = (int)x;
            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.2"), port1);

            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            


            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);
                //DateTime.Now.ToShortTimeString()
                Console.WriteLine($"Сервер №{port1 - 9305} запущен, находится в ожидании подключений");

                string  GPU = GetHardwareInfo("Win32_VideoController", "Name")[0];
                
                
                while (true)
                {
                    Socket handler = listenSocket.Accept();
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[256]; // буфер для получаемых данных

                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);
                    string message = "";

                    string[] a;
                    int h=0,w=0;


                    try
                    {
                        a = builder.ToString().Split(':');
                        h = int.Parse(a[0]);
                        w = int.Parse(a[1]);
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message + " ! " + DateTime.Now.ToString();
                    }
                    if (message.Equals(""))
                    {
                        message = h.ToString()+" ! "+ w.ToString() + " ! " + GPU + " ! ";
                    }
                    data = Encoding.Unicode.GetBytes(message);
                    handler.Send(data);
                    // закрываем сокет
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
