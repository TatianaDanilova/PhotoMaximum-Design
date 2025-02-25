using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Photo_Maximum
{
    public partial class Operator : Page
    {
        private readonly DatabaseService _databaseService;
        private List<Order> _orders;
        private List<UserData> _masters;

        public Operator()
        {
            InitializeComponent();

            // Проверяем роль пользователя
            if (CurrentUser.role != "Оператор")
            {
                MessageBox.Show("Доступ запрещен. Эта страница доступна только для операторов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                NavigationService.Navigate(new Profile()); // Перенаправляем на профиль
                return;
            }

            _databaseService = new DatabaseService("Server=95.31.128.97;Database=PhotoMaximum;User Id=admin;Password=winServer=;");

            // Загружаем данные при создании страницы
            LoadData();
        }

        // Загрузка данных
        private void LoadData()
        {
            try
            {
                // Загружаем список заказов
                _orders = _databaseService.GetAllOrders();
                OrdersList.ItemsSource = _orders;

                // Загружаем список мастеров
                _masters = _databaseService.GetMasters();

                // Устанавливаем Masters как свойство в DataContext
                foreach (var order in _orders)
                {
                    order.Masters = _masters;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик кнопки "Назад"
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Profile());
        }


        // Обработчик нажатия на карточку заказа
        private void OrderCard_Click(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border == null) return;

            var order = border.DataContext as Order;
            if (order == null) return;

            // Открываем всплывающее окно с деталями заказа
            var detailsWindow = new OrderDetailsWindow(order);
            detailsWindow.ShowDialog();
        }

        // Обработчик кнопки "Назначить мастера"
        private void AssignMaster_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            // Получаем выбранный заказ
            var order = button.DataContext as Order;
            if (order == null) return;

            // Получаем выбранного мастера
            var selectedMaster = order.SelectedMaster;
            if (selectedMaster == null)
            {
                MessageBox.Show("Выберите мастера.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Назначаем мастера на заказ
                _databaseService.AssignMaster(order.RequestId, selectedMaster.UserId);

                // Обновляем список заказов
                LoadData();
                MessageBox.Show("Мастер успешно назначен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при назначении мастера: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}