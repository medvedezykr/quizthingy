using Npgsql;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Xml.Serialization;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using System.Data.SqlTypes;
using Microsoft.AspNetCore.Components;

namespace QuizAPI

{
 public class User
    {
        public  int id{get;set;}=0;
        public string name{get;set;}=string.Empty;
        
        public string password{get;set;}=string.Empty;
    }
    public class AnswerMe
    {
        public int id { get; set; }
         public int presetid { get; set; }
        public string name { get; set; }       = string.Empty;
        public string[] options { get; set; }   
        public string correct { get; set; }     = string.Empty;
    }
    public class PresetView
    {
        public int id {get;set;}
        public string preset {get;set;}  = string.Empty;
        public string user {get;set;} = string.Empty;
        
    }
       public class Preset
    {
        public string name{get;set;} = string.Empty;
        public AnswerMe[] questions {get;set;} 
        
    }
    public class PresetSigned
    {
        public string name{get;set;}  = string.Empty;
        public AnswerMe[] questions {get;set;}
        public int userId{get;set;}
    }
    public class TokenAndTime
    {
        public string token{get;set;}  = string.Empty;
       public DateTime expirsAt{get;set;}
    }
    public static class DBmng
    {
       static string connectionstring = "Host=localhost;Username=postgres;Password=root;Database=postgres";
public static void InitializeDB()
        {
             using (NpgsqlConnection conn = new NpgsqlConnection(connectionstring))
        {
            conn.Open();
            string createTables = @"
             CREATE TABLE IF NOT EXISTS Users (
                    Id SERIAL PRIMARY KEY,
                    DisplayName TEXT NOT NULL,
                    UsernameLower TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL
                    );
                     CREATE TABLE IF NOT EXISTS Tokens (
                    UserId INTEGER NOT NULL, 
                    FOREIGN KEY (UserId) REFERENCES Users(Id),
                    SessionToken TEXT NOT NULL,
                    ExpiresAt TIMESTAMP WITH TIME ZONE               
                    );
                    CREATE TABLE IF NOT EXISTS Presets (
                        Id SERIAL PRIMARY KEY,
                        QuizName TEXT NOT NULL UNIQUE,
                        UserId INTEGER NOT NULL,
                        FOREIGN KEY (UserId) REFERENCES Users(Id)
                    );
                    CREATE TABLE IF NOT EXISTS Questions (
                        Id SERIAL PRIMARY KEY,
                        PresetId INTEGER,
                        QuestionText TEXT NOT NULL,
                        OptionsJson TEXT NOT NULL,
                        CorrectAnswer TEXT NOT NULL,
                        FOREIGN KEY (PresetId) REFERENCES Presets(Id) ON DELETE CASCADE
                    );
                   
                    ";
                    
            using (NpgsqlCommand cmd = new NpgsqlCommand(createTables, conn))
            {
                 cmd.ExecuteNonQuery();
            }
        
        }
        }
public static void AddQuiz(string quiz, int UserId)
        {
                     using (NpgsqlConnection conn = new NpgsqlConnection(connectionstring))
            {
                conn.Open();
                string sql = @"
                INSERT INTO Presets (QuizName, UserId) VALUES (@name,@UserId);
                ";
 using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@name", quiz);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                 cmd.ExecuteNonQuery();
            }

            }
            }
            public static void AddQuestion(AnswerMe Question, string towhere)
        {
             using (NpgsqlConnection conn = new NpgsqlConnection(connectionstring))
            {
conn.Open();
string sql = @"
INSERT INTO Questions (PresetId, QuestionText, OptionsJson, CorrectAnswer) 
VALUES (@PresetId, @QuestionText, @OptionsJson, @CorrectAnswer);
";
using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
            {
                   string jsonString = JsonSerializer.Serialize(Question.options);
cmd.Parameters.AddWithValue("@CorrectAnswer", Question.correct);
cmd.Parameters.AddWithValue("@QuestionText", Question.name);
cmd.Parameters.AddWithValue("@OptionsJson", jsonString);
cmd.Parameters.AddWithValue("@PresetId", GetIdByNamePreset(towhere));
cmd.ExecuteNonQuery();
            }
            
        }




    }
public static Preset GetQuiz(int quiztoget)
        {
            Preset thingtoreturn= new();
             List<AnswerMe> questions = new();
             using (NpgsqlConnection conn = new NpgsqlConnection(connectionstring))
            {
                conn.Open();
                string getname =@"
SELECT quizname FROM Presets WHERE quizname = @quiztoget
                "; 
                string getquestions= @"
SELECT Questions.Id,PresetId, QuestionText, OptionsJson, CorrectAnswer FROM Questions
INNER JOIN Presets ON Questions.PresetId = Presets.id
WHERE Presets.Id = @quiztoget 
                ";
                using (NpgsqlCommand cmd = new NpgsqlCommand(getname, conn))
            {
                
                cmd.Parameters.AddWithValue("@quiztoget", quiztoget);
object name = cmd.ExecuteScalar();
thingtoreturn.name = name.ToString();
            }
              using (NpgsqlCommand cmd = new NpgsqlCommand(getquestions, conn))
                {
                    cmd.Parameters.AddWithValue("@quiztoget", quiztoget);
                    using (var reader = cmd.ExecuteReader())
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
public static PresetView[] GetAllQuizNames()
        {
            List<PresetView> bundle = new();
            var names = new List<string>();

            using (var connection = new NpgsqlConnection(connectionstring))
            {
                connection.Open();
                string sql = @"
SELECT
    p.QuizName,
    u.DisplayName,
    p.Id
FROM Presets AS p
INNER JOIN Users AS u ON p.UserId = u.Id;";

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        
                        while (reader.Read())
                        {
                            PresetView thingtoreturn =new();
                            thingtoreturn.preset=reader.GetString(0);
                            thingtoreturn.user = reader.GetString(1);
                            thingtoreturn.id = reader.GetInt32(2);
                            bundle.Add(thingtoreturn);
                        }
                    }
                }
            }
            return bundle.ToArray();
        }
         public static int GetIdByNamePreset(string name)
        {
           object thing;
  using (var connection = new NpgsqlConnection(connectionstring))
    {
        connection.Open();

        string sql = $@"
SELECT Id
FROM Presets
WHERE Quizname = @name;
";

          using (var command = new NpgsqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@name", name);
 thing = command.ExecuteScalar();

        }
        
    }
    return Convert.ToInt32(thing);
    }
    public static void AddUser(string name, string password)
        {
string PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
string UserName = name.ToLower();
            using (var connection = new NpgsqlConnection(connectionstring))
            {
                connection.Open();
                string sql = @"
                INSERT INTO Users (DisplayName, UsernameLower, PasswordHash)
                VALUES (@DisplayName, @UsernameLower, @PasswordHash)
                ";
                using (NpgsqlCommand command = new(sql, connection))
                {
                    command.Parameters.AddWithValue("@DisplayName",name);
                    command.Parameters.AddWithValue("@UsernameLower",UserName);
                    command.Parameters.AddWithValue("@PasswordHash",PasswordHash);
                    command.ExecuteNonQuery();
              
                }
            }
        }
        public static bool IsNameUsed(string name)
        {
             using (var connection = new NpgsqlConnection(connectionstring))
            {
                connection.Open();
 string sql = @"
SELECT EXISTS(SELECT 1 FROM Users WHERE UsernameLower = @SearchKey);
 ";
  using (NpgsqlCommand command = new(sql, connection))
                {
                    command.Parameters.AddWithValue("@SearchKey", name);
                    object result = command.ExecuteScalar();
                    if(result == null)
                    return false;
                    return Convert.ToBoolean(result);
                }
                }
                
        }
public static string GetUserById(int targetId){
using (NpgsqlConnection con = new(connectionstring))
{
    con.Open();
    string sql =@"
    SELECT DisplayName From Users WHERE Id = @targid;
    ";
    using(NpgsqlCommand cmd = new(sql, con))
                {
                    cmd.Parameters.AddWithValue("@targid", targetId);
                  
                        object returnuser = cmd.ExecuteScalar();
                        if(returnuser==string.Empty||returnuser==null) return "";
                    return returnuser.ToString();

                    }
                    }
}
public static int? GetIdByUser(string target)
        {
            using (NpgsqlConnection con = new(connectionstring))
{ con.Open();
string sql =@"
SELECT Id FROM Users WHERE UsernameLower = @target;
    ";
 using(NpgsqlCommand cmd = new(sql, con))
                {
                    cmd.Parameters.AddWithValue("@target",target); 
int? id = Convert.ToInt32(cmd.ExecuteScalar());
return id;
                }
                }
        }
public static bool CheckPassword(string user, string password)
        {
            using (NpgsqlConnection con = new(connectionstring))
            {
                con.Open();
                string sql = @"
                SELECT PasswordHash FROM Users WHERE UsernameLower = @targetUser
                ";
                using(NpgsqlCommand cmd = new(sql, con))
                {
                    cmd.Parameters.AddWithValue("@targetUser", user.ToLower());
                    string CorrectHash = cmd.ExecuteScalar().ToString();
                    return BCrypt.Net.BCrypt.Verify(password, CorrectHash);
                     
            }
            }
}
public static TokenAndTime CreateSessionToken(int userid)
        {
             byte[] randomBytes = new byte[32];
            RandomNumberGenerator.Fill(randomBytes);
            TokenAndTime tokenAndTime = new();
            tokenAndTime.token = Convert.ToHexString(randomBytes).ToLower();
tokenAndTime.expirsAt =  DateTime.UtcNow.AddHours(2);
            using (NpgsqlConnection con = new(connectionstring))
            
            {
                con.Open();
                string sql = @"
                INSERT INTO Tokens (UserId, SessionToken, ExpiresAt)
                VALUES (@UserId, @SessionToken, @ExpiresAt)
                ";
using(NpgsqlCommand cmd = new(sql, con))
                {
                  cmd.Parameters.AddWithValue("@UserId", userid);
                  cmd.Parameters.AddWithValue("@SessionToken", tokenAndTime.token);
                  cmd.Parameters.AddWithValue("@ExpiresAt", tokenAndTime.expirsAt);
                     cmd.ExecuteNonQuery();
                     return tokenAndTime;
            }
            }
        }

        public static UserSession? GetIdAndExpTimeByToken(string token)
        {
            using (NpgsqlConnection con = new(connectionstring))
            {
               con.Open();
               string sql = @"
SELECT UserId, ExpiresAt FROM Tokens WHERE SessionToken = @SessionToken LIMIT 1;
               ";
               using(NpgsqlCommand cmd = new(sql,con))
                {
                    cmd.Parameters.AddWithValue("@SessionToken", token);
                       using (var reader = cmd.ExecuteReader()){
                  while(reader.Read() )
        {
            return new UserSession(
                userid: reader.GetInt32(0),
                expiresAt: reader.GetDateTime(1)
            );
        }
        return null;
        }

                } 


            }
        }
        public static void DeleteSessionToken(string token)
        {
            using NpgsqlConnection con = new(connectionstring);
con.Open();
string sql = @"
DELETE FROM Tokens WHERE SessionToken = @token; 
";
using NpgsqlCommand cmd = new(sql, con);
cmd.Parameters.AddWithValue("@token", token);
cmd.ExecuteNonQuery();
        }
        public static void DeletePreset(int presetid)
        {
            using NpgsqlConnection con = new(connectionstring);
con.Open();
string sql = @"
DELETE FROM Presets WHERE id = @presetName;
";
using NpgsqlCommand cmd = new(sql, con);
cmd.Parameters.AddWithValue("@presetName", presetid);
cmd.ExecuteNonQuery();
        }
        public static int GetAuthorId(int PresetId)
        {
         using NpgsqlConnection con=new(connectionstring);
         con.Open();
         string sql = @"
SELECT UserId
FROM Presets
WHERE Id = @PresetId;
";
using NpgsqlCommand cmd = new(sql, con);
cmd.Parameters.AddWithValue("@PresetId", PresetId);
object? result = cmd.ExecuteScalar();
return Convert.ToInt32(result);

        }
         public static PresetView[] GetPresetViewsOfAUser(int UserId)
        {
            List<PresetView> pvl = new();
           using NpgsqlConnection con = new(connectionstring);
           con.Open();
           
           string sql = @"
           SELECT Presets.Quizname, Presets.Id, Users.DisplayName 
           FROM Presets
           INNER JOIN Users ON Users.Id = Presets.UserId
           WHERE UserId = @UserId;
           ";
           using NpgsqlCommand cmd = new(sql, con);
           cmd.Parameters.AddWithValue("@UserId",UserId);
using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                PresetView pv = new();
                pv.preset = reader.GetString(0);
                pv.id = reader.GetInt32(1);
                pv.user = reader.GetString(2);
                pvl.Add(pv);
            }
            return pvl.ToArray();
}
public static void AddQuestionsList(AnswerMe[] questions, string towhere)
        {
            using (var connection = new NpgsqlConnection(connectionstring))
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
     using (var command = new NpgsqlCommand(sql, connection, trans))
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
        public static bool PresetNameExists(string name)
        {
             
        
         using NpgsqlConnection con=new(connectionstring);
         con.Open();
         string sql = @"
SELECT EXISTS(SELECT 1 FROM Presets WHERE QuizName = @name);
";
using NpgsqlCommand cmd = new(sql, con);
cmd.Parameters.AddWithValue("@name", name);
object result = cmd.ExecuteScalar();
                    if(result == null)
            {
             return false;   
            }
    return Convert.ToBoolean(result);
        }
        public static void AddFullPreset(Preset preset, int userid)
        {
            using NpgsqlConnection con=new(connectionstring);
            con.Open();
          string Writename = @"
    INSERT INTO Presets (QuizName, UserId) 
    VALUES (@PresetName, @UserId)
    RETURNING id
    ;
";
 string WriteAquestion = @"
 INSERT INTO Questions (PresetId,QuestionText,OptionsJson,CorrectAnswer)
VALUES (@PresetId,@QuestionText,@OptionsJson,@CorrectAnswer)

";
using (var transaction = con.BeginTransaction()){
try{
using NpgsqlCommand cmd = new(Writename, con,transaction);
cmd.Parameters.AddWithValue("@QuizName", preset.name);
cmd.Parameters.AddWithValue("@UserId", userid);
int targetid = Convert.ToInt32(cmd.ExecuteScalar());

using NpgsqlCommand cmd2 = new(WriteAquestion, con,transaction);
cmd2.Parameters.AddWithValue("@PresetId", targetid);
cmd2.Parameters.AddWithValue("@QuestionText", DBNull.Value);
cmd2.Parameters.AddWithValue("@OptionsJson", DBNull.Value);
cmd2.Parameters.AddWithValue("@CorrectAnswer", DBNull.Value);

foreach(AnswerMe question in preset.questions){
cmd2.Parameters["@QuestionText"].Value = question.name;
cmd2.Parameters["@OptionsJson"].Value = JsonSerializer.Serialize(question.options);
cmd2.Parameters["@CorrectAnswer"].Value = question.correct;
cmd2.ExecuteNonQuery();
}
transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
}

        }

    }
}
public record UserSession(int userid, DateTime expiresAt);