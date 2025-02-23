using System;
using System.Windows;
using System.Windows.Controls;

namespace Photo_Maximum
{
    public partial class AuthPage : Page
    {
        // Объявляем переменную _databaseService один раз
        private readonly DatabaseService _databaseService;

        public AuthPage()
        {
            InitializeComponent();
            // Инициализируем _databaseService
            _databaseService = new DatabaseService("Server=95.31.128.97;Database=PhotoMaximum;User Id=admin;Password=winServer=;");
        }

        private void EntryClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // Используем _databaseService для проверки авторизации
                int userId = _databaseService.CheckForEntry(UserLogin.Text, UserPass.Password);
                if (userId != -1)
                {
                    CurrentUser.userId = userId;
                    NavigationService.Navigate(new Profile());
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToRegButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegPage());
        }
    }
}