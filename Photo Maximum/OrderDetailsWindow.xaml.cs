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
    }
}