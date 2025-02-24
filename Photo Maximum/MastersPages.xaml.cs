using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Photo_Maximum
{
    public partial class MastersPage : Page
    {
        private readonly DatabaseService _databaseService;

        public MastersPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService("Server=95.31.128.97;Database=PhotoMaximum;User Id=admin;Password=winServer=;");
            LoadMasters(); // Загружаем данные при инициализации страницы
        }

        private void LoadMasters()
        {
            try
            {
                var masters = _databaseService.GetMasters();
                MastersGrid.ItemsSource = masters; // Устанавливаем источник данных для DataGrid
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке мастеров: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик для TextBox: GotFocus
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // Скрываем TextBlock с подсказкой
                var placeholder = FindPlaceholderTextBlock(textBox);
                if (placeholder != null)
                {
                    placeholder.Visibility = Visibility.Collapsed;
                }
            }
        }

        // Обработчик для TextBox: LostFocus
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // Показываем TextBlock с подсказкой, если TextBox пуст
                var placeholder = FindPlaceholderTextBlock(textBox);
                if (placeholder != null && string.IsNullOrEmpty(textBox.Text))
                {
                    placeholder.Visibility = Visibility.Visible;
                }
            }
        }

        // Обработчик для TextBox: TextChanged
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // Управляем видимостью TextBlock с подсказкой
                var placeholder = FindPlaceholderTextBlock(textBox);
                if (placeholder != null)
                {
                    placeholder.Visibility = string.IsNullOrEmpty(textBox.Text) ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        // Обработчик для PasswordBox: GotFocus
        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                // Скрываем TextBlock с подсказкой
                PasswordPlaceholder.Visibility = Visibility.Collapsed;
            }
        }

        // Обработчик для PasswordBox: LostFocus
        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                // Показываем TextBlock с подсказкой, если PasswordBox пуст
                if (string.IsNullOrEmpty(passwordBox.Password))
                {
                    PasswordPlaceholder.Visibility = Visibility.Visible;
                }
            }
        }

        // Обработчик для PasswordBox: PasswordChanged
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                // Управляем видимостью TextBlock с подсказкой
                PasswordPlaceholder.Visibility = string.IsNullOrEmpty(passwordBox.Password) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        // Поиск TextBlock с подсказкой для TextBox
        private TextBlock FindPlaceholderTextBlock(TextBox textBox)
        {
            var parent = textBox.Parent as FrameworkElement;
            if (parent != null)
            {
                return parent.FindName(textBox.Name + "Placeholder") as TextBlock;
            }
            return null;
        }

        // Обработчик для кнопки "Добавить мастера"
        private void AddMaster_Click(object sender, RoutedEventArgs e)
        {
            // Ваш код для добавления мастера
        }

        // Обработчик для кнопки "Назад"
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        // Обработчик для валидации телефона
        private void PhoneBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }
    }
}