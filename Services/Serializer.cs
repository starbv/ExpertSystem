using System;
using System.IO;

namespace ExpertSystem.Services
{
    public class Serializer
    {
        private readonly Database _database;

        public Serializer()
        {
            _database = new Database();
        }

        public void ToFile()
        {
            var questions = _database.GetQuestions();
            var answers = _database.GetAnswers();

            var questionSql = "INSERT INTO question (question_id, question_text, parent_id, question_true) VALUES ";
            foreach (var question in questions)
            {
                questionSql += $"({question.ID}, '{question.Text}',";

                if (question.ParentID == 0)
                {
                    questionSql += "null, ";
                }
                else
                {
                    questionSql += question.ParentID + ", ";
                }

                if (question.isTrue == false && question.ParentID == 0)
                {
                    questionSql += "null), ";
                }
                else
                {
                    questionSql += question.isTrue + "), ";
                }
            }
            questionSql = questionSql.Remove(questionSql.Length - 2) + ";\n";

            var answersSql = string.Empty;
            foreach (var answer in answers)
            {
                var answerSql = "INSERT INTO answer (answer_id, answer_text, question_id, answer_true, answer_image) VALUES ";
                answerSql += $"({answer.ID}, '{answer.Text}', ";
                if (answer.QuestionID == 0)
                {
                    answerSql += "null, ";
                }
                else
                {
                    answerSql += answer.QuestionID + ", ";
                }

                if (answer.isTrue == false && answer.QuestionID == 0)
                {
                    answerSql += "null, ";
                }
                else
                {
                    answerSql += answer.isTrue + ", ";
                }

                if (answer.Image == null)
                {
                    answerSql += "null);";
                }
                else
                {
                    answerSql += "'" + Convert.ToBase64String(answer.Image) + "');";
                }

                answersSql += answerSql + "\n";
            }

            File.WriteAllText("data.sql", questionSql + answersSql);
        }

        public void FromFile(string data)
        {
            _database.CreateData(data);
        }
    }
}
