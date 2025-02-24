using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Photo_Maximum
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Метод для проверки авторизации
        public int CheckForEntry(string login, string password)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT [user_id] FROM Users WHERE login = @login AND pass = @password";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@password", password);

                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            return Convert.ToInt32(result);
                        }
                        else
                        {
                            return -1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Ошибка подключения к базе данных: " + ex.Message);
                }
            }
        }

        // Метод для проверки уникальности логина
        public bool IsLoginUnique(string login)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE login = @login";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", login);
                    return (int)command.ExecuteScalar() == 0;
                }
            }
        }

        // Метод для регистрации нового пользователя
        public int RegisterUser(string fullName, string phone, string login, string password, int roleId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    int newUserId = GetNextUserId(connection);

                    string insertQuery = "INSERT INTO Users (user_id, fio, phone, login, pass, role_id) " +
                                        "VALUES (@ID, @FullName, @Phone, @Login, @Password, @Role)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@ID", newUserId);
                        cmd.Parameters.AddWithValue("@FullName", fullName);
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.Parameters.AddWithValue("@Login", login);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@Role", roleId);

                        return cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Ошибка регистрации: " + ex.Message);
                }
            }
        }

        // Метод для получения следующего ID пользователя
        private int GetNextUserId(SqlConnection connection)
        {
            string getMaxIdQuery = "SELECT ISNULL(MAX(user_id), 0) + 1 FROM Users";
            using (SqlCommand cmd = new SqlCommand(getMaxIdQuery, connection))
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        // Метод для получения данных пользователя
        public UserData GetUserData(int userId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT fio, phone, [login], pass, role_name FROM Users u " +
                               "INNER JOIN Roles r ON u.role_id = r.role_id " +
                               "WHERE u.user_id = @userId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserData
                            {
                                Fio = reader["fio"].ToString(),
                                Phone = reader["phone"].ToString(),
                                Login = reader["login"].ToString(),
                                Password = reader["pass"].ToString(),
                                Role = reader["role_name"].ToString()
                            };
                        }
                        else
                        {
                            throw new Exception("Пользователь не найден.");
                        }
                    }
                }
            }
        }
        // Метод для проверки уникальности логина
        public bool IsLoginUnique(string login, int currentUserId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT user_id FROM Users WHERE login = @login";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", login);
                    object result = command.ExecuteScalar();

                    if (result == null) // Логин свободен
                    {
                        return true;
                    }
                    else // Логин занят
                    {
                        int foundUserId = Convert.ToInt32(result);
                        return foundUserId == currentUserId; // Если это текущий пользователь, то логин валиден
                    }
                }
            }
        }

        // Метод одновления данных о пользователе
        public void UpdateUser(int userId, string fio, string phone, string login, string password)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
            UPDATE Users 
            SET fio = @fio, 
                phone = @phone, 
                login = @login, 
                pass = @password
            WHERE user_id = @userId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@fio", fio);
                    command.Parameters.AddWithValue("@phone", phone);
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@password", password);

                    command.ExecuteNonQuery();
                }
            }
        }
        // Метод для вывода списка заказов клиента
        public List<ClientRequest> GetClientRequests(int clientId)
        {
            var requests = new List<ClientRequest>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT request_id, status FROM Requests WHERE client_id = @clientId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@clientId", clientId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            requests.Add(new ClientRequest
                            {
                                RequestId = Convert.ToInt32(reader["request_id"]),
                                Status = reader["status"].ToString()
                            });
                        }
                    }
                }
            }

            return requests;
        }
        // Метод для создания заказа
        // Метод для создания заказа
        public void CreateRequest(int clientId, string itemType, string itemSize, string photoPath, string comment, int price)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Получаем type_id по названию типа предмета
                int typeId = GetTypeIdByName(itemType, connection);

                if (typeId == -1)
                {
                    throw new Exception("Тип предмета не найден в базе данных.");
                }

                // Находим максимальный request_id
                int newRequestId = GetMaxRequestId(connection) + 1;

                // SQL-запрос для добавления заказа
                var query = @"
            INSERT INTO Requests (request_id, type_id, client_id, size, photo, price, comment, status, date_start)
            VALUES (@RequestId, @TypeId, @ClientId, @Size, @Photo, @Price, @Comment, 'Новый', GETDATE())";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RequestId", newRequestId);
                    command.Parameters.AddWithValue("@TypeId", typeId);
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    command.Parameters.AddWithValue("@Size", itemSize);
                    command.Parameters.AddWithValue("@Photo", photoPath);
                    command.Parameters.AddWithValue("@Price", price);
                    command.Parameters.AddWithValue("@Comment", comment ?? (object)DBNull.Value); // Если комментарий null, записываем DBNull
                    command.ExecuteNonQuery();
                }
            }
        }

        // Метод для поиска максимального request_id
        private int GetMaxRequestId(SqlConnection connection)
        {
            var query = "SELECT MAX(request_id) FROM Requests";
            using (var command = new SqlCommand(query, connection))
            {
                var result = command.ExecuteScalar();
                return result == DBNull.Value ? 0 : Convert.ToInt32(result); // Если таблица пуста, возвращаем 0
            }
        }
        // Метод для получения type_id по названию типа предмета
        private int GetTypeIdByName(string typeName, SqlConnection connection)
        {
            var query = "SELECT type_id FROM Types WHERE type_name = @TypeName";
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TypeName", typeName);
                var result = command.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
        }
        public List<Order> GetRequestsInfo(int userId)
        {
            var orders = new List<Order>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = @"
            SELECT 
                r.request_id AS RequestId,
                r.status AS Status,
                t.type_name AS Type,
                r.size AS Size,
                r.photo AS Photo,
                r.comment AS Comment,
                r.date_start AS StartDate,
                r.date_end AS EndDate,
                u_master.fio AS Executor, -- ФИО мастера
                u_client.fio AS Customer -- ФИО клиента
            FROM Requests r
            JOIN Types t ON r.type_id = t.type_id
            JOIN Users u_client ON r.client_id = u_client.user_id
            LEFT JOIN Users u_master ON r.master_id = u_master.user_id
            WHERE r.client_id = @UserId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var order = new Order
                            {
                                RequestId = reader.GetInt32(0),
                                Status = reader.GetString(1),
                                Type = reader.GetString(2),
                                Size = reader.GetString(3),
                                Photo = reader.GetString(4),
                                Comment = reader.IsDBNull(5) ? null : reader.GetString(5),
                                StartDate = reader.GetDateTime(6),
                                EndDate = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7),
                                Executor = reader.IsDBNull(8) ? null : reader.GetString(8),
                                Customer = reader.GetString(9)
                            };
                            orders.Add(order);
                        }
                    }
                }
            }

            return orders;
        }
    
    public List<Order> GetAllOrders()
    {
        var orders = new List<Order>();

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var query = @"
            SELECT 
                r.request_id AS RequestId,
                r.status AS Status,
                t.type_name AS Type,
                r.size AS Size,
                r.photo AS Photo,
                r.comment AS Comment,
                r.date_start AS StartDate,
                r.date_end AS EndDate,
                u_master.fio AS Executor, -- ФИО мастера
                u_client.fio AS Customer -- ФИО клиента
            FROM Requests r
            JOIN Types t ON r.type_id = t.type_id
            JOIN Users u_client ON r.client_id = u_client.user_id
            LEFT JOIN Users u_master ON r.master_id = u_master.user_id";

            using (var command = new SqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var order = new Order
                        {
                            RequestId = reader.GetInt32(0),
                            Status = reader.GetString(1),
                            Type = reader.GetString(2),
                            Size = reader.GetString(3),
                            Photo = reader.GetString(4),
                            Comment = reader.IsDBNull(5) ? null : reader.GetString(5),
                            StartDate = reader.GetDateTime(6),
                            EndDate = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7),
                            Executor = reader.IsDBNull(8) ? null : reader.GetString(8),
                            Customer = reader.GetString(9)
                        };
                        orders.Add(order);
                    }
                }
            }
        }

        return orders;
    }

    public List<UserData> GetMasters()
    {
        var masters = new List<UserData>();

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var query = "SELECT user_id, fio, phone, login, pass FROM Users WHERE role_id = 2";

            using (var command = new SqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var master = new UserData
                        {
                            UserId = reader.GetInt32(0),
                            Fio = reader.GetString(1),
                            Phone = reader.GetString(2),
                            Login = reader.GetString(3),
                            Password = reader.GetString(4)
                        };
                        masters.Add(master);
                    }
                }
            }
        }

        return masters;
    }

    public void AssignMaster(int requestId, int masterId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var query = "UPDATE Requests SET master_id = @MasterId WHERE request_id = @RequestId";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@MasterId", masterId);
                command.Parameters.AddWithValue("@RequestId", requestId);
                command.ExecuteNonQuery();
            }
        }
    }
}
    // Класс для хранения данных пользователя
    public class UserData
    {
        public int UserId { get; set; }
        public string Fio { get; set; }
        public string Phone { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
   
    }
    public class ClientRequest
    {
        public int RequestId { get; set; }
        public string Status { get; set; }
    }
    public class Order
    {
        public int RequestId { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Photo { get; set; }
        public BitmapImage PhotoSource { get; set; } // BitmapImage для отображения
        public string Comment { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Executor { get; set; }
        public string Customer { get; set; }
        public List<UserData> Masters { get; set; }
        public UserData SelectedMaster { get; set; }
    }

}