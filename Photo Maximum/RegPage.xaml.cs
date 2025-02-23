using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Photo_Maximum
{
    public partial class RegPage : Page
    {
        private readonly DatabaseService _databaseService;
        private bool isPhoneValid = false;
        private bool isLoginValid = false;
        private bool isPasswordValid = false;

        public RegPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService("Server=95.31.128.97;Database=PhotoMaximum;User Id=admin;Password=winServer=;");
        }

        private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int spaceCount = NameBox.Text.Count(c => c == ' ');
            NameBox.BorderBrush = spaceCount < 2 ? Brushes.Red : Brushes.Green;
            NameBox.ToolTip = spaceCount < 2 ? "Введите Имя, Фамилию и Отчество через пробел" : null;
        }

        private void PhoneBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private void PhoneBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PhoneBox.Text.Length == 0)
            {
                PhoneBox.Text = "+7";
                PhoneBox.CaretIndex = PhoneBox.Text.Length;
            }
            else if (!PhoneBox.Text.StartsWith("+7"))
            {
                PhoneBox.Text = "+7";
                PhoneBox.CaretIndex = PhoneBox.Text.Length;
            }

            if (PhoneBox.Text.Length > 12)
            {
                PhoneBox.Text = PhoneBox.Text.Substring(0, 12);
                PhoneBox.CaretIndex = PhoneBox.Text.Length;
            }

            isPhoneValid = PhoneBox.Text.Length == 12;
            PhoneBox.BorderBrush = isPhoneValid ? Brushes.Green : Brushes.Red;
        }

        private void RegLoginBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                isLoginValid = _databaseService.IsLoginUnique(RegLoginBox.Text);
                RegLoginBox.BorderBrush = isLoginValid ? Brushes.Green : Brushes.Red;
                RegLoginBox.ToolTip = isLoginValid ? null : $"Логин {RegLoginBox.Text} уже используется другим пользователем";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            isPasswordValid = Regex.IsMatch(RegPasswordBox.Password, @"^(?=.*[A-Za-z])(?=.*\d).+$");
            RegPasswordBox.BorderBrush = isPasswordValid ? Brushes.Green : Brushes.Red;
            RegPasswordBox.ToolTip = isPasswordValid ? null : "Используйте латинские символы и цифры";
        }

        private void RegisterClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text) || string.IsNullOrWhiteSpace(PhoneBox.Text) ||
                string.IsNullOrWhiteSpace(RegLoginBox.Text) || string.IsNullOrWhiteSpace(RegPasswordBox.Password))
            {
                MessageBox.Show("Необходимо заполнить все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!isPhoneValid || !isLoginValid || !isPasswordValid)
            {
                MessageBox.Show("Проверьте правильность заполнения полей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int rowsAffected = _databaseService.RegisterUser(NameBox.Text, PhoneBox.Text, RegLoginBox.Text, RegPasswordBox.Password, 3);
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Пользователь успешно зарегистрирован!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService.Navigate(new AuthPage());
                }
                else
                {
                    MessageBox.Show("Ошибка регистрации. Попробуйте снова.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToAuthButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }
    }
}