using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Photo_Maximum
{
    /// <summary>
    /// Логика взаимодействия для Profile.xaml
    /// </summary>
    public partial class Profile : Window
    {
        string connectionString = "Server=95.31.128.97;Database=PhotoMaximum;User Id=admin;Password=winServer=;";
        public Profile()
        {
            InitializeComponent();
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

        private void ToRequests_Click(object sender, RoutedEventArgs e)
        {
            Client client = new Client();
            client.Show();
            this.Close();
        }
        private void EditProfileClick(object sender, RoutedEventArgs e)
        {
            EditProfile editProfile = new EditProfile();
            editProfile.Show();
            this.Close();
        }
        private void ToMasters_Click(object sender, RoutedEventArgs e)
        {
        }
        private void ToAutho_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
