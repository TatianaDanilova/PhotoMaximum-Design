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
using System.Windows.Shapes;

namespace Photo_Maximum
{
    /// <summary>
    /// Логика взаимодействия для ReviewWindow.xaml
    /// </summary>
    public partial class ReviewWindow : Window
    {
        private int _requestId;
        private readonly DatabaseService _databaseService;

        public ReviewWindow(int requestId)
        {
            InitializeComponent();
            _requestId = requestId;
            _databaseService = new DatabaseService("Server=95.31.128.97;Database=PhotoMaximum;User Id=admin;Password=winServer===;");
        }

        // Обработчик для кнопки "Отправить отзыв"
        private void SubmitReviewButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, является ли введенное значение числом
            if (!int.TryParse(RatingBox.Text, out int rating))
            {
                MessageBox.Show("Введите корректную числовую оценку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверяем диапазон рейтинга (от 1 до 5)
            if (rating < 1 || rating > 5)
            {
                MessageBox.Show("Оценка должна быть от 1 до 5.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string reviewText = ReviewBox.Text.Trim();
            if (string.IsNullOrEmpty(reviewText))
            {
                MessageBox.Show("Пожалуйста, напишите отзыв.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            
            

            // Добавляем отзыв в базу данных
            bool success = _databaseService.AddReview(CurrentUser.userId, CurrentRequest.requestId, rating, reviewText);
            if (success)
            {
                MessageBox.Show("Ваш отзыв был успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
                // Скрываем панель после отправки отзыва
            }
            else
            {
                MessageBox.Show("Ошибка при добавлении отзыва.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
