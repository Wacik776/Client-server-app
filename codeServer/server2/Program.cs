using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace Server2
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
                Thread myThread1 = new Thread(osnova);
                myThread1.Start();

                for (int i = 0; i < maxconnections; i++)
                {
                    Thread myThread = new Thread(new ParameterizedThreadStart(podkl));
                    myThread.Start(port + i);
                }
            }
        }


        static void osnova()
        {
            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9305);

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


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MemoryStatus
        {
            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern bool GlobalMemoryStatusEx([In, Out] MemoryStatus lpBuffer);

            private uint dwLength;
            public uint MemoryLoad;
            public ulong TotalPhys;
            public ulong AvailPhys;
            public ulong TotalPageFile;
            public ulong AvailPageFile;
            public ulong TotalVirtual;
            public ulong AvailVirtual;
            public ulong AvailExtendedVirtual;

            private static volatile MemoryStatus singleton;
            private static readonly object syncroot = new object();

            public static MemoryStatus CreateInstance()
            {
                if (singleton == null)
                    lock (syncroot)
                        if (singleton == null)
                            singleton = new MemoryStatus();
                return singleton;
            }

            [SecurityCritical]
            private MemoryStatus()
            {
                dwLength = (uint)Marshal.SizeOf(typeof(MemoryStatus));
                GlobalMemoryStatusEx(this);
            }
        }

        static void podkl(object x)
        {
            int port1 = (int)x;
            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port1);

            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);




            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);
                
                Console.WriteLine($"Сервер №{port1-9305} запущен, находится в ожидании подключений");

                while (true)
                {
                    Socket handler = listenSocket.Accept();
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[3024]; // буфер для получаемых данных

                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);
                    string message = "";

                    string[] a;
                    int fk;
                    MemoryStatus status = MemoryStatus.CreateInstance();
                    uint MemoryLoad = status.MemoryLoad;
                    ulong TotalPhys = status.TotalPhys;
                    ulong AvailPhys = status.AvailPhys;
                    ulong TotalPageFile = status.TotalPageFile;
                    ulong AvailPageFile = status.AvailPageFile;
                    ulong TotalVirtual = status.TotalVirtual;
                    ulong AvailVirtual = status.AvailVirtual;
                    ulong AvailExtendedVirtual = status.AvailExtendedVirtual;


                    ulong podkach = AvailVirtual;
                    ulong ispolzFiz = (TotalPhys - AvailPhys);
              
                    a = builder.ToString().Split(':');
                    fk = int.Parse(a[0]);
                    if (fk == 1)
                    {
                        message = podkach + " байт" + "!" + ispolzFiz + " байт" + "!";
                        data = Encoding.Unicode.GetBytes(message);
                        handler.Send(data);
                        // закрываем сокет
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                    if (fk == 2)
                    {

                        podkach = podkach / 1024;
                        ispolzFiz = ispolzFiz / 1024;
                        message = podkach + " кбайт" + "!" + ispolzFiz + " кбайт" + "!";
                        data = Encoding.Unicode.GetBytes(message);
                        handler.Send(data);
                        // закрываем сокет
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                    if (fk == 3)
                    {
                        podkach = (podkach / 1024) / 1024;
                        ispolzFiz = (ispolzFiz / 1024) / 1024;
                        message = podkach + " мбайт" + "!" + ispolzFiz + " мбайт" + "!";
                        data = Encoding.Unicode.GetBytes(message);
                        handler.Send(data);
                        // закрываем сокет
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                    if (fk == 4)
                    {
                        podkach = ((podkach / 1024) / 1024) / 1024;
                        ispolzFiz = ((ispolzFiz / 1024) / 1024) / 1024;
                        message = podkach + " гигабайт" + "!" + ispolzFiz + " гигабайт" + "!";
                        data = Encoding.Unicode.GetBytes(message);
                        handler.Send(data);
                        // закрываем сокет
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }





                    //реакция по серверу
                    
                    System.Threading.Thread.Sleep(4000);
                    MemoryStatus status1 = MemoryStatus.CreateInstance();
                    ulong podkachSafe = status1.AvailVirtual;
                    ulong ispolzFizSafe = (status1.TotalPhys - status1.AvailPhys);
                    string message1 = podkach + " байт" + " ! " + ispolzFiz + " байт" + " ! ";
                    if (fk == 5)
                    {
                        
                        
                       
                        message = "";
                         message = podkachSafe + " байт" + " ! " + ispolzFizSafe + " байт"+ " ! "  ;
                       
                        if (!message.Equals(message1)) {
                            
                            message += message1;
                        }


                        data = Encoding.Unicode.GetBytes(message);
                        handler.Send(data);
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                    
                    if (fk == 6)
                    {
                        message = (podkachSafe / 1024) + " кбайт" + "!" + (ispolzFizSafe / 1024) + " кбайт" + "!";
                        if (!message.Equals(message1))
                        {

                            message += message1;
                        }
                        handler.Send(data);
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                    if (fk == 7)
                    {
                        System.Threading.Thread.Sleep(100);
                        
                            
                                message = ((podkachSafe / 1024) / 1024) + " мбайт" + "!" + ((ispolzFizSafe / 1024) / 1024) + " мбайт" + "!";
                            if (!message.Equals(message1))
                            {

                                message += message1;
                            }
                            handler.Send(data);
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                        }
                    if (fk == 8)
                    {
                           
                        message = (((podkachSafe / 1024) / 1024) / 1024) + " гигабайт" + "!" + (((ispolzFizSafe / 1024) / 1024) / 1024) + " гигабайт" + "!";
                        if (!message.Equals(message1))
                        {

                            message += message1;
                        }
                        handler.Send(data);
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Socket handler = listenSocket.Accept();
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }

    }
}

