using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Photo_Maximum
{
    public partial class NewRequestPage : Page
    {
        // Словарь для хранения размеров и их стоимости
        private Dictionary<string, Dictionary<string, int>> _itemPrices = new Dictionary<string, Dictionary<string, int>>
        {
            { "Кружка", new Dictionary<string, int> { { "300 мл", 500 }, { "400 мл", 600 }, { "500 мл", 700 } } },
            { "Футболка", new Dictionary<string, int> { { "XS", 1000 }, { "S", 1100 }, { "M", 1200 }, { "L", 1300 }, { "XL", 1400 } } },
            { "Подушка", new Dictionary<string, int> { { "60х40 см", 1500 }, { "70х50 см", 1600 }, { "60х60 см", 1700 }, { "70х70 см", 1800 }, { "80х40 см", 1900 } } }
        };

        // Переменные для хранения выбранных значений
        private string _selectedItemType;
        private string _selectedItemSize;
        private string _photoPath;

        public NewRequestPage()
        {
            InitializeComponent();
        }

        // Обработчик кнопки "Назад"
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Client());
        }

        // Обработчик выбора типа предмета
        private void ItemTypeCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border)
            {
                // Сбрасываем стиль для всех карточек типа
                foreach (var card in ItemTypePanel.Children)
                {
                    if (card is Border cardBorder)
                    {
                        cardBorder.BorderBrush = Brushes.Transparent;
                        cardBorder.Background = Brushes.White;
                    }
                }

                // Применяем стиль к выбранной карточке типа
                border.BorderBrush = Brushes.DodgerBlue;
                border.Background = Brushes.AliceBlue;

                // Получаем выбранный тип
                _selectedItemType = (border.Child as StackPanel)?.Children[0] is TextBlock textBlock ? textBlock.Text : null;

                // Сбрасываем выбранный размер
                _selectedItemSize = null;

                // Сбрасываем стиль для всех карточек размеров
                foreach (var card in ItemSizePanel.Children)
                {
                    if (card is Border cardBorder)
                    {
                        cardBorder.BorderBrush = Brushes.Transparent;
                        cardBorder.Background = Brushes.White;
                    }
                }

                // Обновляем доступные размеры
                UpdateSizes();
                UpdateCost();
            }
        }

        // Обновление доступных размеров
        private void UpdateSizes()
        {
            ItemSizePanel.Children.Clear();

            if (_selectedItemType != null && _itemPrices.ContainsKey(_selectedItemType))
            {
                foreach (var size in _itemPrices[_selectedItemType].Keys)
                {
                    var stackPanel = new StackPanel
                    {
                        Children =
                {
                    new TextBlock
                    {
                        Text = size,
                        FontSize = 16,
                        Foreground = Brushes.Black,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                }
                    };

                    // Добавляем информацию о размерах для футболок
                    if (_selectedItemType == "Футболка")
                    {
                        stackPanel.Children.Add(new TextBlock
                        {
                            Text = GetSizeInfo(size),
                            FontSize = 12,
                            Foreground = Brushes.Gray,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(0, 5, 0, 0)
                        });
                    }

                    var sizeCard = new Border
                    {
                        Style = (Style)FindResource("OrderCardStyle"),
                        Margin = new Thickness(5),
                        Child = stackPanel
                    };

                    sizeCard.MouseLeftButtonDown += SizeCard_Click;
                    ItemSizePanel.Children.Add(sizeCard);
                }
            }
        }

        // Получение информации о размерах для футболок
        private string GetSizeInfo(string size)
        {
            switch (size)
            {
                case "XS": return "Обхват груди: 81-86 см\nДлина: 66 см";
                case "S": return "Обхват груди: 86-91 см\nДлина: 68 см";
                case "M": return "Обхват груди: 91-96 см\nДлина: 70 см";
                case "L": return "Обхват груди: 96-101 см\nДлина: 72 см";
                case "XL": return "Обхват груди: 101-106 см\nДлина: 74 см";
                default: return "";
            }
        }

        // Обработчик выбора размера
        private void SizeCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border)
            {
                // Сбрасываем стиль для всех карточек
                foreach (var card in ItemSizePanel.Children)
                {
                    if (card is Border cardBorder)
                    {
                        cardBorder.BorderBrush = Brushes.Transparent;
                        cardBorder.Background = Brushes.White;
                    }
                }

                // Применяем стиль к выбранной карточке
                border.BorderBrush = Brushes.DodgerBlue;
                border.Background = Brushes.AliceBlue;

                // Получаем выбранный размер
                _selectedItemSize = (border.Child as StackPanel)?.Children[0] is TextBlock textBlock ? textBlock.Text : null;

                // Обновляем стоимость
                UpdateCost();
            }
        }

        // Обработчик загрузки фотографии
        private void UploadPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*",
                Title = "Выберите фотографию"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _photoPath = openFileDialog.FileName;
                UploadedPhoto.Source = new BitmapImage(new Uri(_photoPath));
                MessageBox.Show("Фотография загружена: " + _photoPath, "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Обработчик создания заказа
        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedItemType) || string.IsNullOrEmpty(_selectedItemSize) || string.IsNullOrEmpty(_photoPath))
            {
                MessageBox.Show("Пожалуйста, заполните все поля и загрузите фотографию.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Сохраняем фотографию (путь к файлу)
                string photoFileName = Path.GetFileName(_photoPath);
                string savePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Photos", photoFileName);
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                File.Copy(_photoPath, savePath, true);

                // Сохраняем заказ в базу данных
                var databaseService = new DatabaseService("Server=95.31.128.97;Database=PhotoMaximum;User Id=admin;Password=winServer=;");
                databaseService.CreateRequest(CurrentUser.userId, _selectedItemType, _selectedItemSize, savePath, CommentTextBox.Text, _itemPrices[_selectedItemType][_selectedItemSize]);

                MessageBox.Show("Заказ успешно создан!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new Client()); // Возвращаемся на предыдущую страницу
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании заказа: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обновление стоимости заказа
        private void UpdateCost()
        {
            if (!string.IsNullOrEmpty(_selectedItemType) && !string.IsNullOrEmpty(_selectedItemSize))
            {
                // Проверяем, существует ли выбранный тип в словаре
                if (_itemPrices.ContainsKey(_selectedItemType))
                {
                    // Проверяем, существует ли выбранный размер для этого типа
                    if (_itemPrices[_selectedItemType].ContainsKey(_selectedItemSize))
                    {
                        int cost = _itemPrices[_selectedItemType][_selectedItemSize];
                        CostTextBlock.Text = $"Стоимость: {cost} руб.";
                    }
                    else
                    {
                        // Если размер не найден, сбрасываем стоимость
                        CostTextBlock.Text = "Стоимость: 0 руб.";
                    }
                }
                else
                {
                    // Если тип не найден, сбрасываем стоимость
                    CostTextBlock.Text = "Стоимость: 0 руб.";
                }
            }
            else
            {
                // Если тип или размер не выбраны, сбрасываем стоимость
                CostTextBlock.Text = "Стоимость: 0 руб.";
            }
        }
    }
}