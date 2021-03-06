using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Threading;
namespace ccucom
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리 https://chud.tistory.com/8
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // 시리얼 통신 관련 함수 호출
            Loaded += new RoutedEventHandler(InitSerialPort);
        }

        #region < 시리얼 통신 관련 함수 >

        SerialPort serial = new SerialPort();
        string g_sRecvData = String.Empty;
        delegate void SetTextCallBack(String text);

        //
        // 시리얼 통신 초기화
        //
        void InitSerialPort(object sender, EventArgs e)
        {
            serial.DataReceived += new SerialDataReceivedEventHandler(serial_DataReceived);

            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                cbComPort.Items.Add(port);
            }

            sbStrip1.Content = "통신포트와 통신속도를 선택해 주세요";
        }

        //
        // 콤보박스에서 시리얼 통신포트 선택시 발생되는 이벤트 핸들러
        //
        private void cbComPort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 통신포트가 열려 있을 경우 닫음
            if (serial.IsOpen)
            {
                serial.Close();
            }

            serial.PortName = cbComPort.SelectedItem.ToString();
            sbStrip1.Content = serial.PortName + " : " + serial.BaudRate + ", 8N1";
            serial.ReadTimeout = 1000;
            OpenComPort(sender, e);
        }

        //
        // 시리얼 통신 열기 메소드
        //
        void OpenComPort(object sender, RoutedEventArgs e)
        {
            try
            {
                serial.Open();
            }
            catch (Exception ex)
            {
                sbStrip1.Content = "통신포트 " + serial.PortName + "열 수 없습니다";
                cbComPort.SelectedItem = "";
            }
        }

        // 콤보박스에서 시리얼 통신속도 선택시 발생되는 이벤트 핸들러
        private void cbComSpeed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string[] names = cbComSpeed.SelectedItem.ToString().Split(':');
            serial.BaudRate = int.Parse(names[1].ToString().Trim());

            // 통신포트가 열려 있을 경우 닫음
            if (serial.IsOpen)
            {
                sbStrip1.Content = serial.PortName + " : " + serial.BaudRate + ", 8N1";
            }
        }

        //
        // 시리얼 데이터 수신시 발생되는 이벤트 핸들러
        //
        int aa = 0;
        byte[] inputx = { 0,0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0} ;

        void serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {


              //  string str= serial.ReadLine();
            //    SetText("==>" + str + "test\n");
             
             String recv_data_0 = "";
             Thread.Sleep(100);
             int bytesize = 14;
             byte[] tt = new byte[bytesize];
             serial.Read(tt, 0, bytesize);

             for (int i = 0; i < tt.Length; i++)                {
                 //recv_data_0 += ("L=>"+tt.Length+" " + tt[i] + " ");
                 recv_data_0 += Convert.ToString(tt[i].ToString("X2") + " ");
             }


                //tt[] + tt[i]
                
                int a;
               
                   a = tt[4] << 24;
                    a |= tt[5] << 16;
                    a |= tt[6] << 8;
                    a |= tt[7] << 0;
                if (a < 1000) SetVolt(a.ToString());

                    a = tt[8] << 24;
                    a |= tt[9] << 16;
                    a |= tt[10] << 8;
                    a |= tt[11] << 0;
                if (a < 1000) SetAmp(a.ToString());
                 

              



                SetText("==>"+ recv_data_0 + "test\n");

             /*
             int RecvSize = 100;
             string RecvStr = string.Empty;
             // Recv Data가 있는 경우...
             if (RecvSize != 0)
             {
                 byte[] buff = new byte[RecvSize];
                 SetText(RecvSize + "size\n");

                 serial.Read(buff, 0, RecvSize);

                 String print = "";
                     foreach (byte bData in buff)
                 {

                     print += bData.ToString() + " ";

                 }
                       //  RecvStr += " " + bData.ToString("X2");
              //   }
                 //textBox.Text += RecvStr;

                 SetText(print +"test\n");
             }


             */

                /*
                int a = serial.ReadByte();
             SetText(a.ToString() + " ");
                inputx[aa]= Convert.ToByte(a);
                aa++;

                if (a== 3)
                {  
                    aa = 0;
                  SetText($" a  \n "  );
                } else
                {
                    
                    if (aa == 12)
                    {
                        int sum = 0;
                         
                        for (int i = 3; i <= 11; i++)
                        {
                            sum += (int)inputx[i] ;

                        }
                        sum = sum & 0xFF;
                        Console.WriteLine(sum);

                        int x =  255 - sum ;
                       

                       // SetText(" ==============> "+ x);
                    }
                }
             */
            }
            catch (TimeoutException)
            {
                g_sRecvData = string.Empty;
            }
        }

        //
        // 수신 데이터 처리 메소드
        //
        private void SetText(string text)
        {
            if (tbRecvData.Dispatcher.CheckAccess())
            {
                tbRecvData.AppendText(text);
            }
            else
            {
                SetTextCallBack d = new SetTextCallBack(SetText);
                tbRecvData.Dispatcher.Invoke(d, new object[] { text });
            }
        }

        private void SetVolt(string text)
        {
            if (volt.Dispatcher.CheckAccess())
            {
                volt.Text=text;
            }
            else
            {
                SetTextCallBack d = new SetTextCallBack(SetVolt);
                volt.Dispatcher.Invoke(d, new object[] { text });
            }
        }

        private void SetAmp(string text)
        {
            if (amp.Dispatcher.CheckAccess())
            {
                amp.Text = text;
            }
            else
            {
                SetTextCallBack d = new SetTextCallBack(SetAmp);
                volt.Dispatcher.Invoke(d, new object[] { text });
            }
        }


        //
        // 데이터 전송 버튼 클릭시 발생되는 이벤트 핸들러
        //
        private void btnSendData_Click(object sender, RoutedEventArgs e)
        {
            if (serial.IsOpen)
            {
                serial.WriteLine(tbSendData.Text);
            }
            else
            {
                sbStrip1.Content = "통신포트가 열리지 않았습니다";
            }
        }

        //
        // 화면 지우기 버튼을 클릭시 발생되는 이벤트 핸들러
        //
        private void btnScreenClear_Click(object sender, RoutedEventArgs e)
        {
            tbRecvData.Text = string.Empty;
        }

        #endregion 시리얼 통신 관련 함수

        private void Window_Closed(object sender, EventArgs e)
        {
            // 윈도우 종료시 시리얼 포트 닫기 
            if (serial.IsOpen) serial.Close();
        }


    }
}
