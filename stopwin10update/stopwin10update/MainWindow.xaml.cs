using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace stopwin10update
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string serviceName = "";
        public MainWindow()
        {
            InitializeComponent();


            serviceName = "wuauserv";
            ServiceController controller = GetService();
            ///add timer
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }



        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            RefreshLabel();
        }
        /// <summary>
        /// Refresh the status lable value
        /// </summary>
        private void RefreshLabel()
        {

            lblstatus.Dispatcher.BeginInvoke(new Action(delegate
            {
                // Do your work
                bool isRunning = IsServiceRunning();
                string status;
                if (isRunning)
                    status = "is running";
                else
                    status = "is stopped";
                lblstatus.Content = "Windows Update " + status;
            }));

        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StopService();

            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }

        }
        /// <summary>
        /// Get Service
        /// </summary>
        /// <param name="serviceName">Service Name</param>
        public static ServiceController GetService()
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (var item in services)
            {
                if (item.ServiceName.Equals(serviceName))
                {
                    return item;
                }
            }
            return services[0];
        }
        /// <summary>
        /// check service status
        /// </summary>
        public static bool IsServiceRunning()
        {
            ServiceControllerStatus status;
            uint counter = 0;
            do
            {
                ServiceController service = GetService();
                if (service == null)
                {
                    return false;
                }

                Thread.Sleep(100);
                status = service.Status;
            } while (!(status == ServiceControllerStatus.Stopped ||
                       status == ServiceControllerStatus.Running) &&
                     (++counter < 30));
            return status == ServiceControllerStatus.Running;
        }

        public static bool IsServiceInstalled(string serviceName)
        {
            return GetService() != null;
        }

        public static void StartService()
        {
            ServiceController controller = GetService();
            if (controller == null)
            {
                return;
            }

            controller.Start();
            controller.WaitForStatus(ServiceControllerStatus.Running);
        }
        public static void StopService()
        {
            ServiceController controller = GetService();
            if (controller == null)
            {
                return;
            }
            controller.Stop();
            controller.WaitForStatus(ServiceControllerStatus.Stopped);
        }
    }
}
