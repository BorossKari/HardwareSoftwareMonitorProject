using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Management;
using System.IO;

namespace HardwareSoftwareMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ManagementObjectSearcher myVideoObject = new ManagementObjectSearcher("select * from Win32_VideoController");
            string driver;
            foreach (ManagementObject obj in myVideoObject.Get())
            {
                gpunev.Content = obj["Name"];
                gpuallapot.Content = obj["Status"];
                gpuid.Content = obj["DeviceID"];
                gpuram.Content = Math.Round((Convert.ToDouble(obj["AdapterRAM"]) / 1073741824), 2) + " GB";
                gpudac.Content = obj["AdapterDACType"];
                gpumono.Content = obj["Monochrome"];
                gpudriverversion.Content = obj["DriverVersion"];
                gpuprocessor.Content = obj["VideoProcessor"];
                gpuarch.Content = obj["VideoArchitecture"];
                gpumemtype.Content = obj["VideoMemoryType"];
                driver = Convert.ToString(obj["InstalledDisplayDrivers"]);
                foreach (var item in Convert.ToString(obj["InstalledDisplayDrivers"]).Split(','))
                {
                    driverslist.Items.Add(item);
                }
            }
            meghajtoinfo.AppendText("\r");
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                meghajtoinfo.AppendText(d.Name);
                meghajtoinfo.AppendText(" Meghajtó\r");
                meghajtoinfo.AppendText("    Meghajtó típusa: ");
                meghajtoinfo.AppendText(d.DriveType + "\r");
                if (d.IsReady == true)
                {
                    meghajtoinfo.AppendText("    Meghajtó neve: ");
                    meghajtoinfo.AppendText(d.VolumeLabel + "\r");
                    meghajtoinfo.AppendText("    Fájlrendszer: ");
                    meghajtoinfo.AppendText(d.DriveFormat + "\r");
                    meghajtoinfo.AppendText("    A felhasználó számára elérhető szabad tárhely: ");
                    meghajtoinfo.AppendText(Convert.ToString(d.AvailableFreeSpace/1073741824 + " Gigabyte\r"));
                    meghajtoinfo.AppendText("    Összes elérhető szabad tárhely: ");
                    meghajtoinfo.AppendText(Convert.ToString(d.TotalFreeSpace / 1073741824 + " Gigabyte\r"));
                    meghajtoinfo.AppendText("    Meghajtó teljes mérete: ");
                    meghajtoinfo.AppendText(Convert.ToString(d.TotalSize / 1073741824 + " Gigabyte\r"));
                    meghajtoinfo.AppendText("    Gyökérkönyvtár: ");
                    meghajtoinfo.AppendText(Convert.ToString(d.RootDirectory) + "\r");
                }
            }
            ManagementObjectSearcher myProcessorObject = new ManagementObjectSearcher("select * from Win32_Processor");

            foreach (ManagementObject obj in myProcessorObject.Get())
            {
                cpunev.Content = obj["Name"];
                cpuid.Content = obj["DeviceID"];
                cpugyarto.Content = obj["Manufacturer"];
                cpuseb.Content = obj["CurrentClockSpeed"];
                cpuleir.Content = obj["Caption"];
                cpucores.Content = obj["NumberOfCores"];
                cpuarch.Content = obj["Architecture"];
                cpufamily.Content = obj["Family"];
                cputype.Content = obj["ProcessorType"];
            }
        }
        private void homeres_Click(object sender, RoutedEventArgs e)
        {
            Double homerseklet = 0;
            String instanceName = "";

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");

            try
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    homerseklet = Convert.ToDouble(obj["CurrentTemperature"].ToString());
                    homerseklet = (homerseklet - 2732) / 10.0;
                    instanceName = obj["InstanceName"].ToString();
                }
                cputemperature.Content = homerseklet + " °C";
                instancename.Content = instanceName;
            }
            catch (ManagementException)
            {
                cputemperature.Content = "A rendszer megtagadta a hozzáférést ehhez az információhoz.";
                instancename.Content = "Próbálja meg adminisztrátorként futtatni a programot.";
            }
        }
    }
}
