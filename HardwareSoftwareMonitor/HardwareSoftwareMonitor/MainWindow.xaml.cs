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
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Windows.Threading;
using Microsoft.Win32;

namespace HardwareSoftwareMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class ProgramAdatok
    {
        public string programnev { get; set; }
        public string programverzio { get; set; }
        public ProgramAdatok(ManagementObject obj)
        {
            programnev = Convert.ToString(obj["Name"]);
            programverzio = Convert.ToString(obj["Version"]);
        }
    }
    public class DriverAdatok
    {
        public string driverfriendly { get; set; }
        public string driverhwid { get; set; }
        public string driverdevid { get; set; }
        public string drivergyarto { get; set; }
        public string driverhely { get; set; }
        public DriverAdatok(ManagementObject obj)
        {
            try
            {
                driverfriendly = Convert.ToString(obj["FriendlyName"]);
                driverhwid = Convert.ToString(obj["HardWareID"]);
                driverdevid = Convert.ToString(obj["DeviceID"]);
                drivergyarto = Convert.ToString(obj["Manufacturer"]);
                driverhely = Convert.ToString(obj["Location"]);
            }
            catch (Exception)
            {

            }
        }

    }
    public partial class MainWindow : Window
    {
        PerformanceCounter cpukihaszn = new PerformanceCounter("Processzorinformációk", "A processzor kihasználtsága (%)", "_Total");
        PerformanceCounter ramkihaszn = new PerformanceCounter("Memória", "Rendelkezésre álló memória (megabájt)");
        DispatcherTimer tick = new DispatcherTimer();
        public MainWindow()
        {
            tick.Interval = TimeSpan.FromSeconds(1);
            tick.Tick += tick_Tick;
            tick.Start();
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
            NetworkInterface[] halozatikapcsolatok = NetworkInterface.GetAllNetworkInterfaces();

            if (halozatikapcsolatok == null || halozatikapcsolatok.Length < 1)
            {
                Console.WriteLine("  Nem találtunk hálózati interface-eket.");
            }
            else
            {
                foreach (NetworkInterface adapter in halozatikapcsolatok)
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    halobox.AppendText(adapter.Description + "\r");
                    halobox.AppendText("    Interface típus: ");
                    halobox.AppendText(Convert.ToString(adapter.NetworkInterfaceType) + "\r");
                    halobox.AppendText("    Fizikai elhelyezkedés: ");
                    halobox.AppendText(Convert.ToString(adapter.GetPhysicalAddress().ToString()) + "\r");
                    halobox.AppendText("    Működési állapot: ");
                    halobox.AppendText(Convert.ToString(adapter.OperationalStatus) + "\r");
                }
            }
            ManagementObjectSearcher hangeszkozok = new ManagementObjectSearcher("select * from Win32_SoundDevice");

            foreach (ManagementObject obj in hangeszkozok.Get())
            {
                hangbox.AppendText("Név: " + obj["Name"] + "\r");
                hangbox.AppendText("    Eszköznév: " + obj["ProductName"] + "\r");
                hangbox.AppendText("    Eszköz ID: " + obj["DeviceID"] + "\r");
                hangbox.AppendText("    Energiamegtakarítást támogatja: " + obj["PowerManagementSupported"] + "\r");
                hangbox.AppendText("    Állapot: " + obj["Status"] + "\r");
                hangbox.AppendText("    Állapot infó: " + obj["StatusInfo"] + "\r");
            }
            ManagementObjectSearcher nyomtatok = new ManagementObjectSearcher("select * from Win32_Printer");

            foreach (ManagementObject obj in nyomtatok.Get())
            {
                nyomtatobox.AppendText("Név: " + obj["Name"] + "\r");
                nyomtatobox.AppendText("    Hálózati nyomtató: " + obj["Network"] + "\r");
                nyomtatobox.AppendText("    Alapértelmezett nyomtató: " + obj["Default"] + "\r");
                nyomtatobox.AppendText("    Eszköz ID: " + obj["DeviceID"] + "\r");
                nyomtatobox.AppendText("    Állapot: " + obj["Status"] + "\r");
            }
            ManagementObjectSearcher alaplap = new ManagementObjectSearcher("select * from Win32_MotherboardDevice");

            foreach (ManagementObject obj in alaplap.Get())
            {
                alapnev.Content = obj["Name"];
                alapeler.Content = obj["Availability"];
                alapbusz1.Content = obj["PrimaryBusType"];
                alapbusz2.Content = obj["SecondaryBusType"];
                alapleir.Content = obj["Caption"];
                alapid.Content = obj["DeviceID"];
                alapall.Content = obj["Status"];
                alapsysname.Content = obj["SystemName"];
            }
            ManagementObjectSearcher memoria = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");

            foreach (ManagementObject obj in memoria.Get())
            {
                memnev.Content = obj["Name"];
                memkap.Content = Convert.ToInt64(obj["Capacity"]) / 1073741824 + " GB";
                memtip.Content = obj["MemoryType"];
                memseb.Content = obj["Speed"];
                memform.Content = obj["FormFactor"];
            }
            ManagementObjectSearcher oprend = new ManagementObjectSearcher("select * from Win32_OperatingSystem");

            foreach (ManagementObject obj in oprend.Get())
            {
                oscaption.Content = obj["Caption"];
                osdir.Content = obj["WindowsDirectory"];
                osproducttype.Content = obj["ProductType"];
                osserial.Content = obj["SerialNumber"];
                ossys.Content = obj["SystemDirectory"];
                oscode.Content = obj["CountryCode"];
                ostime.Content = obj["CurrentTimeZone"];
                oslevel.Content = obj["EncryptionLevel"];
                ostype.Content = obj["OSType"];
                osver.Content = obj["Version"];
            }
            ManagementObjectSearcher programkereses = new ManagementObjectSearcher("select * from Win32_Product");
            foreach (ManagementObject obj in programkereses.Get())
            {
                ProgramGrid.Items.Add(new ProgramAdatok(obj));
            }
            telepitettdarab.Content = "Telepített alkalmazások száma: " + ProgramGrid.Items.Count;
            ManagementObjectSearcher driverkereses = new ManagementObjectSearcher("select * from Win32_PnPSignedDriver");
            foreach (ManagementObject obj in driverkereses.Get())
            {
                DriverGrid.Items.Add(new DriverAdatok(obj));
            }
            driverdarab.Content = "Telepített driverek száma: " + DriverGrid.Items.Count;
        }
        private void homeres_Click(object sender, RoutedEventArgs e)
        {
            Double homerseklet = 0;
            String instanceName = "";

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\WMI", "select * from MSAcpi_ThermalZoneTemperature");

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
        private void tick_Tick(object sender, EventArgs e)
        {
            procitext.Content = (int)cpukihaszn.NextValue() + " %";
            memszaz.Content = (int)ramkihaszn.NextValue() + " MB";

        }
    }
}
