using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

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

            // Загружаем данные при создании страницы
            LoadClientRequests();

            // Подписываемся на событие Unloaded
            this.Unloaded += Client_Unloaded;
        }

        // Обработчик события Unloaded
        private void Client_Unloaded(object sender, RoutedEventArgs e)
        {
            // Освобождаем ресурсы изображений
            ReleaseImageResources();
        }

        // Метод для освобождения ресурсов изображений
        private void ReleaseImageResources()
        {
            if (ClientRequestsList.ItemsSource is List<Order> orders)
            {
                foreach (var order in orders)
                {
                    if (order.PhotoSource is BitmapImage bitmapImage)
                    {
                        bitmapImage.StreamSource?.Close(); // Закрываем поток, если он есть
                        bitmapImage.UriSource = null; // Освобождаем UriSource
                    }
                }
            }

            // Очищаем ItemsSource
            ClientRequestsList.ItemsSource = null;
        }

        private void LoadClientRequests()
        {
            try
            {
                // Получаем заказы из базы данных
                var orders = _databaseService.GetRequestsInfo(CurrentUser.userId);

                // Преобразуем пути к фото в BitmapImage
                foreach (var order in orders)
                {
                    if (!string.IsNullOrEmpty(order.Photo))
                    {
                        var bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // Загружаем в память
                        bitmapImage.UriSource = new Uri(order.Photo);
                        bitmapImage.EndInit();
                        order.PhotoSource = bitmapImage; // Сохраняем BitmapImage в объекте Order
                    }
                }

                // Привязываем данные к ItemsControl
                ClientRequestsList.ItemsSource = orders;

                // Проверяем, есть ли заказы
                if (orders == null || orders.Count == 0)
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