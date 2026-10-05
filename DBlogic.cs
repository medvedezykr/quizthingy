using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using System.Linq.Expressions;
using System.Security.AccessControl;
using SQLitePCL;

namespace Mainquizthing
{
     public class AnswerMe
    {
        public int id { get; set; }
         public int presetid { get; set; }
        public string name { get; set; }       
        public string[] options { get; set; }  
        public string correct { get; set; }     
    }
    public static class DBmng
    {
        
       static string executableFolder = AppDomain.CurrentDomain.BaseDirectory;
static string databasePath = Path.Combine(executableFolder, "quiz.db");

static string quizconnection = $"Data Source={databasePath};";
        
        public static void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(quizconnection))
            {
                connection.Open();

                string createPresetsTable = @"
                    CREATE TABLE IF NOT EXISTS Presets (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        QuizName TEXT NOT NULL UNIQUE
                    );";

                string createQuestionsTable = @"
                    CREATE TABLE IF NOT EXISTS Questions (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        PresetId INTEGER,
                        QuestionText TEXT NOT NULL,
                        OptionsJson TEXT NOT NULL,
                        CorrectAnswer TEXT NOT NULL,
                        FOREIGN KEY (PresetId) REFERENCES Presets(Id) ON DELETE CASCADE
                    );";

                using (var command = new SqliteCommand(createPresetsTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new SqliteCommand(createQuestionsTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        
        public static void AddQuizPreset(string name)
        {
            using (var connection = new SqliteConnection(quizconnection))
            {
                connection.Open();

                
                string sql = "INSERT INTO Presets (QuizName) VALUES (@name);";

                using (var command = new SqliteCommand(sql, connection))
                {
                   
                    command.Parameters.AddWithValue("@name", name);

                     try
            {
                command.ExecuteNonQuery();
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
               
                Console.WriteLine($"[!] Note: '{name}' already exists in the database. so this will not be added.");
            }
                }
            }
        }
public static void   AddQuestionInternal(AnswerMe questiontoadd, string towhere)
        {
            using (var connection = new SqliteConnection(quizconnection))
            {
                connection.Open();
string sql = @"
    INSERT INTO Questions (PresetId, QuestionText, OptionsJson, CorrectAnswer) 
    VALUES (@PresetId, @QuestionText, @OptionsJson, @CorrectAnswer);
    ";

using (var command = new SqliteCommand(sql, connection))
                {
                    string jsonString = JsonSerializer.Serialize(questiontoadd.options);
command.Parameters.AddWithValue("@CorrectAnswer", questiontoadd.correct);
command.Parameters.AddWithValue("@QuestionText", questiontoadd.name);
command.Parameters.AddWithValue("@OptionsJson", jsonString);
command.Parameters.AddWithValue("@PresetId", towhere);

                command.ExecuteNonQuery();
                }
                

            }
        }
        
        public static List<string> GetAllQuizNames()
        {
            var names = new List<string>();

            using (var connection = new SqliteConnection(quizconnection))
            {
                connection.Open();

                string sql = "SELECT QuizName FROM Presets;";

                using (var command = new SqliteCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        
                        while (reader.Read())
                        {
                            names.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return names;
        }
      

public static List<AnswerMe> GetQuestionsForQuiz(string quizId)
{
    var questionsList = new List<AnswerMe>();

    using (var connection = new SqliteConnection(quizconnection))
    {
        connection.Open();

        string sql = "SELECT Id,PresetId, QuestionText, OptionsJson, CorrectAnswer FROM Questions WHERE PresetId = @quizId;";

        using (var command = new SqliteCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@quizId", quizId);

            using (var reader = command.ExecuteReader())
            {
                
                while (reader.Read())
                {
                    var question = new AnswerMe();
                    
                      question.id = reader.GetInt32(0);
                    question.presetid = reader.GetInt32(1);
                    question.name = reader.GetString(2);     
                  string rawJsonText = reader.GetString(3);
                    question.correct = reader.GetString(4);   

                     
                    question.options = JsonSerializer.Deserialize<string[]>(rawJsonText); 
                    

                    
                    questionsList.Add(question);
                }
            }
        }
    }
    return questionsList;
}
public static void DeleteAPresetInternal(string PresetToDelete)
        {
              using (var connection = new SqliteConnection(quizconnection))
            {
                connection.Open();
string sql = $@"
DELETE FROM Presets WHERE QuizName = @PresetToDelete;
";
using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@PresetToDelete", PresetToDelete);
                    command.ExecuteNonQuery();
                }



        }

    }
     public static void RemoveQuestionInternal(string idofaquestiontodelete)
        {
             using (var connection = new SqliteConnection(quizconnection))
            {
                connection.Open();
string sql = $@"
DELETE FROM Questions WHERE QuestionText = @idofaquestiontodelete
"; using (var command = new SqliteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@idofaquestiontodelete", idofaquestiontodelete);
                command.ExecuteNonQuery();
            }
            }
            }
public static List<AnswerMe> GetAllQuestions()
        {
            var questionsList = new List<AnswerMe>();

    using (var connection = new SqliteConnection(quizconnection))
    {
        connection.Open();

        string sql = "SELECT Id, PresetId, QuestionText, OptionsJson, CorrectAnswer FROM Questions";

        using (var command = new SqliteCommand(sql, connection))
        {
using (var reader = command.ExecuteReader())
            {
                
                while (reader.Read())
                {
                    var question = new AnswerMe();
                    
                      question.id = reader.GetInt32(0);
                    question.presetid = reader.GetInt32(1);
                   

                    question.name = reader.GetString(2);      
                    question.correct = reader.GetString(4);   
 string rawJsonText = reader.GetString(3);
                    question.options = JsonSerializer.Deserialize<string[]>(rawJsonText);
                   
                    

                    
                    questionsList.Add(question);
                }
            
          
        }

            
}
}
return questionsList;
    }
    public static string GetPresetNameOfAQuestion(int QuestionId)
        {
             using (var connection = new SqliteConnection(quizconnection))
    {
        connection.Open();

        string sql = $@"
SELECT Presets.QuizName
FROM Questions
JOIN Presets ON Questions.Presetid = Presets.id
WHERE Questions.Id = @QuestionId;
";

          using (var command = new SqliteCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@QuestionId", QuestionId);

            object result = command.ExecuteScalar();

            if (result != null && result != DBNull.Value)
            {
                return result.ToString();
            }
            
            return "non existant preset";
        }
    
    }
    }
    public static string GetIdByNamePreset(string name)
        {
           object thing;
  using (var connection = new SqliteConnection(quizconnection))
    {
        connection.Open();

        string sql = $@"
SELECT Id
FROM Presets
WHERE Quizname = @name;
";

          using (var command = new SqliteCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@name", name);
 thing = command.ExecuteScalar();

        }
        
    }
    return Convert.ToString(thing);
    }
}
}