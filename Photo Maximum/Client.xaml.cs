using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Photo_Maximum
{
    public partial class Client : Page
    {
        private readonly DatabaseService _databaseService;

        public Client()
        {
            InitializeComponent();

            // Проверяем роль пользователя
            if (CurrentUser.role != "Клиент")
            {
                MessageBox.Show("Доступ запрещен. Эта страница доступна только для клиентов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                NavigationService.Navigate(new Profile()); // Перенаправляем на профиль или другую страницу
                return;
            }

            _databaseService = new DatabaseService("Server=95.31.128.97;Database=PhotoMaximum;User Id=admin;Password=winServer=;");
            LoadClientRequests();
        }

        private void LoadClientRequests()
        {
            try
            {
                var requests = _databaseService.GetClientRequests(CurrentUser.userId);
                ClientRequestsList.ItemsSource = requests;

                // Проверяем, есть ли заказы
                if (requests == null || requests.Count == 0)
                {
                    NoOrdersText.Visibility = Visibility.Visible; // Показываем сообщение
                }
                else
                {
                    NoOrdersText.Visibility = Visibility.Collapsed; // Скрываем сообщение
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке заказов: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToProfile_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Profile());
        }

        private void ToAutho_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void NewRequestClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new NewRequestPage());
        }
    }
}