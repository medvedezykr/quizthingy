
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System.Text.Json;

namespace QuizOnline {
    
    public class User
    {
        public  int Id{get;set;}
        public string name{get;set;}
        
        public string password{get;set;}
    }
    public class UserSession
{
    public string Username { get; set; }
    public string Token { get; set; }
    public bool IsLoggedIn => !string.IsNullOrEmpty(Token);
}
   public class Preset
    {
        public string name{get;set;}
        public AnswerMe[] questions {get;set;}
    }
     public class AnswerMe
    {
        public int id { get; set; }
         public int presetid { get; set; }
        public string name { get; set; }       
        public string[] options { get; set; }  
        public string correct { get; set; }     
    }
 
         public class AnswerMeWithPreset : AnswerMe
{
    public string PresetName { get; set; } = string.Empty;
}
 public class PresetNId
    {
        public string name{get;set;}
        public int id {get;set;}
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

                string createTables = @"
                    CREATE TABLE IF NOT EXISTS SessionTokens(
                     id INT PRIMARY KEY DEFAULT 1,
                        SessionToken TEXT NOT NULL,
                        ExpiresAt TEXT NOT NULL
                          CONSTRAINT force_single_row CHECK (id = 1)
                    );  
                    CREATE TABLE IF NOT EXISTS Presets (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        QuizName TEXT NOT NULL UNIQUE
                    );

                    CREATE TABLE IF NOT EXISTS Questions (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        PresetId INTEGER,
                        QuestionText TEXT NOT NULL,
                        OptionsJson TEXT NOT NULL,
                        CorrectAnswer TEXT NOT NULL,
                        FOREIGN KEY (PresetId) REFERENCES Presets(Id) ON DELETE CASCADE
                    );";

                using (var command = new SqliteCommand(createTables, connection))
                {
                    command.ExecuteNonQuery();
                }

              
            }
        }

        
        public static void AddPreset(string name)
        {
            using (var connection = new SqliteConnection(quizconnection))
            {
                connection.Open();

                
                string sql = "INSERT INTO Presets (QuizName) VALUES (@name);";

                using (var command = new SqliteCommand(sql, connection))
                {
                   
                    command.Parameters.AddWithValue("@name", name);

                command.ExecuteNonQuery();
            
           
                }
            }
        }
public static void   AddQuestion(AnswerMe questiontoadd, string towhere)
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
command.Parameters.AddWithValue("@PresetId", GetIdByNamePreset(towhere));

                command.ExecuteNonQuery();
                }
                

            }
        }
        
       public static string[] GetAllQuizNames()
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
            return names.ToArray();
        }

public static AnswerMe[] GetQuestionsForQuiz(int quizId)
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
    return questionsList.ToArray();
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
     public static void RemoveQuestionInternal(int idofaquestiontodelete)
        {
             using (var connection = new SqliteConnection(quizconnection))
            {
                connection.Open();
string sql = $@"
DELETE FROM Questions WHERE Id = @idofaquestiontodelete
"; using (var command = new SqliteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@idofaquestiontodelete", idofaquestiontodelete);
                command.ExecuteNonQuery();
            }
            }
            }
public static AnswerMe[] GetAllQuestions()
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
return questionsList.ToArray();
    }
    public static string GetPresetNameOfAQuestion(int QuestionId)
        {
             using (var connection = new SqliteConnection(quizconnection))
    {
        connection.Open();

        string sql = @"
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
    public static int GetIdByNamePreset(string name)
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
    return Convert.ToInt32(thing);
    }
    public static AnswerMeWithPreset[] GetAllQuestionsWithPreset()
        {
            var questionsList = new List<AnswerMeWithPreset>();

    using (var connection = new SqliteConnection(quizconnection))
    {
        connection.Open();

        string sql = @"SELECT q.Id, q.PresetId, q.QuestionText, q.OptionsJson, q.CorrectAnswer, p.Quizname
        From Questions q  
        INNER JOIN Presets p ON q.PresetId = p.Id
        ";

        using (var command = new SqliteCommand(sql, connection))
        {
using (var reader = command.ExecuteReader())
            {
                
                while (reader.Read())
                {
                    var question = new AnswerMeWithPreset();
                    
                      question.id = reader.GetInt32(0);
                    question.presetid = reader.GetInt32(1);
                   

                    question.name = reader.GetString(2);      
                    question.correct = reader.GetString(4);   
                    question.PresetName = reader.GetString(5);
 string rawJsonText = reader.GetString(3);
                    question.options = JsonSerializer.Deserialize<string[]>(rawJsonText);
                   
                    

                    
                    questionsList.Add(question);
                }
            
          
        }

            
}
}
return questionsList.ToArray();
    }
   
     public static void DeleteQuestion(int idofaquestiontodelete)
        {
             using (var connection = new SqliteConnection(quizconnection))
            {
                connection.Open();
string sql = $@"
DELETE FROM Questions WHERE Id = @idofaquestiontodelete
"; using (var command = new SqliteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@idofaquestiontodelete", idofaquestiontodelete);
                command.ExecuteNonQuery();
            }
            }
            }
            public static Preset GetQuiz(string quiztoget)
        {
            Preset thingtoreturn= new();
             List<AnswerMe> questions = new();
             using (SqliteConnection conn = new SqliteConnection(quizconnection))
            {
                conn.Open();
                string getname =@"
SELECT quizname FROM Presets WHERE quizname = @quiztoget
                "; 
                string getquestions = @"
SELECT q.Id, q.PresetId, q.QuestionText, q.OptionsJson, q.CorrectAnswer 
FROM Questions q
INNER JOIN Presets p ON q.PresetId = p.Id
WHERE p.QuizName = @quiztoget";
                using (SqliteCommand cmd = new SqliteCommand(getname, conn))
            {
                
                cmd.Parameters.AddWithValue("@quiztoget", quiztoget);
object name = cmd.ExecuteScalar();
thingtoreturn.name = name.ToString();
            }
              using (SqliteCommand cmd = new SqliteCommand(getquestions, conn))
                {
                    cmd.Parameters.AddWithValue("@quiztoget", quiztoget);
                    using (var reader = cmd.ExecuteReader())  //ambiguous column name Id
            {
                
                while (reader.Read())
                        {
                           AnswerMe question = new();
                            question.id = reader.GetInt32(0);
                    question.presetid = reader.GetInt32(1);
                    question.name = reader.GetString(2);     
                  string rawJsonText = reader.GetString(3);
                   question.options = JsonSerializer.Deserialize<string[]>(rawJsonText); 
                    question.correct = reader.GetString(4);   

questions.Add(question);
                        }
                    
                        }
                        }

                }

thingtoreturn.questions = questions.ToArray(); 
return thingtoreturn;  
}

public static void AddQuestionsList(AnswerMe[] questions, string towhere)
        {
            using (var connection = new SqliteConnection(quizconnection))
            {
                connection.Open();
                using(var trans = connection.BeginTransaction())
                {
                    try{
                    foreach (AnswerMe question in questions)
                    {
                        string sql = @"
    INSERT INTO Questions (PresetId, QuestionText, OptionsJson, CorrectAnswer) 
    VALUES (@PresetId, @QuestionText, @OptionsJson, @CorrectAnswer);
    ";
     using (var command = new SqliteCommand(sql, connection, trans))
                    {
                            string jsonString = JsonSerializer.Serialize(question.options);
command.Parameters.AddWithValue("@CorrectAnswer", question.correct);
command.Parameters.AddWithValue("@QuestionText", question.name);
command.Parameters.AddWithValue("@OptionsJson", jsonString);
command.Parameters.AddWithValue("@PresetId", GetIdByNamePreset(towhere));
                        command.ExecuteNonQuery();
                    }
                    }
trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
public static void AddToken(TokenAndTime token)
        {
            using SqliteConnection con = new(quizconnection);
            con.Open();
            string sql =@"
            INSERT INTO SessionTokens (SessionToken, ExpiresAt)
            VALUES (@SessionToken, @ExpiresAt);
            ";
            using SqliteCommand cmd = new(sql, con);
cmd.Parameters.AddWithValue("@SessionToken", token.token);
cmd.Parameters.AddWithValue("@ExpiresAt", token.expirsAt.ToString());
cmd.ExecuteNonQuery();
        }
public static void WipeTokens()
        {
            using SqliteConnection con = new(quizconnection);
            con.Open();
            string sql =@"
            DELETE FROM SessionTokens
            ";
            using SqliteCommand cmd = new(sql, con);
            cmd.ExecuteNonQuery();
        }
        public static string? GetSessionToken()
        {
            using SqliteConnection con = new(quizconnection);
            con.Open();
            string sql = @"
            SELECT SessionToken FROM SessionTokens LIMIT 1;
            ";
            using SqliteCommand cmd = new(sql,con);
            object? token = cmd.ExecuteScalar();
if(token==null)return null;
            return token.ToString();
        }
}

}