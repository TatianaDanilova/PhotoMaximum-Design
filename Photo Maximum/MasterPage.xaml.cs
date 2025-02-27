using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Photo_Maximum
{
    public partial class MasterPage : Page
    {
        private readonly DatabaseService _databaseService;
        private List<Photo_Maximum.Order> _orders;

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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Подтверждение заказа
        private void ConfirmOrder_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            var order = button.DataContext as Order;
            if (order == null) return;

            try
            {
                // Обновляем статус заказа
                _databaseService.UpdateOrderStatus(order.RequestId, "подтвержден");

                // Обновляем статус в объекте Order
                order.Status = "подтвержден";

                MessageBox.Show("Заказ подтвержден.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подтверждении заказа: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Отказ от заказа
        private void RejectOrder_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            var order = button.DataContext as Order;
            if (order == null) return;

            try
            {
                // Уведомляем оператора
                _databaseService.AddNotification(order.RequestId, 1, "Мастер отказался от заказа.", CurrentUser.userId);

                // Убираем мастера из заказа
                _databaseService.UpdateOrderStatus(order.RequestId, "отклонен");
                _databaseService.RemoveMasterFromOrder(order.RequestId);

                // Обновляем данные
                LoadData();
                MessageBox.Show("Заказ отклонен. Оператор уведомлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при отказе от заказа: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Начало выполнения заказа
        private void StartOrder_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            var order = button.DataContext as Order;
            if (order == null) return;

            try
            {
                // Обновляем статус заказа
                _databaseService.UpdateOrderStatus(order.RequestId, "в процессе");

                // Устанавливаем дату начала выполнения
                _databaseService.UpdateOrderStartDate(order.RequestId, DateTime.Now);

                // Обновляем статус в объекте Order
                order.Status = "в процессе";

                MessageBox.Show("Заказ начат.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при начале выполнения заказа: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Завершение заказа
        private void CompleteOrder_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            var order = button.DataContext as Order;
            if (order == null) return;

            try
            {
                // Обновляем статус заказа
                _databaseService.UpdateOrderStatus(order.RequestId, "завершен");

                // Устанавливаем дату завершения
                _databaseService.UpdateOrderEndDate(order.RequestId, DateTime.Now);

                // Обновляем статус в объекте Order
                order.Status = "завершен";

                MessageBox.Show("Заказ завершен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при завершении заказа: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            public Visibility StartButtonVisibility => Status == "подтвержден" ? Visibility.Visible : Visibility.Collapsed;
            public Visibility CompleteButtonVisibility => Status == "в процессе" ? Visibility.Visible : Visibility.Collapsed;

            // Реализация INotifyPropertyChanged
            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}