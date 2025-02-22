using ControlzEx.Theming;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Security;
using System.Web.UI.WebControls;
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
        string connectionString = "Server=95.31.128.97;Database=PhotoMaximum;User Id=admin;Password=winServer=;";
        private bool isPhoneValid = false;
        private bool isLoginValid = false;
        private bool isPasswordValid = false;
        public MainWindow()
        {

            InitializeComponent();
        }
        
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
                string query = "SELECT COUNT(*) FROM Users WHERE login = @login";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", RegLoginBox.Text);
                    int count = (int)command.ExecuteScalar();

                    if (count > 0)
                    {
                        RegLoginBox.BorderBrush = System.Windows.Media.Brushes.Red;
                        RegLoginBox.ToolTip = ($"Логин {RegLoginBox.Text} уже используется другим пользователем");
                        isLoginValid = false;
                    }
                    else
                    {
                        RegLoginBox.BorderBrush = System.Windows.Media.Brushes.Green;
                        isLoginValid = true;
                    }
                }
            }

        }

        private void RegPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string password = RegPasswordBox.Password;

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

        public int CheckForEntry()
        {
            if (UserLogin.Text == "" || UserPass.Password == "")
            {
                MessageBox.Show("Необходимо заполнить все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return -1;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Запрос для получения ID пользователя по логину и паролю
                    string query = "SELECT [user_id] FROM Users WHERE login = @login AND pass = @password";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@login", UserLogin.Text);
                        command.Parameters.AddWithValue("@password", UserPass.Password); // Пароль должен быть хеширован!

                        object result = command.ExecuteScalar(); // Выполняем запрос и получаем ID

                        if (result != null) // Если нашли пользователя
                        {
                            int userId = Convert.ToInt32(result);
                            return userId;
                        }
                        else
                        {
                            MessageBox.Show("Неверный логин или пароль.");
                            return -1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
                    return -1;
                }
            }
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
            int check = CheckForEntry();
            if (check != -1)
            {   
                CurrentUser.userId = check;
                Profile profile = new Profile();
                profile.Show();
                this.Close();
            }
                
        }

        private void RegisterClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneBox.Text) ||
                string.IsNullOrWhiteSpace(RegLoginBox.Text) ||
                string.IsNullOrWhiteSpace(RegPasswordBox.Password))
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

                    
                    // Получаем максимальный ID и прибавляем 1
                    int newUserId = 1;
                    string getMaxIdQuery = "SELECT ISNULL(MAX(user_id), 0) + 1 FROM Users";
                    using (SqlCommand cmd = new SqlCommand(getMaxIdQuery, connection))
                    {
                        newUserId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Добавляем пользователя в базу
                    string insertQuery = "INSERT INTO Users (user_id, fio, phone, login, pass, role_id) " +
                                         "VALUES (@ID, @FullName, @Phone, @Login, @Password, @Role)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@ID", newUserId);
                        cmd.Parameters.AddWithValue("@FullName", NameBox.Text);
                        cmd.Parameters.AddWithValue("@Phone", PhoneBox.Text);
                        cmd.Parameters.AddWithValue("@Login", RegLoginBox.Text);
                        cmd.Parameters.AddWithValue("@Password", RegPasswordBox.Password);
                        cmd.Parameters.AddWithValue("@Role", 3);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Пользователь успешно зарегистрирован!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                            // Переключаемся на форму авторизации
                            RegGrid.Visibility = Visibility.Collapsed;
                            AuthGrid.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            MessageBox.Show("Ошибка регистрации. Попробуйте снова.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


    }
    }
