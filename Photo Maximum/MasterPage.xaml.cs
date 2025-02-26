using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Photo_Maximum
{
    public partial class MasterPage : Page
    {
        private readonly DatabaseService _databaseService;
        private List<Order> _orders;

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
                OrdersGrid.ItemsSource = _orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Подтверждение заказа
        private void ConfirmOrder_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrder = OrdersGrid.SelectedItem as Order;
            if (selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ для подтверждения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _databaseService.UpdateOrderStatus(selectedOrder.RequestId, "подтвержден");
                LoadData();
                MessageBox.Show("Заказ подтвержден.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подтверждении заказа: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Начало выполнения заказа
        private void StartOrder_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrder = OrdersGrid.SelectedItem as Order;
            if (selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ для начала выполнения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _databaseService.UpdateOrderStatus(selectedOrder.RequestId, "в процессе");
                _databaseService.UpdateOrderStartDate(selectedOrder.RequestId, DateTime.Now);
                LoadData();
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
            var selectedOrder = OrdersGrid.SelectedItem as Order;
            if (selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ для завершения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _databaseService.UpdateOrderStatus(selectedOrder.RequestId, "завершен");
                _databaseService.UpdateOrderEndDate(selectedOrder.RequestId, DateTime.Now);
                LoadData();
                MessageBox.Show("Заказ завершен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при завершении заказа: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Отказ от заказа
        private void RejectOrder_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrder = OrdersGrid.SelectedItem as Order;
            if (selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ для отказа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Уведомляем оператора
             //   _databaseService.NotifyOperator(selectedOrder.RequestId, CurrentUser.userId);

                // Убираем заказ из списка мастера
                _databaseService.UpdateOrderStatus(selectedOrder.RequestId, "отклонен");
                _databaseService.RemoveMasterFromOrder(selectedOrder.RequestId);

                LoadData();
                MessageBox.Show("Заказ отклонен. Оператор уведомлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при отказе от заказа: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}