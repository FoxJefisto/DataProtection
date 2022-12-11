using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Shapes;
using TestApp.Model;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

namespace Installer
{
    /// <summary>
    /// Логика взаимодействия для InstallWindow.xaml
    /// </summary>
    public partial class InstallWindow : Window
    {
        public InstallWindow()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnInstall_Click(object sender, RoutedEventArgs e)
        {
            string sourcePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\TestApp\bin\Debug\net6.0-windows"));
            try
            {
                Directory.CreateDirectory(tbPathFile.Text);
                CopyFilesRecursively(sourcePath, tbPathFile.Text);
                SHA512 sha = new SHA512Managed();
                if (tbSectionName.Text != "")
                {
                    Registry.SetValue($"HKEY_CURRENT_USER\\Software\\{tbSectionName.Text}", "Signature",
                                        sha.ComputeHash(SystemInfoCollector.GetSystemInfo()));
                    var finishWindow = new FinishWindow();
                    finishWindow.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Требуется указать имя раздела реестра",
                                          "Error",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            this.Owner.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbSectionName.Text = "Filippov";
            tbPathFile.Text = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Lab6Project";
        }

        private void btnPathFile_Click(object sender, RoutedEventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbPathFile.Text = fbd.SelectedPath + "\\Lab6Project";
            }
        }

        private static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }
    }
}
