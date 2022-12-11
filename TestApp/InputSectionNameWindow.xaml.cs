using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TestApp.Model;

namespace TestApp
{
    /// <summary>
    /// Логика взаимодействия для InputSectionNameWindow.xaml
    /// </summary>
    public partial class InputSectionNameWindow : Window
    {
        public InputSectionNameWindow()
        {
            InitializeComponent();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            var sha = new SHA512Managed();
            if(tbSectionName.Text == "")
            {
                tbAnswer.Text = "Необходимо ввести имя раздела реестра!";
            }
            else
            {
                var registryData =
(byte[])Registry.GetValue("HKEY_CURRENT_USER\\Software\\" + tbSectionName.Text, "Signature", 0);
                if (registryData == null || !sha.ComputeHash(SystemInfoCollector.GetSystemInfo()).SequenceEqual(registryData))
                {
                    tbAnswer.Text = "Подписи не совпадают";
                }
                else
                {
                    tbAnswer.Text = "Подписи совпадают";
                    DialogResult = true;
                }
            }
        }
    }
}
