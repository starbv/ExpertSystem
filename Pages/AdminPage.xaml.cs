using ExpertSystem.Models;
using ExpertSystem.Services;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ExpertSystem.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Window
    {
        private readonly Database _database;
        private List<Question> _questions;
        private List<Answer> _answers;

        public class ObjectData
        {
            public int ID { get; set; }
            public string Text { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        public AdminPage()
        {
            InitializeComponent();
            _database = new Database();
            update_form();
        }

        private void update_form()
        {
            var questions = _database.GetQuestions();
            var answers = _database.GetAnswers();
            _questions = questions;
            _answers = answers;
            lv_questions.Items.Clear();
            cb_answer_no.Items.Clear();
            cb_answer_yes.Items.Clear();
            foreach (var question in questions)
            {
                var item = new ObjectData { ID = question.ID, Text = question.Text };
                lv_questions.Items.Add(item);
                cb_yes.Items.Add(item);
                cb_no.Items.Add(item);
            }
            foreach (var answer in answers)
            {
                var item = new ObjectData { ID = answer.ID, Text = answer.Text };
                cb_answer_no.Items.Add(item);
                cb_answer_yes.Items.Add(item);
            }
        }

        private void btn_add_question_Click(object sender, RoutedEventArgs e)
        {
            var createQuestionWindow = new CreateQuestion();
            var r = createQuestionWindow.ShowDialog();
            update_form();
        }

        private void btn_add_answer_Click(object sender, RoutedEventArgs e)
        {
            var createAnswerWindow = new CreateAnswer();
            var r = createAnswerWindow.ShowDialog();
            update_form();
        }

        private void lv_questions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = lv_questions.SelectedItem as ObjectData;
            if (selected == null)
            {
                return;
            }
            tb_question.Text = selected.Text;
            var detail = _database.GetDetailQuestion(selected.ID);
            if (detail.NoAnswer != null)
            {
                var targetIdx = _answers.FindIndex(item => item.ID == detail.NoAnswer.ID);
                cb_answer_no.SelectedIndex = targetIdx;
            }
            else
            {
                cb_answer_no.SelectedIndex = -1;
            }

            if (detail.YesAnswer != null)
            {
                var targetIdx = _answers.FindIndex(item => item.ID == detail.YesAnswer.ID);
                cb_answer_yes.SelectedIndex = targetIdx;
            }
            else
            {
                cb_answer_yes.SelectedIndex = -1;
            }

            if (detail.YesQuestion != null)
            {
                var targetIdx = _questions.FindIndex(item => item.ID == detail.YesQuestion.ID);
                cb_yes.SelectedIndex = targetIdx;
            }
            else
            {
                cb_yes.SelectedIndex = - 1;
            }

            if (detail.NoQuestions != null)
            {
                var targetIdx = _questions.FindIndex(item => item.ID == detail.NoQuestions.ID);
                cb_no.SelectedIndex = targetIdx;
            }
            else
            {
                cb_no.SelectedIndex = -1;
            }
        }

        private void btn_decline_answer_yes_Click(object sender, RoutedEventArgs e)
        {
            var selected = cb_answer_yes.SelectedItem as ObjectData;
            if (selected == null)
            {
                return;
            }
            var targetAnswer = _answers.Find(item => item.ID == selected.ID);
            var selectedQuestion = lv_questions.SelectedItem as ObjectData;
            if (selectedQuestion == null)
            {
                return;
            }
            targetAnswer.QuestionID = 0;
            _database.UpdateAnswer(targetAnswer);
            cb_answer_yes.SelectedIndex = -1;
        }

        private void btn_decline_answer_no_Click(object sender, RoutedEventArgs e)
        {
            var selected = cb_answer_no.SelectedItem as ObjectData;
            if (selected == null)
            {
                return;
            }
            var targetAnswer = _answers.Find(item => item.ID == selected.ID);
            targetAnswer.QuestionID = 0;
            _database.UpdateAnswer(targetAnswer);
            cb_answer_no.SelectedIndex = -1;
        }

        private void cb_answer_no_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = cb_answer_no.SelectedItem as ObjectData;
            if (selected == null)
            {
                return;
            }
            var targetAnswer = _answers.Find(item => item.ID == selected.ID);
            var selectedQuestion = lv_questions.SelectedItem as ObjectData;
            if (selectedQuestion == null)
            {
                return;
            }
            targetAnswer.QuestionID = selectedQuestion.ID;
            targetAnswer.isTrue = false;
            _database.UpdateAnswer(targetAnswer);
        }

        private void cb_answer_yes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = cb_answer_yes.SelectedItem as ObjectData;
            if (selected == null)
            {
                return;
            }
            var targetAnswer = _answers.Find(item => item.ID == selected.ID);
            var selectedQuestion = lv_questions.SelectedItem as ObjectData;
            if (selectedQuestion == null)
            {
                return;
            }
            targetAnswer.QuestionID = selectedQuestion.ID;
            targetAnswer.isTrue = true;
            _database.UpdateAnswer(targetAnswer);
        }

        private void cb_no_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = cb_no.SelectedItem as ObjectData;
            if (selected == null)
            {
                return;
            }
            var targetQuestion = _questions.Find(item => item.ID == selected.ID);
            var selectedQuestion = lv_questions.SelectedItem as ObjectData;
            if (selectedQuestion == null)
            {
                return;
            }
            targetQuestion.ParentID = selectedQuestion.ID;
            targetQuestion.isTrue = false;
            _database.UpdateQuestion(targetQuestion);
        }

        private void btn_decline_no_Click(object sender, RoutedEventArgs e)
        {
            var selected = cb_no.SelectedItem as ObjectData;
            if (selected == null)
            {
                return;
            }
            var targetQuestion = _questions.Find(item => item.ID == selected.ID);
            targetQuestion.ParentID = 0;
            _database.UpdateQuestion(targetQuestion);
            cb_no.SelectedIndex = -1;
        }

        private void cb_yes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = cb_yes.SelectedItem as ObjectData;
            if (selected == null)
            {
                return;
            }
            var targetQuestion = _questions.Find(item => item.ID == selected.ID);
            var selectedQuestion = lv_questions.SelectedItem as ObjectData;
            if (selectedQuestion == null)
            {
                return;
            }
            targetQuestion.ParentID = selectedQuestion.ID;
            targetQuestion.isTrue = true;
            _database.UpdateQuestion(targetQuestion);
        }

        private void btn_decline_yes_Click(object sender, RoutedEventArgs e)
        {
            var selected = cb_yes.SelectedItem as ObjectData;
            if (selected == null)
            {
                return;
            }
            var targetQuestion = _questions.Find(item => item.ID == selected.ID);
            var selectedQuestion = lv_questions.SelectedItem as ObjectData;
            if (selectedQuestion == null)
            {
                return;
            }
            targetQuestion.ParentID = 0;
            _database.UpdateQuestion(targetQuestion);
            cb_yes.SelectedIndex = -1;
        }
    }
}
