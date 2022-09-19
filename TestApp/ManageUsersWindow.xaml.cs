using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для ManageUsersWindow.xaml
    /// </summary>
    public partial class ManageUsersWindow : Window
    {
        UserManager userManager;
        public ManageUsersWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            userManager = (this.Owner as MainWindow).userManager;
            lvUsers.ItemsSource = userManager.GetAllUsers();
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            var result = userManager.SignUp(tbLogin.Text, "", false, true);
            if (result.state)
            {
                tbAnswer.Foreground = Brushes.Green;
            }
            else
            {
                tbAnswer.Foreground = Brushes.Red;
            }
            tbAnswer.Text = result.message;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            userManager.SaveChanges();
        }

        private void btnRemoveUser_Click(object sender, RoutedEventArgs e)
        {
            var result = userManager.RemoveUser(tbLogin.Text);
            if (result.state)
            {
                tbAnswer.Foreground = Brushes.Green;
            }
            else
            {
                tbAnswer.Foreground = Brushes.Red;
            }
            tbAnswer.Text = result.message;
        }

        private void lvUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if((sender as ListView).SelectedItem is User)
            {
                tbLogin.Text = ((sender as ListView).SelectedItem as User).Login;
            }
        }

        private void cbIsBanned_Checked(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            var user = cb.DataContext as User;
            if (user.RootAccess)
            {
                cb.IsChecked = false;
            }
        }
    }
}
