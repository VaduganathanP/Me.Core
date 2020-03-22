using Leveldb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text;

namespace Me.Core
{
    public static class Answer
    {
        static string _sqlConnectionString = Resource.SqlConnectionString;
        static sbyte[] _err = new sbyte[] { };

        public static Reply GetAnswer(Reply currentReply)
        {
            if (currentReply == null)
                return null;

            Reply retVal = new Reply
            {
                IsAnswer = currentReply.IsAnswer,
                Answers = currentReply.Answers,
                Question = currentReply.Question
            };
            string answerId = currentReply.AnswerId;
            ulong vallen = 0;
            byte[] returnBytes = null;
            DB db = null;

            if (currentReply.Answers.Count > 1)
            {
                using (SqlConnection connection = new SqlConnection(_sqlConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("update tblAnswers set UsedCount=UsedCount - 1 where Id=@answerId", connection))
                    {
                        command.Parameters.AddWithValue("answerId", answerId);
                        command.Connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                for (int i = 0; i < currentReply.Answers.Count; i++)
                {
                    if (currentReply.Answers[i] == currentReply.AnswerId)
                    {
                        if (i == currentReply.Answers.Count - 1)
                            i = -1;
                        retVal.AnswerId = currentReply.Answers[i + 1];
                        retVal.CurrentAnswerIndex = i + 2;
                        answerId = retVal.AnswerId;
                        using (db = new DB(_err))
                            returnBytes = db.Get(answerId, ref vallen, _err);
                        using (SqlConnection connection = new SqlConnection(_sqlConnectionString))
                        {
                            using (SqlCommand command = new SqlCommand("update tblAnswers set UsedCount=UsedCount + 1 where Id=@answerId", connection))
                            {
                                command.Parameters.AddWithValue("answerId", answerId);
                                command.Connection.Open();
                                command.ExecuteNonQuery();
                            }
                        }
                        break;
                    }

                }
            }

            if (returnBytes != null)
                retVal.DisplayValue = Encoding.UTF8.GetString(returnBytes);
            else
                retVal = null;

            return retVal;
        }
        public static Reply GetAnswer(Request request)
        {
            Reply retVal = new Reply() { IsAnswer = true };
            ulong vallen = 0;
            byte[] returnBytes = null;
            DB db = null;
            string answerId;
            DataTable table = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(_sqlConnectionString))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter("select TA.Id,TA.UsedCount from tblQuestions TQ inner join tblAnswers TA on TQ.Id = TA.QuestionId where TQ.Question=@Question order by TA.UsedCount desc", connection))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("Question", request.Question);
                        adapter.Fill(table);
                    }
                }

                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                        retVal.Answers.Add(row[0].ToString());

                    answerId = table.Rows[0][0].ToString();
                    retVal.AnswerId = answerId;
                    retVal.Question = request.Question;

                    retVal.CurrentAnswerIndex = 1;
                    using (db = new DB(_err))
                        returnBytes = db.Get(answerId, ref vallen, _err);

                    using (SqlConnection connection = new SqlConnection(_sqlConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand("update tblAnswers set UsedCount=UsedCount + 1 where Id=@answerId", connection))
                        {
                            command.Parameters.AddWithValue("answerId", answerId);
                            command.Connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            if (returnBytes != null)
                retVal.DisplayValue = Encoding.UTF8.GetString(returnBytes);
            else
                retVal = null;
            return retVal;
        }
        public static void SaveAnswer(Request request)
        {
            string answer;
            if (!string.IsNullOrEmpty(request.Answer))
                answer = request.Answer;
            else
                answer = request.DisplayValue;
            string questionId;
            string answerId = Guid.NewGuid().ToString();

            using (SqlConnection connection = new SqlConnection(_sqlConnectionString))
            {
                using (SqlCommand command = new SqlCommand("select Id from tblQuestions where Question=@Question", connection))
                {
                    command.Parameters.AddWithValue("Question", request.Question);
                    command.Connection.Open();
                    questionId = command.ExecuteScalar() == null ? null : command.ExecuteScalar().ToString();
                }
            }

            if (questionId == null)
            {
                questionId = Guid.NewGuid().ToString();
                using (SqlConnection connection = new SqlConnection(_sqlConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("INSERT INTO tblQuestions (Id, Question) VALUES (@Id, @Question)", connection))
                    {
                        command.Parameters.AddWithValue("Id", questionId);
                        command.Parameters.AddWithValue("Question", request.Question);
                        command.Connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }

            using (SqlConnection connection = new SqlConnection(_sqlConnectionString))
            {
                using (SqlCommand command = new SqlCommand("INSERT INTO tblAnswers (Id, QuestionId, UsedCount) VALUES (@Id, @QuestionId, @UsedCount)", connection))
                {
                    command.Parameters.AddWithValue("Id", answerId);
                    command.Parameters.AddWithValue("QuestionId", questionId);
                    command.Parameters.AddWithValue("UsedCount", 1);
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }
            }


            using (DB db = new DB(_err))
                db.Put(answerId, answer, _err);
        }
    }
}
