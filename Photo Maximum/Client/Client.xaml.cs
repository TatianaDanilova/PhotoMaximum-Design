using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Photo_Maximum
{
    public partial class Client : Page
    {
        private readonly DatabaseService _databaseService;
        private List<Order> _allOrders; // Все заказы
        private ObservableCollection<Order> _filteredOrders; // Отфильтрованные заказы

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

            _databaseService = new DatabaseService("Server=95.31.128.97;Database=PhotoMaximum;User Id=admin;Password=winServer===;");

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
                // Загружаем все заказы
                _allOrders = _databaseService.GetAllOrders();
                _allOrders = _allOrders.Where(o => o.ClientId == CurrentUser.userId).ToList();

                // Применяем фильтр по умолчанию (Актуальные)
                ApplyFilter("Актуальные");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Применение фильтра
        private void ApplyFilter(string filter)
        {
            if (_allOrders == null) return;

            List<Order> filtered;

            // Фильтруем заказы в зависимости от выбранного фильтра
            if (filter == "Актуальные")
            {
                filtered = _allOrders.Where(o => o.Status != "Завершен").ToList();
            }
            else if (filter == "Завершенные")
            {
                filtered = _allOrders.Where(o => o.Status == "Завершен").ToList();
            }
            else
            {
                // По умолчанию показываем все заказы
                filtered = _allOrders.ToList();
            }

            // Обновляем отфильтрованный список
            _filteredOrders = new ObservableCollection<Order>(filtered);
            ClientRequestsList.ItemsSource = _filteredOrders;
        }

        // Обработчик изменения выбора в ComboBox
        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null) return;

            var selectedFilter = (comboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (selectedFilter != null)
            {
                ApplyFilter(selectedFilter);
            }
        }


        // Обработчик кнопки "Назад"
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Profile());
        }
        private void LeaveReview_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null || btn.Tag == null) return;

            int requestId = (int)btn.Tag;
            CurrentRequest.requestId = requestId;
            ReviewWindow reviewWindow = new ReviewWindow(requestId);
            var clientId = CurrentUser.userId; // Используем ID текущего пользователя

            // Проверяем, оставлял ли уже клиент отзыв на это блюдо
            bool alreadyReviewed = _databaseService.HasAlreadyReviewed(clientId, CurrentRequest.requestId);

            if (alreadyReviewed)
            {
                MessageBox.Show("Вы уже оставили отзыв на это блюдо.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;  // Прерываем выполнение, если отзыв уже существует
            }
            reviewWindow.ShowDialog();
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