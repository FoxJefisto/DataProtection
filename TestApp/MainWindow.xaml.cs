using System;
using System.Collections.Generic;
using System.IO;
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
using TestApp.Model;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public UserManager userManager;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SetControlsToLoginState()
        {
            iNotUser.Visibility = Visibility.Collapsed;
            tbUserName.Text = userManager.CurrentUser.Login;
            iUser.Visibility = Visibility.Visible;
            if (userManager.CurrentUser.RootAccess)
            {
                btnManageUsers.Visibility = Visibility.Visible;
            }
        }

        private void SetControlsToLogoutState()
        {
            btnManageUsers.Visibility = Visibility.Collapsed;
            iUser.Visibility = Visibility.Collapsed;
            iNotUser.Visibility = Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string path = "users.json";
            userManager = new UserManager(path);
            if (userManager.isFirstExecute)
            {
                var signUpWindow = new SignUpWindow();
                signUpWindow.Owner = this;
                var answerDialog = signUpWindow.ShowDialog();
                if (!answerDialog.Value)
                {
                    Application.Current.Shutdown();
                }
            }
            else
            {
                var logInWindow = new LogInWindow();
                logInWindow.Owner = this;
                var answerDialog = logInWindow.ShowDialog();
                if (answerDialog.Value)
                {
                    SetControlsToLoginState();
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
        }

        private void btnLogIn_Click(object sender, RoutedEventArgs e)
        {
            var logInWindow = new LogInWindow();
            logInWindow.Owner = this;
            logInWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            var answerDialog = logInWindow.ShowDialog();
            if (answerDialog.Value)
            {
                SetControlsToLoginState();
            }
        }

        private void btnLogOut_click(object sender, RoutedEventArgs e)
        {
            if (userManager.CurrentUser.Password == "")
            {
                MessageBox.Show(messageBoxText:"Вы забыли установить пароль.");
                btnChangePassword_Click(sender, null);
            }
            else
            {
                userManager.LogOut();
                SetControlsToLogoutState();
            }
        }

        //private void btnSignUp_Click(object sender, RoutedEventArgs e)
        //{
        //    var signUpWindow = new SignUpWindow();
        //    signUpWindow.Owner = this;
        //    signUpWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        //    var answerDialog = signUpWindow.ShowDialog();
        //}

        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var changePasswordWindow = new ChangePasswordWindow();
            changePasswordWindow.Owner = this;
            changePasswordWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            changePasswordWindow.ShowDialog();
        }

        private void btnManageUsers_Click(object sender, RoutedEventArgs e)
        {
            var manageUsersWindow = new ManageUsersWindow();
            manageUsersWindow.Owner = this;
            manageUsersWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            manageUsersWindow.ShowDialog();
        }

        private void HelpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.Owner = this;
            aboutWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            aboutWindow.ShowDialog();
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
