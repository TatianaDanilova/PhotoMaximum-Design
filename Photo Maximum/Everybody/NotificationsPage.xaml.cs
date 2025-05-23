using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Photo_Maximum.MasterPage;

namespace Photo_Maximum
{
    /// <summary>
    /// Логика взаимодействия для OperatorNotificationsPage.xaml
    /// </summary>
    public partial class OperatorNotificationsPage : Page
    {
        private readonly DatabaseService _databaseService;
        private ObservableCollection<Notification> _notifications;

        public OperatorNotificationsPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService("Server=95.31.128.97;Database=PhotoMaximum;User Id=admin;Password=winServer===;");

            LoadNotifications();
        }

        private void LoadNotifications()
        {
            try
            {
                // Загружаем уведомления из базы данных
                var notifications = _databaseService.GetNotificationsForUser(CurrentUser.userId);

                // Инициализируем ObservableCollection
                _notifications = new ObservableCollection<Notification>(notifications);

                // Привязываем список уведомлений к ItemsControl
                NotificationsList.ItemsSource = _notifications;
                if (notifications == null || notifications.Count == 0)
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
                MessageBox.Show("Ошибка при загрузке уведомлений: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        // Обработчик кнопки "Назад"
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Profile());
        }

        // Обработчик кнопки "Скрыть"
        private void HideNotification_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            // Получаем уведомление, связанное с кнопкой
            var notification = button.DataContext as Notification;
            if (notification == null) return;

            try
            {
                // Помечаем уведомление как прочитанное в базе данных
                _databaseService.MarkNotificationAsRead(notification.NotificationId);

                // Удаляем уведомление из списка
                _notifications.Remove(notification);

                MessageBox.Show("Уведомление скрыто.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при скрытии уведомления: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
