using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Photo_Maximum
{
    /// <summary>
    /// Логика взаимодействия для MasterReviews.xaml
    /// </summary>
    public partial class MasterReviews : Page
    {
        public ObservableCollection<ReviewViewModel> Reviews { get; set; } = new ObservableCollection<ReviewViewModel>();

        int _masterId = CurrentUser.userId;

        public MasterReviews(int masterId)
        {
            InitializeComponent();
            _masterId = masterId;
            this.DataContext = this;

            LoadReviews();
        }

        private void LoadReviews()
        {
            Reviews.Clear();

            using (SqlConnection conn = new SqlConnection("Server=95.31.128.97;Database=PhotoMaximum;User Id=admin;Password=winServer===;"))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"
                SELECT 
                    R.review_text AS ReviewText,
                    R.review_date AS ReviewDate,
                    U.fio AS CustomerName, 
	                R.rating
                FROM Reviews R
                JOIN Requests Req ON R.request_id = Req.request_id
                JOIN Users U ON R.client_id = U.user_id
                WHERE Req.master_id = @MasterId
                ORDER BY R.review_date DESC", conn);

                cmd.Parameters.AddWithValue("@MasterId", _masterId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Reviews.Add(new ReviewViewModel
                        {
                            CustomerName = reader["CustomerName"].ToString(),
                            ReviewText = reader["ReviewText"].ToString(),
                            ReviewDate = Convert.ToDateTime(reader["ReviewDate"]),
                            rating = Convert.ToInt32(reader["rating"])
                        });
                    }
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
