using System;
using System.Collections.Generic;
using System.Data.SQLite;
using ExpertSystem.Models;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace ExpertSystem.Services
{
    public class Database
    {
        private readonly SQLiteConnection _connection;

        public Database()
        {
            _connection = new SQLiteConnection("Data Source=expert_system.db; Version=3");
            _connection.Open();
            InitializeDatabase();
        }

        public void CreateData(string sql)
        {
            var cmd = new SQLiteCommand(sql, _connection);
            cmd.ExecuteNonQuery();
        }

        public DetailQuestion GetDetailQuestion(int id)
        {
            var yesAnswer = GetAnswerAfterQuestion(id, true);
            var noAnswer = GetAnswerAfterQuestion(id, false);
            var yesQuestion = GetNextQuestion(id, true);
            var noQuestion = GetNextQuestion(id, false);
            return new DetailQuestion
            {
                NoAnswer = noAnswer,
                YesQuestion = yesQuestion,
                YesAnswer = yesAnswer,
                NoQuestions = noQuestion
            };
        }

        public void DeleteAnswer(int id)
        {
            var sql = "DELETE FROM answer WHERE answer_id = @answer_id";
            var cmd = new SQLiteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("answer_id", id);
            cmd.ExecuteNonQuery();
        }

        public Answer UpdateAnswer(Answer answer)
        {
            var sql = "UPDATE answer SET answer_text = @answer_text, question_id = @question_id, answer_true = @answer_true WHERE answer_id = @answer_id";
            var cmd = new SQLiteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("answer_text", answer.Text);
            if (answer.QuestionID == 0)
            {
                cmd.Parameters.AddWithValue("question_id", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("question_id", answer.QuestionID);
            }
            cmd.Parameters.AddWithValue("answer_true", answer.isTrue);
            if (answer.ID == 0)
            {
                cmd.Parameters.AddWithValue("answer_id", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("answer_id", answer.ID);
            }
            cmd.ExecuteNonQuery();
            return answer;
        }

        public Answer CreateAnswer(string text, byte[] image)
        {
            var sql = "INSERT INTO answer (answer_text, answer_image) VALUES (@text, @image) RETURNING answer_id, answer_text, question_id, answer_true, answer_image";
            var cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("text", text);
            if (image == null)
            {
                cmd.Parameters.AddWithValue("image", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("image", Convert.ToBase64String(image));
            }
            var response = cmd.ExecuteReader();
            if (response.HasRows)
            {
                while (response.Read())
                {
                    var answer = new Answer
                    {
                        ID = Convert.ToInt32(response["answer_id"]),
                        isTrue = response["answer_true"] != DBNull.Value ? Convert.ToBoolean(response["answer_true"]) : false,
                        Text = Convert.ToString(response["answer_text"]),
                        QuestionID = response["question_id"] == DBNull.Value ? 0 : Convert.ToInt32(response["question_id"]),
                        Image = ObjectToByteArray(response["answer_image"] == DBNull.Value ? null : response["answer_image"])
                    };
                    return answer;
                }
            }
            return null;
        }

        public Answer GetAnswerAfterQuestion(int questionId, bool isTrue)
        {
            var sql = "SELECT answer_id, answer_text, question_id, answer_true, answer_image FROM answer WHERE question_id = @question_id AND answer_true = @answer_true";
            var cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("question_id", questionId);
            cmd.Parameters.AddWithValue("answer_true", isTrue);
            var response = cmd.ExecuteReader();
            if (response.HasRows)
            {
                while (response.Read())
                {
                    var answer = new Answer
                    {
                        ID = Convert.ToInt32(response["answer_id"]),
                        isTrue = response["answer_true"] != DBNull.Value ? Convert.ToBoolean(response["answer_true"]) : false,
                        Text = Convert.ToString(response["answer_text"]),
                        QuestionID = response["question_id"] == DBNull.Value ? 0 : Convert.ToInt32(response["question_id"]),
                        Image = ObjectToByteArray(response["answer_image"] == DBNull.Value ? null : response["answer_image"])
                    };
                    return answer;
                }
            }
            return null;
        }

        public Question GetStartQuestion()
        {
            var sql = "SELECT question_id, question_text, parent_id, question_true FROM question WHERE parent_id IS NULL";
            var cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            var response = cmd.ExecuteReader();
            if (response.HasRows)
            {
                while (response.Read())
                {
                    var question = new Question
                    {
                        ID = Convert.ToInt32(response["question_id"]),
                        isTrue = response["question_true"] == DBNull.Value ? false : Convert.ToBoolean(response["question_true"]),
                        Text = Convert.ToString(response["question_text"]),
                        ParentID = response["parent_id"] == DBNull.Value ? 0 : Convert.ToInt32(response["parent_id"])
                    };
                    return question;
                }
            }
            return null;
        }

        public Question GetNextQuestion(int id, bool isTrue)
        {
            var sql = "SELECT question_id, question_text, parent_id, question_true FROM question WHERE parent_id = @parent_id AND question_true = @question_true";
            var cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("parent_id", id);
            cmd.Parameters.AddWithValue("question_true", isTrue);
            var response = cmd.ExecuteReader();
            if (response.HasRows)
            {
                while (response.Read())
                {
                    var question = new Question
                    {
                        ID = Convert.ToInt32(response["question_id"]),
                        isTrue = response["question_true"] == DBNull.Value ? false : Convert.ToBoolean(response["question_true"]),
                        Text = Convert.ToString(response["question_text"]),
                        ParentID = response["parent_id"] == DBNull.Value ? 0 : Convert.ToInt32(response["parent_id"])
                    };
                    return question;
                }
            }
            return null;
        }

        public List<Question> ResetQuestions()
        {
            var sql = "UPDATE question SET parent_id = NULL, question_true = NULL;";
            var cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            return GetQuestions();
        }

        public void DeleteQuestion(int id)
        {
            var sql = "DELETE FROM question WHERE question_id = @question_id";
            var cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("question_id", id);
            cmd.ExecuteNonQuery();
        }

        public Question UpdateQuestion(Question question)
        {
            var sql = "UPDATE question SET question_text = @text, parent_id = @parent_id, question_true = @question_true WHERE question_id = @question_id;";
            var cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("text", question.Text);
            if (question.ParentID == 0)
            {
                cmd.Parameters.AddWithValue("parent_id", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("parent_id", question.ParentID);                
            }
            cmd.Parameters.AddWithValue("question_true", question.isTrue);
            if (question.ID == 0)
            {
                cmd.Parameters.AddWithValue("question_id", DBNull.Value);
            } 
            else
            {
                cmd.Parameters.AddWithValue("question_id", question.ID);
            }            
            cmd.ExecuteNonQuery();
            return question;
        }

        public Question CreateQuestion(string text)
        {
            var sql = "INSERT INTO question (question_text) VALUES (@text) RETURNING question_id, question_text, parent_id, question_true;";
            var cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("text", text);
            var response = cmd.ExecuteReader();
            if (response.HasRows)
            {
                while (response.Read())
                {
                    var question = new Question
                    {
                        ID = Convert.ToInt32(response["question_id"]),
                        isTrue = response["question_true"] == DBNull.Value ? false : Convert.ToBoolean(response["question_true"]),
                        Text = Convert.ToString(response["question_text"]),
                        ParentID = response["parent_id"] == DBNull.Value ? 0 : Convert.ToInt32(response["parent_id"])
                    };
                    return question;
                }
            }
            return null;
        }

        public List<Answer> GetAnswers()
        {
            var answerList = new List<Answer>();
            var answerSql = @"SELECT answer_id, answer_text, question_id, answer_true, answer_image
                            FROM answer
                            ORDER BY answer_id;";
            var answerCmd = new SQLiteCommand(answerSql, _connection);
            var response = answerCmd.ExecuteReader();
            if (response.HasRows)
            {
                while (response.Read())
                {
                    var answer = new Answer 
                    { 
                        ID = Convert.ToInt32(response["answer_id"]),
                        isTrue = response["answer_true"] != DBNull.Value ? Convert.ToBoolean(response["answer_true"]) : false,
                        Text = Convert.ToString(response["answer_text"]),
                        QuestionID = response["question_id"] == DBNull.Value ? 0 : Convert.ToInt32(response["question_id"]),
                        Image = ObjectToByteArray(response["answer_image"] == DBNull.Value ? null : response["answer_image"]) 
                    };
                    answerList.Add(answer);
                }
            }
            return answerList;
        }

        public List<Question> GetQuestions()
        {
            var questionList = new List<Question>();
            var questionSql = @"SELECT question_id, question_text, parent_id, question_true
                                FROM question
                                ORDER BY question_id;";
            var questionCmd = new SQLiteCommand(questionSql, _connection);
            var response = questionCmd.ExecuteReader();
            if (response.HasRows)
            {
                while (response.Read())
                {
                    var question = new Question
                    {
                        ID = Convert.ToInt32(response["question_id"]),
                        isTrue = response["question_true"] == DBNull.Value ? false : Convert.ToBoolean(response["question_true"]),
                        Text = Convert.ToString(response["question_text"]),
                        ParentID = response["parent_id"] == DBNull.Value ? 0 : Convert.ToInt32(response["parent_id"])
                    };
                    questionList.Add(question);
                }
            }
            return questionList;
        }

        private byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            try
            {
                return Convert.FromBase64String(obj.ToString());
            }
            catch (Exception)
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    bf.Serialize(ms, obj);
                    return ms.ToArray();
                }
            }
        }

        private void InitializeDatabase()
        {
            var questionSql = @"CREATE TABLE IF NOT EXISTS question (
                                    question_id INTEGER PRIMARY KEY,
                                    question_text TEXT NOT NULL,
                                    parent_id INTEGER NULL,
                                    question_true BOOLEAN NULL
                                ); ";
            var answerSql = @"CREATE TABLE IF NOT EXISTS answer (
                                answer_id INTEGER PRIMARY KEY,
                                answer_text TEXT NOT NULL,
                                question_id INTEGER NULL,
                                answer_true BOOLEAN NULL,
                                answer_image TEXT NULL,
                                FOREIGN KEY (question_id) REFERENCES question(question_id) ON DELETE SET NULL
                            );";
            var questionCommand = new SQLiteCommand(questionSql, _connection);
            questionCommand.ExecuteNonQuery();

            var answerCommand = new SQLiteCommand(answerSql, _connection);
            answerCommand.ExecuteNonQuery();
        }
    }
}
