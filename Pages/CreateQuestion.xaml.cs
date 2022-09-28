using ExpertSystem.Services;
using System.Windows;

namespace ExpertSystem.Pages
{
    /// <summary>
    /// Логика взаимодействия для CreateQuestion.xaml
    /// </summary>
    public partial class CreateQuestion : Window
    {
        public CreateQuestion()
        {
            InitializeComponent();
        }

        private void btn_create_Click(object sender, RoutedEventArgs e)
        {
            var text = tb_question.Text;
            if (text.Trim().Length != 0)
            {
                var db = new Database();
                db.CreateQuestion(text.Trim());

                string messageBoxText = "Вопрос успешно создан!";
                string caption = "Создание вопроса";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Information;
                var result = MessageBox.Show(messageBoxText, caption, button, icon);
                if (result == MessageBoxResult.Yes)
                {
                    Close();
                }
            }
        }
    }
}
