using System.Windows;
using System.Windows.Media;

namespace TestApp
{
    /// <summary>
    /// Логика взаимодействия для LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        int attempts;
        public LogInWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            attempts = 3;
        }

        private void btnLogIn_Click(object sender, RoutedEventArgs e)
        {
            var userManager = (this.Owner as MainWindow).userManager;
            var result = userManager.LogIn(tbLogin.Text, pbPassword.Password);
            if (result.state)
            {
                DialogResult = true;
            }
            else
            {
                tbAnswer.Foreground = Brushes.Red;
                tbAnswer.Text = result.message;
                if (result.message == "Был введен неверный пароль.")
                {
                    attempts--;
                    tbAnswer.Text += $"\nОсталось попыток: {attempts}";
                    if (attempts == 0)
                        DialogResult = false;
                }
            }
        }
    }
}
