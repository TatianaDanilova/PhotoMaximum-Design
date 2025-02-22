using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Photo_Maximum
{
    /// <summary>
    /// Логика взаимодействия для EditProfile.xaml
    /// </summary>
    public partial class EditProfile : Window
    {
        private bool isPhoneValid = false;
        private bool isLoginValid = false;
        private bool isPasswordValid = false;
        public EditProfile()
        {
            InitializeComponent();
            NameBox.Text = CurrentUser.fio;
            PhoneBox.Text = CurrentUser.phone;
            RegLoginBox.Text = CurrentUser.login;
            RegPasswordBox.Text = CurrentUser.pass;
        }
        string connectionString = "Server=95.31.128.97;Database=PhotoMaximum;User Id=admin;Password=winServer=;";

        private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = NameBox.Text;

            // Считаем количество пробелов
            int spaceCount = text.Count(c => c == ' ');

            if (spaceCount < 2)
            {
                NameBox.BorderBrush = System.Windows.Media.Brushes.Red; // Подсветка ошибки
                NameBox.ToolTip = ($"Введите Имя, Фамилию и Отчество через пробел");
            }
            else
            {
                NameBox.BorderBrush = System.Windows.Media.Brushes.Green; // Если ОК
            }
        }
        private void PhoneBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }
        private void PhoneBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PhoneBox.Text.Length == 0)
            {
                PhoneBox.Text = "+7"; // Устанавливаем префикс, если поле пустое
                PhoneBox.CaretIndex = PhoneBox.Text.Length; // Устанавливаем курсор в конец
            }
            else
            {
                // Если пользователь пытается стереть +7, восстанавливаем его
                if (!PhoneBox.Text.StartsWith("+7"))
                {
                    PhoneBox.Text = "+7";
                    PhoneBox.CaretIndex = PhoneBox.Text.Length;
                }

                // Ограничиваем длину ввода до 12 символов
                if (PhoneBox.Text.Length > 12)
                {
                    PhoneBox.Text = PhoneBox.Text.Substring(0, 12);
                    PhoneBox.CaretIndex = PhoneBox.Text.Length;
                }
            }
            string phone = PhoneBox.Text;

            if (phone.Length < 12)
            {
                PhoneBox.BorderBrush = System.Windows.Media.Brushes.Red;
                isPhoneValid = false;
            }
            else
            {
                PhoneBox.BorderBrush = System.Windows.Media.Brushes.Green;
                isPhoneValid = true;
            }
        }

        private void RegLoginBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT user_id FROM Users WHERE login = @login";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", RegLoginBox.Text);
                    object result = command.ExecuteScalar(); // Получаем user_id, если он есть

                    if (result != null) // Логин найден в базе
                    {
                        int foundUserId = Convert.ToInt32(result);

                        if (foundUserId == CurrentUser.userId) // Это наш логин → зелёный
                        {
                            RegLoginBox.BorderBrush = System.Windows.Media.Brushes.Green;
                            RegLoginBox.ToolTip = null;
                            isLoginValid = true;
                        }
                        else // Логин занят другим пользователем → ошибка
                        {
                            RegLoginBox.BorderBrush = System.Windows.Media.Brushes.Red;
                            RegLoginBox.ToolTip = $"Логин {RegLoginBox.Text} уже используется другим пользователем";
                            isLoginValid = false;
                        }
                    }
                    else // Логина в базе нет → свободен
                    {
                        RegLoginBox.BorderBrush = System.Windows.Media.Brushes.Green;
                        RegLoginBox.ToolTip = null;
                        isLoginValid = true;
                    }
                }
            }
        
    }

        private void RegPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string password = RegPasswordBox.Text;

            if (Regex.IsMatch(password, @"^(?=.*[A-Za-z])(?=.*\d).+$"))
            {
                RegPasswordBox.BorderBrush = System.Windows.Media.Brushes.Green;
                isPasswordValid = true;
            }
            else
            {
                RegPasswordBox.BorderBrush = System.Windows.Media.Brushes.Red;
                RegPasswordBox.ToolTip = ($"Используйте латинские символы и цифры");
                isPasswordValid = false;
            }
        }

            private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneBox.Text) ||
                string.IsNullOrWhiteSpace(RegLoginBox.Text) ||
                string.IsNullOrWhiteSpace(RegPasswordBox.Text))
            {
                MessageBox.Show("Необходимо заполнить все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Прерываем выполнение метода
            }
            if (NameBox.BorderBrush == System.Windows.Media.Brushes.Red ||
                 PhoneBox.BorderBrush == System.Windows.Media.Brushes.Red ||
                RegLoginBox.BorderBrush == System.Windows.Media.Brushes.Red ||
                 RegPasswordBox.BorderBrush == System.Windows.Media.Brushes.Red)
            {
                MessageBox.Show("Проверьте правильность заполнения полей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Прерываем выполнение метода
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Запрос для обновления данных пользователя
                    string query = @"
                UPDATE Users 
                SET fio = @fio, 
                    phone = @phone, 
                    login = @login, 
                    pass = @password
                WHERE user_id = @userId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId",CurrentUser.userId);
                        command.Parameters.AddWithValue("@fio", NameBox.Text);
                        command.Parameters.AddWithValue("@phone", PhoneBox.Text);
                        command.Parameters.AddWithValue("@login", RegLoginBox.Text);
                        command.Parameters.AddWithValue("@password", RegPasswordBox.Text);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Данные успешно обновлены!");

                        }
                        else
                        {
                            MessageBox.Show("Ошибка: данные не обновлены.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при обновлении данных: " + ex.Message);
                }
            }
            Profile profile = new Profile();
            profile.Show();
            this.Close();
        }
    }
}
