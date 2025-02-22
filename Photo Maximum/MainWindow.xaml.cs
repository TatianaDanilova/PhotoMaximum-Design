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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void ToRegButton_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
            AuthGrid.Visibility = Visibility.Collapsed;
            RegGrid.Visibility = Visibility.Visible;
        }

        private void ToAuthButton_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
            RegGrid.Visibility = Visibility.Collapsed;
            AuthGrid.Visibility = Visibility.Visible;
        }

        private void ClearFields()
        {
            // Очистка полей авторизации
            UserLogin.Text = "";
            UserPass.Password = "";

            // Очистка полей регистрации
            NameBox.Text = "";
            PhoneBox.Text = "";
            RegLoginBox.Text = "";
            RegPasswordBox.Password = "";
        }

        private void EntryClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вход выполнен!");
        }

        private void RegisterClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Регистрация завершена!");
        }
    }
}