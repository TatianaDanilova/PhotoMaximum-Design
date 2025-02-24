using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Photo_Maximum
{
    public partial class Profile : Page
    {
        private readonly DatabaseService _databaseService;

        public Profile()
        {
            string connectionString = "Server=95.31.128.97;Database=PhotoMaximum;User Id=admin;Password=winServer=;";
            InitializeComponent();
            _databaseService = new DatabaseService("Server=95.31.128.97;Database=PhotoMaximum;User Id=admin;Password=winServer=;");
            LoadUserData();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // Запрос на получение информации о пользователе
                    string query = "SELECT fio, phone, [login], pass, role_name FROM Users u inner join Roles r on u.role_id = r.role_id WHERE u.user_id = @userId;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", CurrentUser.userId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Если нашли пользователя
                            {
                                CurrentUser.fio = reader["fio"].ToString();
                                CurrentUser.phone = reader["phone"].ToString();
                                CurrentUser.login = reader["login"].ToString();
                                CurrentUser.pass = reader["pass"].ToString(); // Если хеширован, лучше не выводить!
                                CurrentUser.role = reader["role_name"].ToString();

                            }
                            else
                            {
                                MessageBox.Show("Пользователь не найден.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при получении данных: " + ex.Message);
                }
            }
            NameBox.Text = CurrentUser.fio;
            PhoneBox.Text = CurrentUser.phone;
            RegLoginBox.Text = CurrentUser.login;
            RegPasswordBox.Text = CurrentUser.pass;
            if (CurrentUser.role != "Клиент")
            {
                RoleBox.Text = CurrentUser.role;
            }
            else
            {
                RoleBlock.Visibility = Visibility.Collapsed;
                RoleBox.Visibility = Visibility.Collapsed;
            }
            if (CurrentUser.role == "Оператор")
            {
                ToMasters.Visibility = Visibility.Visible;
            }

        
    }

        private void LoadUserData()
        {
            try
            {
                // Получаем данные пользователя
                var userData = _databaseService.GetUserData(CurrentUser.userId);

                // Заполняем поля на странице
                NameBox.Text = userData.Fio;
                PhoneBox.Text = userData.Phone;
                RegLoginBox.Text = userData.Login;
                RegPasswordBox.Text = userData.Password;

                // Устанавливаем роль
                if (userData.Role != "Клиент")
                {
                    RoleBox.Text = userData.Role;
                }
                else
                {
                    RoleBlock.Visibility = Visibility.Collapsed;
                    RoleBox.Visibility = Visibility.Collapsed;
                }

                // Показываем кнопку для оператора
                if (userData.Role == "Оператор")
                {
                    ToMasters.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToRequests_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.role == "Клиент")
            {
                NavigationService.Navigate(new Client());
            }
            else if (CurrentUser.role == "Оператор")
            {
                NavigationService.Navigate(new Operator());
            }
            else
            {
                MessageBox.Show("Доступ запрещен. Эта страница недоступна.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void ToMasters_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.role == "Оператор")
            {
                NavigationService.Navigate(new MastersPage());
            }
            else
            {
                MessageBox.Show("Доступ запрещен. Эта страница недоступна.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        

        private void EditProfileClick(object sender, RoutedEventArgs e)
        {
            // Проверяем, что CurrentUser не пустой
            if (string.IsNullOrEmpty(CurrentUser.fio) || string.IsNullOrEmpty(CurrentUser.phone) ||
                string.IsNullOrEmpty(CurrentUser.login) || string.IsNullOrEmpty(CurrentUser.pass))
            {
                MessageBox.Show("Данные пользователя не загружены&&&&&&&&&&&.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Переходим на страницу редактирования профиля
            NavigationService.Navigate(new EditProfile());
        }

        

        private void ToAutho_Click(object sender, RoutedEventArgs e)
        {
            // Возврат на страницу авторизации
            NavigationService.Navigate(new AuthPage());
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            // Закрытие приложения
            Application.Current.Shutdown();
        }
    }
}