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
using TestApp.Model;

namespace TestApp
{
    /// <summary>
    /// Логика взаимодействия для ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        UserManager userManager;
        public ChangePasswordWindow()
        {
            InitializeComponent();
        }

        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var result = userManager.ChangeCurrentUserPassword(pbOldPassword.Password, pbNewPassword.Password);
            tbAnswer.Foreground = Brushes.Red;
            tbAnswer.Text = result.message;
            tbAnswer.ToolTip = "Чередование цифр, знаков препинания и снова цифр.";
            if (result.state)
            {
                DialogResult = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            userManager = (this.Owner as MainWindow).userManager;
            if (userManager.CurrentUser.HasConstraint)
            {
                tbConstraint.Text = "Чередование цифр, знаков препинания и снова цифр.";
                
            }
            else
            {
                tbConstraint.Text = "Отсутствуют";
            }
        }
    }
}
