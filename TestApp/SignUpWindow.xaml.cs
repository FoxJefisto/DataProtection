using System.Windows;
using System.Windows.Media;

namespace TestApp
{
    /// <summary>
    /// Логика взаимодействия для SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        Model.UserManager userManager;
        public SignUpWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            userManager = (this.Owner as MainWindow).userManager;
            if (userManager.isFirstExecute)
            {
                tbLogin.Text = "admin";
                tbLogin.IsReadOnly = true;
                tbkPassword.Visibility = Visibility.Collapsed;
                pbPassword.Visibility = Visibility.Collapsed;
            }
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            var result = userManager.SignUp(tbLogin.Text, pbPassword.Password, userManager.isFirstExecute, userManager.isFirstExecute);
            tbAnswer.Foreground = Brushes.Red;
            tbAnswer.Text = result.message;
            tbAnswer.ToolTip = "24.	Чередование цифр, знаков препинания и снова цифр.";
            if (result.state)
            {
                DialogResult = true;
            }     
        }

        
    }
}
