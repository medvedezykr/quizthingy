using System.Text.RegularExpressions;
using QuizAPI;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;



var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
//-------------------------------------------
app.MapGet("/", () =>
{
    DBmng.InitializeDB();
    return "works!";        
});
//-------------------------------------------
app.MapPost("/postquiz", (HttpRequest request, Preset preset) =>
{

    string? authHeader = request.Headers["Authorization"];
    if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ")) 
    {
        return Results.Unauthorized();
    }

    string token = authHeader.Substring("Bearer ".Length);
    UserSession? serverSession = DBmng.GetIdAndExpTimeByToken(token);
    if (serverSession == null || serverSession.expiresAt < DateTime.UtcNow) 
    {
        return Results.Unauthorized();
    }

   DBmng.AddFullPreset(preset,serverSession.userid);

    return Results.Ok($"added {preset.name}!");
});

//-------------------------------------------
app.MapGet("/getquiz/{quiz}", (int quiz) =>
{
 Preset quiztoget = DBmng.GetQuiz(quiz); ;;
   return quiztoget;
});
//-------------------------------------------
app.MapGet("/showRiches(quizes)", () =>
{
    PresetView[] riches = DBmng.GetAllQuizNames();
    return riches;
});
//-------------------------------------------
app.MapPost("/register", (User user) =>
{
    string usernamecheck = @"^[a-zA-Z0-9_-]{3,16}$";
    if(!Regex.IsMatch(user.name, usernamecheck))
    {
ErrorResponse Error = new();
    Error.error = "IncorrectUsernameFormat";
    Error.message = "Password has to have an uppercase, a lowercase, a num, have at least 4 uniques characters and be 6 chars long at mininmum";
return Results.BadRequest(Error);
    }
    if (DBmng.IsNameUsed(user.name.ToLower()))
    {
        ErrorResponse Error = new();
        Error.error = "UsernameAlreadyTaken";
        Error.message = "Choosen username is already taken";
return Results.Conflict(Error);
        }
    DBmng.AddUser(user.name, user.password);
return Results.Ok("registered successfully");
});
//-------------------------------------------
 app.MapPost("/login", (LoginRequest login) =>
    {
        if(!DBmng.IsNameUsed(login.username)){
            ErrorResponse err = new()
            {
                error = "InvalidCredentials",
                message = "Provided name or password are incorrect"
            };
            return Results.Conflict(err);
        }
         int? id = DBmng.GetIdByUser(login.username.ToLower());
       if(id==null||id==0)
        {
            ErrorResponse err = new()
            {
                error = "InvalidCredentials",
                message = "Provided name or password are incorrect"
            };
            return Results.Conflict(err);
        }
        if (!DBmng.CheckPassword(login.username.ToLower(), login.password))
        {
            ErrorResponse err = new()
            {
                error = "InvalidCredentials",
                message = "Provided name or password are incorrect"
            };
            return Results.Conflict(err);
      }
      
        return Results.Ok(DBmng.CreateSessionToken(Convert.ToInt32(id)));
       });
//-------------------------------------------
app.MapPost("/logout", (HttpRequest request) =>
{
string? authHeader = request.Headers["Authorization"];
if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ")) 
    {
        return Results.Unauthorized();
    }

    string token = authHeader.Substring("Bearer ".Length);
    DBmng.DeleteSessionToken(token);
    return Results.Ok("logged out");

});
//-------------------------------------------
app.MapDelete("/deletepreset/{quizid}", (HttpRequest request,int quizid) =>
{
    string? authHeader = request.Headers["Authorization"];
if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ")) 
    {
        return Results.Unauthorized();
    }

    string token = authHeader.Substring("Bearer ".Length);
    (int userid, DateTime expiresat) = DBmng.GetIdAndExpTimeByToken(token);
   int actualluserId= DBmng.GetAuthorId(quizid);
   if(userid!= actualluserId) return Results.Unauthorized();
   DBmng.DeletePreset(quizid); 

return Results.Ok();
});
//-------------------------------------------
app.MapGet("/getuserspresets", (HttpRequest request) =>
{
     string? authHeader = request.Headers["Authorization"];
if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ")) 
    {
        return Results.Unauthorized();
    }

    string token = authHeader.Substring("Bearer ".Length);
(int userid, DateTime expiresat) = DBmng.GetIdAndExpTimeByToken(token);
PresetView  [] userspresets = DBmng.GetPresetViewsOfAUser(userid);
return Results.Ok(userspresets);
});
//-------------------------------------------

app.Run();

public class ErrorResponse{
    public string error{get;set;}=string.Empty;
    public string message{get;set;}=string.Empty;
}
public class LoginRequest
{
    public  string username{get;set;}
    public  string password{get;set;}
}
public class PresetWithUserResponse
    {
        public string preset {get;set;}  = string.Empty;
        public string user {get;set;} = string.Empty;
        public bool IsSucess {get;set;}
        
    }