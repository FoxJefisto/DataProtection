using Microsoft.Win32;
using System;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;
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
            userManager = new UserManager();
            var inputSectionWindow = new InputSectionNameWindow();
            if (!inputSectionWindow.ShowDialog().Value)
            {
                Application.Current.Shutdown();
            }
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
            var connectionDBWindow = new ConnectionDBWindow();
            connectionDBWindow.Owner = this;
            var answerConnect = connectionDBWindow.ShowDialog();
            if (!answerConnect.Value)
            {
                Application.Current.Shutdown();
            }
            else if (userManager.isFirstExecute)
            {
                var signUpWindow = new SignUpWindow();
                signUpWindow.Owner = this;
                var answerSignUp = signUpWindow.ShowDialog();
                if (!answerSignUp.Value)
                {
                    Application.Current.Shutdown();
                }
            }
            else
            {
                var logInWindow = new LogInWindow();
                logInWindow.Owner = this;
                var answerLogIn = logInWindow.ShowDialog();
                if (answerLogIn.Value)
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
            changePasswordWindow.ShowDialog();
        }

        private void btnManageUsers_Click(object sender, RoutedEventArgs e)
        {
            var manageUsersWindow = new ManageUsersWindow();
            manageUsersWindow.Owner = this;
            manageUsersWindow.ShowDialog();
        }

        private void HelpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.Owner = this;
            aboutWindow.ShowDialog();
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            userManager.CloseDB();
        }
    }
}
