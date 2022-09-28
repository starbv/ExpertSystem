using ExpertSystem.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace ExpertSystem.Pages
{
    /// <summary>
    /// Логика взаимодействия для AnswerHistory.xaml
    /// </summary>
    public partial class AnswerHistory : Window
    {
        private readonly List<LocalAnswer> _answers;

        public class DataObject
        {
            public string Text { get; set; }
            public string Result { get; set; }
        }

        public AnswerHistory(List<LocalAnswer> answers)
        {
            InitializeComponent();
            var list = new ObservableCollection<DataObject>();
            foreach (var answer in answers)
            {
                list.Add(new DataObject { Text = answer.Question, Result = answer.isTrue ? "Да" : "Нет" });
            }
            answers_datagrid.ItemsSource = list;
        }
    }
}
