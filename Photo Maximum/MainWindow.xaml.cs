using System.Windows;

namespace Photo_Maximum
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Устанавливаем стартовую страницу (авторизация)
            MainFrame.Navigate(new AuthPage());
        }
    }
}