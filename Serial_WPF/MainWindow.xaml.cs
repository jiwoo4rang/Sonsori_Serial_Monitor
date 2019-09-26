using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO.Ports;
using System.Threading;
using System.Speech.Synthesis;

namespace Serial_WPF
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort sport = null;
        Thread readAduinoThread;
        bool running = false;
        TextBox tbox = null;
        Thread testAduinoThread;

        public string data0 = "";
        public string data1 = "";
        public string data2 = "";
        public string data3 = "";

        public MainWindow()
        {
            InitializeComponent();
            SerialTest();
        }

        private void SerialTest()
        {
            sport = new SerialPort();
            sport.PortName = "COM8";
            sport.BaudRate = 9600;
            sport.Parity = Parity.None;
            sport.DataBits = 8;
            sport.StopBits = StopBits.One;
            sport.Handshake = Handshake.None;

            sport.ReadTimeout = 3000; // 아두이노와 맞아야함

            try
            {
                sport.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Open error: " + ex.Message);
                txtb_info.Text = "Open error: " + ex.Message;
            }
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            running = true;
            readAduinoThread = new Thread(arduReader);
            readAduinoThread.Start();
            Console.WriteLine("Connect");
            txtb_info.Text = "Connect";
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            running = false;
            if (sport != null) sport.Close();
            Console.WriteLine("Disconnect");
            txtb_info.Text = "Disconnect";
        }

        public void arduReader()
        {
           
            while (running)
            {
                string str = "";
                try
                {
                    str = sport.ReadLine();
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new Action(delegate
                    {
                        txtb_info.Text = "Serial Read error..";

                    }));
                    Console.WriteLine("Serial Read error..");
                    continue;
                }
                
                string[] strs = str.Split('=');
                string str2 = string.Format("장치: {0}  값:{1}", strs[0], Convert.ToInt32(strs[1]));
                Console.WriteLine(str2);

                Dispatcher.Invoke(new Action(delegate
                {
                    txtb_info.Text = "Serial Read Success..";
                    if (strs[0] == "a")
                    {
                        data0 = strs[1].ToString(); 
                        Console.WriteLine(data0);
                    }
                    else if (strs[0] == "b")
                    {
                        data1 = strs[1].ToString();
                        Console.WriteLine(data1);
                    }
                    else if (strs[0] == "c")
                    {
                        data2 = strs[1].ToString();
                        Console.WriteLine(data2);
                    }
                    else if (strs[0] == "d")
                    {
                        data3 = strs[1].ToString();
                        Console.WriteLine(data3);
                    }

                    //데이터 가공 함수 (가공 및 출력)
                    try
                    {
                        if(data0!="" && data1 != ""&& data2 != ""&& data3 != "")
                            processData(data0, data1, data2, data3);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e);
                    }
                   
                }));

            }
        }

        string strt = ""; // txtStr에 들어갈 내용
        int d0, d1, d2,d3; // 가공된 데이터 값 0,1,2

        private void btnjump_Click(object sender, RoutedEventArgs e)
        {
            result_value();
        }

        private void result_value()
        {
            // K = 2 2 0 0
            if (d0.Equals(0) && d1.Equals(1) && d2.Equals(0) && d3.Equals(2))
            {
                txtResult.Text = "K";
                strt = "K";
                txtstr.Text += strt;
            }
            // O = 1 1 1 1
            else if (d0.Equals(1) && d1.Equals(1) && d2.Equals(1) && d3.Equals(1))
            {
                txtResult.Text = "O";
                strt = "O";
                txtstr.Text += strt;
            }
            // N = 1 1 2 2
            else if (d0.Equals(1) && d1.Equals(1) && d2.Equals(2) && d3.Equals(2))
            {
                txtResult.Text = "N";
                strt = "N";
                txtstr.Text += strt;
            }
            // Y = 0 0 0 2
            else if (d0.Equals(0) && d1.Equals(0) && d2.Equals(0) && d3.Equals(2))
            {
                txtResult.Text = "Y";
                strt = "Y";
                txtstr.Text += strt;
            }
            // A = 0 0 0 0
            else if (d0.Equals(0) && d1.Equals(0) && d2.Equals(0) && d3.Equals(0))
            {
                txtResult.Text = "A";
                strt = "A";
                txtstr.Text += strt;
            }
            // G = 2 0 0 0
            else if (d0.Equals(2) && d1.Equals(0) && d2.Equals(0) && d3.Equals(0))
            {
                txtResult.Text = "G";
                strt = "G";
                txtstr.Text += strt;
            }
        }

        private void processData(string data0, string data1, string data2, string data3)
        {
            
            // if문 -> 범위 지정해서 0,1,2로 가공
            // 0
            if (int.Parse(data0).Equals(0))
            {
                d0 = 0;
                txt_Value1.Text = "구부러짐";
            }
                
            if (int.Parse(data1).Equals(0))
            {
                d1 = 0;
                txt_Value2.Text = "구부러짐";
            }
            if (int.Parse(data2).Equals(0))
            {
                d2 = 0;
                txt_Value3.Text = "구부러짐";
            }
            if (int.Parse(data3).Equals(0))
            {
                d3 = 0;
                txt_Value4.Text = "구부러짐";
            }

            // 1
            if (int.Parse(data0).Equals(1))
            {
                d0 = 1;
                txt_Value1.Text = "반접힘";
            }
            if (int.Parse(data1).Equals(1))
            {
                d1 = 1;
                txt_Value2.Text = "반접힘";
            }
            if (int.Parse(data2).Equals(1))
            {
                d2 = 1;
                txt_Value3.Text = "반접힘";
            }
            if (int.Parse(data3).Equals(1))
            {
                d3 = 1;
                txt_Value4.Text = "반접힘";
            }


            // 2
            if (int.Parse(data0).Equals(2))
            {
                d0 = 2;
                txt_Value1.Text = "펴짐";
            }
            if (int.Parse(data1).Equals(2))
            {
                d1 = 2;
                txt_Value2.Text = "펴짐";
            }
            if (int.Parse(data2).Equals(2))
            {
                d2 = 2;
                txt_Value3.Text = "펴짐";
            }
            if (int.Parse(data3).Equals(2))
            {
                d3 = 2;
                txt_Value4.Text = "펴짐";
            };
          

            // 가공 후, data0,1,2,3 if문 사용해서 문자 출력
            // K = 2 2 0 0
           
            

        }

        private void btnSpeek_Click(object sender, RoutedEventArgs e)
        {
            SpeechSynthesizer ts = new SpeechSynthesizer();

            // 보이스  처리됨
            //ts.SelectVoice("Microsoft Server Speech Text to Speech Voice (ko-KR, Heami)");
            ts.SetOutputToDefaultAudioDevice();
            ts.Speak(txtstr.Text);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtstr.Text = "";
            txtResult.Text = "";
        }
    }
}
