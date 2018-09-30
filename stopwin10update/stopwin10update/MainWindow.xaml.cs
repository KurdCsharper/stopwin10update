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
namespace stopwin10update
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StopService("WinDefend");

            }
            catch (Exception er)
            {

                MessageBox.Show(er.Message.ToString());
            }

        }
        public static ServiceController GetService(string serviceName)
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
        public static bool IsServiceRunning(string serviceName)
        {
            ServiceControllerStatus status;
            uint counter = 0;
            do
            {
                ServiceController service = GetService(serviceName);
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
            return GetService(serviceName) != null;
        }

        public static void StartService(string serviceName)
        {
            ServiceController controller = GetService(serviceName);
            if (controller == null)
            {
                return;
            }

            controller.Start();
            controller.WaitForStatus(ServiceControllerStatus.Running);
        }
        public static void StopService(string serviceName)
        {
            ServiceController controller = GetService("WinDefend");
            if (controller == null)
            {
                return;
            }
            controller.Stop();
            controller.WaitForStatus(ServiceControllerStatus.Stopped);
        }
    }
}
