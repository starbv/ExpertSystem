using ExpertSystem.Services;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ExpertSystem.Pages
{
    /// <summary>
    /// Логика взаимодействия для CreateAnswer.xaml
    /// </summary>
    public partial class CreateAnswer : Window
    {
        private byte[] _imageData;

        public CreateAnswer()
        {
            InitializeComponent();
        }

        private void btn_image_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Title = "Выбор фотографии";
            openFileDialog.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            if (openFileDialog.ShowDialog() == true)
            {
                var imageSrc = new BitmapImage(new Uri(openFileDialog.FileName));
                _imageData = ImageToByteArray(imageSrc);
            }
        }

        private void btn_create_Click(object sender, RoutedEventArgs e)
        {
            var answer = tb_answer.Text;
            if (answer.Trim().Length != 0)
            {
                var db = new Database();
                var answerNew = db.CreateAnswer(answer, _imageData);
                string messageBoxText = "Ответ успешно создан!";
                string caption = "Создание ответа";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Information;
                MessageBox.Show(messageBoxText, caption, button, icon);
            }
        }

        public byte[] ImageToByteArray(BitmapImage imageIn)
        {
            byte[] data = null;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageIn));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }
    }
}
