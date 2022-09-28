using ExpertSystem.Models;
using ExpertSystem.Pages;
using ExpertSystem.Services;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ExpertSystem
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Database _database;
        private Question _currentQuestion;
        private List<LocalAnswer> _answers;

        public MainWindow()
        {
            InitializeComponent();
            _database = new Database();
            var question = _database.GetStartQuestion();
            _answers = new List<LocalAnswer>();
            _currentQuestion = question;
            updateQuestionText();
        }

        private void menu_btn_exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btn_agree_Click(object sender, RoutedEventArgs e)
        {
            var nextQuestion = _database.GetNextQuestion(_currentQuestion.ID, true);
            _answers.Add(new LocalAnswer { isTrue = true, Question = _currentQuestion.Text });
            if (nextQuestion != null)
            {
                _currentQuestion = nextQuestion;
                updateQuestionText();
            }
            else
            {
                var answer = _database.GetAnswerAfterQuestion(_currentQuestion.ID, true);
                hide_button();
                if (answer != null)
                {
                    tb_question_text.Text = "Вам подходит: " + answer.Text.ToLower();
                    if (answer.Image != null && convertByteArray(answer.Image) != null)
                    {
                        answer_logo.Source = convertByteArray(answer.Image);
                        answer_logo.Opacity = 1;
                        question_logo.Opacity = 0;
                    }
                }
                else
                {
                    tb_question_text.Text = "Ответа на данную комбинацию у меня нет =(";
                }
            }
        }

        private void btn_disagree_Click(object sender, RoutedEventArgs e)
        {
            var nextQuestion = _database.GetNextQuestion(_currentQuestion.ID, false);
            _answers.Add(new LocalAnswer { isTrue = false, Question = _currentQuestion.Text });
            if (nextQuestion != null)
            {
                _currentQuestion = nextQuestion;
                updateQuestionText();
            }
            else
            {
                var answer = _database.GetAnswerAfterQuestion(_currentQuestion.ID, false);
                hide_button();
                if (answer != null)
                {
                    tb_question_text.Text = "Вам подходит: " + answer.Text.ToLower();
                    if (answer.Image != null && convertByteArray(answer.Image) != null)
                    {
                        answer_logo.Source = convertByteArray(answer.Image);
                        answer_logo.Opacity = 1;
                        question_logo.Opacity = 0;
                    }
                }
                else
                {
                    tb_question_text.Text = "Ответа на данную комбинацию у меня нет =(";
                }
            }
        }

        private void updateQuestionText()
        {
            tb_question_text.Text = _currentQuestion != null ? _currentQuestion.Text : "Здесь появится вопрос для Вас";
            if (_currentQuestion == null)
            {
                hide_button();
            }
        }

        private void show_button()
        {
            btn_agree.Opacity = 1;
            btn_disagree.Opacity = 1;
            answer_logo.Opacity = 0;
            question_logo.Opacity = 1;
        }

        private void hide_button()
        {
            btn_agree.Opacity = 0;
            btn_disagree.Opacity = 0;
        }

        private void menu_btn_start_Click(object sender, RoutedEventArgs e)
        {
            show_button();
            var question = _database.GetStartQuestion();
            _answers = new List<LocalAnswer>();
            _currentQuestion = question;
            updateQuestionText();
        }

        private void menu_btn_history_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new AnswerHistory(_answers);
            historyWindow.Show();
        }

        private void menu_about_Click(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "Экспертная система выбора домашнего животного\nСтудент группы: АС-19-1\nБыкова Алина";
            string caption = "О программе";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;
            MessageBox.Show(messageBoxText, caption, button, icon);
        }

        private void menu_btn_admin_Click(object sender, RoutedEventArgs e)
        {
            var adminPanel = new AdminPage();
            adminPanel.Show();
        }

        private BitmapImage convertByteArray(byte[] imgIn)
        {
            try
            {
                BitmapImage img = new BitmapImage();
                using (MemoryStream memStream = new MemoryStream(imgIn))
                {
                    img.BeginInit();
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.StreamSource = memStream;
                    img.EndInit();
                    img.Freeze();
                }
                return img;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        private void menu_btn_export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var service = new Serializer();
                service.ToFile();

                string messageBoxText = "Экспорт произошел успешно в файл data.sql";
                string caption = "Экспорт в файл";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Information;
                MessageBox.Show(messageBoxText, caption, button, icon);
            }
            catch (System.Exception)
            {
                string messageBoxText = "Не удалось экспортировать данные в файл";
                string caption = "Экспорт в файл";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBox.Show(messageBoxText, caption, button, icon);
            }
        }

        private void menu_btn_import_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "SQL files (*.sql)|*.sql";
                if (openFileDialog.ShowDialog() == true)
                {
                    var data = File.ReadAllText(openFileDialog.FileName);
                    if (data.StartsWith("INSERT INTO question"))
                    {
                        var service = new Serializer();
                        service.FromFile(data);

                        string messageBoxText = "Импорт из файла произошел успешно";
                        string caption = "Импорт из файла";
                        MessageBoxButton button = MessageBoxButton.OK;
                        MessageBoxImage icon = MessageBoxImage.Information;
                        MessageBox.Show(messageBoxText, caption, button, icon);
                    }
                    else
                    {
                        string messageBoxText = "Неправильное содержимое файла!";
                        string caption = "Импорт из файла";
                        MessageBoxButton button = MessageBoxButton.OK;
                        MessageBoxImage icon = MessageBoxImage.Error;
                        MessageBox.Show(messageBoxText, caption, button, icon);
                    }
                }
            }
            catch (System.Exception)
            {
                string messageBoxText = "Ошибка при импорте из файла!";
                string caption = "Импорт из файла";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBox.Show(messageBoxText, caption, button, icon);
            }
        }
    }
}
