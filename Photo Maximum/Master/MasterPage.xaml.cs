using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using static Photo_Maximum.MasterPage;
using System.Windows.Media.Imaging;
using System.Linq;

namespace Photo_Maximum
{
    public partial class MasterPage : Page
    {
        private readonly DatabaseService _databaseService;
        private List<Photo_Maximum.Order> _orders;
        private ObservableCollection<Photo_Maximum.Order> _filteredOrders; // Отфильтрованные заказы

        public MasterPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService("Server=95.31.128.97;Database=PhotoMaximum;User Id=admin;Password=winServer=;");
            LoadData();

        }

        // Загрузка данных
        private void LoadData()
        {
            
            try
            {
                // Загружаем заказы, назначенные текущему мастеру
                _orders = _databaseService.GetOrdersByMaster(CurrentUser.userId);
                OrdersList.ItemsSource = _orders;

                // Преобразуем пути к фото в BitmapImage
                foreach (var order in _orders)
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
                ApplyFilter("Актуальные");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ApplyFilter(string filter)
        {
            if (_orders == null) return;

            List<Photo_Maximum.Order> filtered;

            // Фильтруем заказы в зависимости от выбранного фильтра
            switch (filter)
            {
                case "Актуальные":
                    filtered = _orders.Where(o => o.Status == "Ждет подтверждения" || o.Status == "Подтвержден" || o.Status == "В процессе").ToList();
                    break;
                case "Завершенные":
                    filtered = _orders.Where(o => o.Status == "Завершен").ToList();
                    break;
                default:
                    // По умолчанию показываем все заказы
                    filtered = _orders.ToList();
                    break;
            }

            // Обновляем отфильтрованный список
            _filteredOrders = new ObservableCollection<Photo_Maximum.Order>(filtered);
            OrdersList.ItemsSource = _filteredOrders;
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
        public ObservableCollection<Order> Orders { get; set; } = new ObservableCollection<Order>();
        // Подтверждение заказа
        private void ConfirmOrder_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var order = button.DataContext as Photo_Maximum.Order;

            if (order == null)
            {
                Debug.WriteLine("DataContext кнопки: " + (button.DataContext?.ToString() ?? "null"));
                MessageBox.Show("Ошибка: DataContext кнопки не содержит объект Order.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Обновляем статус заказа в базе данных
                _databaseService.UpdateOrderStatus(order.RequestId, "Подтвержден");

                // Обновляем статус в объекте Order
                order.Status = "Подтвержден";

                // Принудительно обновляем интерфейс
                OrdersList.Items.Refresh();

                MessageBox.Show("Заказ подтвержден.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                _databaseService.AddNotification(order.RequestId, order.ClientId, $"Мастер {CurrentUser.fio} подтвердил заказ №{order.RequestId}.\n Скоро начнет его выполнение.", CurrentUser.userId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подтверждении заказа: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            LoadData();
        }

        // Отказ от заказа
        private void RejectOrder_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            var order = button.DataContext as Photo_Maximum.Order;
            if (order == null) return;

            try
            {
                // Уведомляем оператора
                _databaseService.AddNotification(order.RequestId, 1, $"Мастер {CurrentUser.fio} отказался от заказа №{order.RequestId}.", CurrentUser.userId);
                _databaseService.AddNotification(order.RequestId, order.ClientId, $"Мастер {CurrentUser.fio} отказался от заказа №{order.RequestId}.\nОператор скоро назначит нового исполнителя.", CurrentUser.userId);

                // Убираем мастера из заказа
                _databaseService.UpdateOrderStatus(order.RequestId, "Отклонен");
                _databaseService.RemoveMasterFromOrder(order.RequestId);

                // Обновляем данные
                LoadData();
                MessageBox.Show("Заказ отклонен. Оператор уведомлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при отказе от заказа: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            LoadData();
        }

        // Начало выполнения заказа
        private void StartOrder_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            var order = button.DataContext as Photo_Maximum.Order;
            if (order == null) return;

            try
            {
                // Обновляем статус заказа
                _databaseService.UpdateOrderStatus(order.RequestId, "В процессе");

                // Устанавливаем дату начала выполнения
                _databaseService.UpdateOrderStartDate(order.RequestId, DateTime.Now);

                // Обновляем статус в объекте Order
                order.Status = "В процессе";

                MessageBox.Show("Заказ начат.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                _databaseService.AddNotification(order.RequestId, order.ClientId, $"Мастер начал выполнять заказ №{order.RequestId}.", CurrentUser.userId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при начале выполнения заказа: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            LoadData();
        }

        // Завершение заказа
        private void CompleteOrder_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            var order = button.DataContext as Photo_Maximum.Order;
            if (order == null) return;

            try
            {
                // Обновляем статус заказа
                _databaseService.UpdateOrderStatus(order.RequestId, "Завершен");

                // Устанавливаем дату завершения
                _databaseService.UpdateOrderEndDate(order.RequestId, DateTime.Now);

                // Обновляем статус в объекте Order
                order.Status = "Завершен";

                MessageBox.Show("Заказ завершен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                _databaseService.AddNotification(order.RequestId, order.ClientId, $"Мастер завершил заказ №{order.RequestId}.", CurrentUser.userId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при завершении заказа: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            LoadData();
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        public class Order : INotifyPropertyChanged
        {
            private string _status;

            public int RequestId { get; set; }
            public string TypeName { get; set; }
            public string Size { get; set; }
            public string Status
            {
                get => _status;
                set
                {
                    if (_status != value)
                    {
                        _status = value;
                        OnPropertyChanged(nameof(Status));
                        OnPropertyChanged(nameof(ConfirmButtonVisibility));
                        OnPropertyChanged(nameof(RejectButtonVisibility));
                        OnPropertyChanged(nameof(StartButtonVisibility));
                        OnPropertyChanged(nameof(CompleteButtonVisibility));
                    }
                }
            }
            public string ClientName { get; set; }
            public DateTime? DateStart { get; set; }
            public DateTime? DateEnd { get; set; }

            // Свойства для управления видимостью кнопок
            public Visibility ConfirmButtonVisibility => Status == "Ждет подтверждения" ? Visibility.Visible : Visibility.Collapsed;
            public Visibility RejectButtonVisibility => Status == "Ждет подтверждения" ? Visibility.Visible : Visibility.Collapsed;
            public Visibility StartButtonVisibility => Status == "Подтвержден" ? Visibility.Visible : Visibility.Collapsed;
            public Visibility CompleteButtonVisibility => Status == "В процессе" ? Visibility.Visible : Visibility.Collapsed;

            // Реализация INotifyPropertyChanged
            public event PropertyChangedEventHandler PropertyChanged; 
            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}