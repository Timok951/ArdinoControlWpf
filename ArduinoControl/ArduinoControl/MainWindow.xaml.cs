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
using System.Timers;
using System.IO.Ports;


namespace ArduinoControl
{


    public partial class MainWindow : Window
    {
        System.Timers.Timer aTimer;
        SerialPort currentPort;
        private delegate void UpdateDelegate(string txt);

        private bool ArduinoDetected()
        {
            try
            {
                currentPort.Open();
                System.Threading.Thread.Sleep(1000);//a little pause port do not like fast movments
                string returnMessage = currentPort.ReadLine();
                currentPort.Close();
                //loop must have message info from arduino
                if (returnMessage.Contains("Info from Arduino"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e) {
                return false;
                    }
        }

        private void OnTImedEvent(object sender, EventArgs e)
        {
            if (!currentPort.IsOpen) return;
            try//after sceen was closed timer can execute 
            {
                //delete buffer 
                currentPort.DiscardInBuffer();
                //read last value 
                string stFromPort = currentPort.ReadLine();
                lblPortData.Dispatcher.BeginInvoke(new UpdateDelegate(updateTextBox), stFromPort);
                if (stFromPort.Contains("b"))
                {
                    string btn = "buttn1 Press";
                    bttnpress.Dispatcher.BeginInvoke(new UpdateDelegate(updateButton), btn);
                }
                else if (stFromPort.Contains("c"))
                {
                    string btn = "buttn2 Press";
                    bttnpress.Dispatcher.BeginInvoke(new UpdateDelegate(updateButton), btn);
                }

            }
            catch
            { }
            }

        public MainWindow()
        {
            InitializeComponent();
            bool ArduinoPortFound = false;

            List<string> frequency = new List<string> { "0.1" , "0.5" ,"1" };
            FrequencyCmbx.ItemsSource = frequency;

            try
            {
                string[] ports = SerialPort.GetPortNames();
                foreach (string port in ports)
                {
                    currentPort = new SerialPort(port, 9600);
                    if (ArduinoDetected())
                    {
                        ArduinoPortFound = true;
                        break;
                    }
                    else
                    {
                        ArduinoPortFound = false;
                    }
                }
            }
            catch { }
            if (ArduinoPortFound == false) return;
            System.Threading.Thread.Sleep(500);//wait a little

            currentPort.BaudRate = 9600;
            currentPort.ReadTimeout = 1000;
            try
            {
                currentPort.Open();
            }
            catch{ }
            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += OnTImedEvent;
            aTimer.Enabled = true;
        }

        private void updateTextBox(string txt)
        {
            lblPortData.Content = txt;
        }
        private void updateButton(string txt)
        {
            bttnpress.Content = txt;
        }

        private void ButnOne_Click(object sender, RoutedEventArgs e)
        {
            switch (FrequencyCmbx.SelectedItem as string)
            {
                case "0.1":
                    if (!currentPort.IsOpen) return;
                    currentPort.Write("1");
                    break;
                case "0.5":
                    if (!currentPort.IsOpen) return;
                    currentPort.Write("2");
                    break;

                case "1":
                    if (!currentPort.IsOpen) return;
                    currentPort.Write("3");
                    break;

            }





        }

        private void ButnTwo_Click(object sender, RoutedEventArgs e)
        {
            if (!currentPort.IsOpen) return;
                currentPort.Write("0");
            
        }
    }
}