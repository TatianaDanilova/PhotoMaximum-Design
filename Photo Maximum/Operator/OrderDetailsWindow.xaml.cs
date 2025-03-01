using System.Windows;

namespace Photo_Maximum
{
    public partial class OrderDetailsWindow : Window
    {
        public OrderDetailsWindow(Order order)
        {
            InitializeComponent();
            DataContext = order; // Привязываем данные заказа
        }
        private void Close_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
      }
        
}