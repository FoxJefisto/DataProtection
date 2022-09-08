using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TestApp
{
    /// <summary>
    /// Логика взаимодействия для LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        public LogInWindow()
        {
            InitializeComponent();
        }

        private void btnLogIn_Click(object sender, RoutedEventArgs e)
        {
            var userManager = (this.Owner as MainWindow).userManager;
            bool rootAccess = userManager.isFirstExecute;
            var result = userManager.LogIn(tbLogin.Text, pbPassword.Password);
            tbAnswer.Foreground = Brushes.Red;
            tbAnswer.Text = result.message;
            if (result.state)
            {
                DialogResult = true;
            }
        }
    }
}
