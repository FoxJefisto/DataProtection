using System;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TestApp.Model
{
    public static class SystemInfoCollector
    {
        public static byte[] GetSystemInfo()
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms))
                {
                    sw.WriteLine(Environment.UserName);
                    sw.WriteLine(Environment.MachineName);
                    sw.WriteLine(Environment.GetEnvironmentVariable("windir"));
                    sw.WriteLine(Environment.SystemDirectory);
                    sw.WriteLine(GetKeyboardType(0));
                    sw.WriteLine(GetKeyboardType(1));
                    sw.WriteLine(SystemInformation.PrimaryMonitorSize.Height);
                    var objectSearcher = new ManagementObjectSearcher("Select * from Win32_LogicalDisk");
                    var objectCollection = objectSearcher.Get();
                    foreach (var mo in objectCollection)
                    {
                        sw.WriteLine($"{mo["Name"]} {mo["VolumeSerialNumber"]}");
                    }
                }
                return ms.ToArray();
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        private static extern int
            GetKeyboardType(int nTypeFlag); /* получение типа (nTypeFlag=0) или подтипа (nTypeFlag=1) клавиатуры */
    }
}
