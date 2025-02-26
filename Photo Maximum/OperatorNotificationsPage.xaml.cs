using System;
using System.Collections.Generic;
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

namespace Photo_Maximum
{
    /// <summary>
    /// Логика взаимодействия для OperatorNotificationsPage.xaml
    /// </summary>
    public partial class OperatorNotificationsPage : Page
    {
        private readonly DatabaseService _databaseService;

        public OperatorNotificationsPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService("Server=95.31.128.97;Database=PhotoMaximum;User Id=admin;Password=winServer=;");
            LoadNotifications();
        }

        private void LoadNotifications()
        {
            try
            {
                var notifications = _databaseService.GetNotificationsForUser(CurrentUser.userId);
                NotificationsGrid.ItemsSource = notifications;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке уведомлений: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Profile());
        }
    }
}
