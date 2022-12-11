using System.Windows;
using System.Windows.Media;

namespace TestApp
{
    /// <summary>
    /// Логика взаимодействия для WindowConnectionDB.xaml
    /// </summary>
    public partial class ConnectionDBWindow : Window
    {
        Model.UserManager userManager;
        public ConnectionDBWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            userManager = (this.Owner as MainWindow).userManager;
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if(pbPassphrase.Password == pbСonfirm.Password)
            {
                var result = userManager.LoadDatabase(pbPassphrase.Password);
                if (result.state || userManager.isFirstExecute)
                {
                    DialogResult = true;
                }
                else
                {
                    tbAnswer.Foreground = Brushes.Red;
                    tbAnswer.Text = result.message;
                    if(result.message.Contains("Не удалось найти файл"))
                    {
                        spDelete.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                tbAnswer.Foreground = Brushes.Red;
                tbAnswer.Text = "Парольные фразы не совпадают";
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            userManager.DeleteDB();
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Application.Restart();
            DialogResult = false;
        }
    }
}
