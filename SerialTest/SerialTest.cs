using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO.Ports;
using System.Threading;

namespace ProjectClass
{ 
    partial class SerialTest:Window
    {
        SerialPort sport = null;
        Thread readAduinoThread;
        bool running = false;
        TextBox tbox = null;

        [STAThread]
        static void Main(string[] args)
        {
            new Application().Run(new SerialTest());
        }

        public SerialTest()
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            Title = "Serial Test";
            Background = Brushes.Blue;

            tbox = new TextBox();
            tbox.Width = 500;
            tbox.Height = 300;
            tbox.Background = Brushes.Blue;
            tbox.Foreground = Brushes.White;
            tbox.TextWrapping = TextWrapping.Wrap;
            tbox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            

            tbox.Text = "Test line\n테스트입니다.";

            Content = tbox;

            Closing += SerialTest_Closing;

            //-----------------------

            sport = new SerialPort();
            sport.PortName = "COM7";
            sport.BaudRate = 9600;
            sport.Parity = Parity.None;
            sport.DataBits = 8;
            sport.StopBits = StopBits.One;
            sport.Handshake = Handshake.None;

            sport.ReadTimeout = 1000;
        
            try { 
                sport.Open();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Open error: " + ex.Message);
            }

            //sport.DataReceived += Sport_DataReceived;
            //-----------------------
            running = true;
            readAduinoThread = new Thread(arduReader);
            readAduinoThread.Start();


        }

        private void SerialTest_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            running = false;
            //readAduinoThread.Join();
            if (sport != null) sport.Close();
            Console.WriteLine("Bye..");
            
        }

       //private void Sport_DataReceived(object sender, SerialDataReceivedEventArgs e)
       // {
       //     string str = "";
       //     try
       //     {
       //         str = sport.ReadLine();
       //     }
       //     catch (Exception ex)
       //     {
       //         Console.WriteLine("Serial Read error" + ex.Message);
       //     }

       //     Console.WriteLine(str);
       //     Dispatcher.Invoke(new Action(delegate {
       //         tbox.Text = tbox.Text + str + "\n";
       //     }));
       // }

        //Thread serial 포트로부터 데이터를 읽어서 텍스트 박스에 출력하는 쓰레드
        private void arduReader()
        {
            int count = 0;
            while(running)
            {
                string str = "";
                try { 
                    str = sport.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Serial Read error..");
                    continue; 
                }

                string[] strs = str.Split('=');
                string str2 = string.Format("장치: {0}  값:{1}", strs[0], Convert.ToInt32(strs[1]));
                Console.WriteLine(str2);
                Dispatcher.Invoke(new Action(delegate {
                    tbox.Text = tbox.Text + str2 + "\n";
                    tbox.ScrollToEnd();
                }));

                if(++count > 100)
                {
                    count = 0;
                    Dispatcher.Invoke(new Action(delegate {
                        tbox.Clear();
                    }));
                }
            }
            Console.WriteLine("Thread terminated..");
        }
    }
}

