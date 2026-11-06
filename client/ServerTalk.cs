using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace QuizOnline {
     public class PresetView
    {
        public int id {get;set;}
        public string preset {get;set;}  = string.Empty;
        public string user {get;set;} = string.Empty;
        
    }
    public class ErrorResponse{
    public string error{get;set;}=string.Empty;
    public string message{get;set;}=string.Empty;
}
 public class TokenAndTime
    {
        public string token{get;set;}
       public DateTime expirsAt{get;set;}
    }
public class LoginRequest
{
    public string username{get;set;}
    public string password{get;set;}
}
   public static class ServerTalk
    {
          private static readonly string BaseUrl;

     
        static ServerTalk()
        {
            string exefold = AppDomain.CurrentDomain.BaseDirectory;
            string configPath = Path.Combine(exefold, "config.txt");
            string defaultAddress = "http://localhost:5231";

            if (!File.Exists(configPath))
            {
                try 
                {
                    File.WriteAllText(configPath, defaultAddress);
                }
                catch 
                {
                
                }
                
                BaseUrl = defaultAddress;
            }
            else
            {
                string fileContent = File.ReadAllText(configPath).Trim();
                BaseUrl = string.IsNullOrWhiteSpace(fileContent) ? defaultAddress : fileContent;
            }
        }
        public static async Task<(PresetView[]?, bool IsSuccess)> FetchQuiznames()
        {
            using HttpClient client = new();
            using HttpResponseMessage response = await client.GetAsync($"{BaseUrl}/showRiches(quizes)");
            PresetView[]? thingies = await response.Content?.ReadFromJsonAsync<PresetView[]>();

if(response.IsSuccessStatusCode) return (thingies, true);
    else return (null, false);
        }

        
        public static async Task<bool> PostQuiz(Preset quiz)
        {
            using HttpClient client = new();
string? token = DBmng.GetSessionToken();
            if (token == null)
            {
                return false;
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
            try{
            using HttpResponseMessage response = await client.PostAsJsonAsync($"{BaseUrl}/postquiz", quiz);
            response.EnsureSuccessStatusCode();
            return true;
            }
            catch
            {
                return false;
            }
        }
        public static async Task<Preset> GetQuiz(int which){
            using HttpClient client = new();
             using HttpResponseMessage response = await client.GetAsync($"{BaseUrl}/getquiz/{which}");
Preset quiz = await response.Content.ReadFromJsonAsync<Preset>();
 response.EnsureSuccessStatusCode();
return quiz;    
        }
        public static async Task<(bool IsSuccess, string ErrorMessage)> Register(User user)
        {
            using HttpClient client = new();
            using HttpResponseMessage response = await client.PostAsJsonAsync($"{BaseUrl}/register", user);
            if (response.IsSuccessStatusCode)
            {
                return (true, string.Empty);
            }
            else
            {
                ErrorResponse error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                return (false, error?.message??"Unexpected error"); 
            }
        }
public static async Task<(string? ErrorMessage, TokenAndTime? token)> Login(LoginRequest login)
        {
           using HttpClient cli = new();
using HttpResponseMessage response = await cli.PostAsJsonAsync($"{BaseUrl}/login", login);
            if (response.IsSuccessStatusCode)
            {
                TokenAndTime token = await response.Content.ReadFromJsonAsync<TokenAndTime>();
                return (null, token);
            }
            else
            {
                ErrorResponse err = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                return ( err?.message??"Unexpected error",null);
            }
        }
        public static async Task<bool> Logout()
        {
           using HttpClient cli = new();

           string? token = DBmng.GetSessionToken();
            if (token == null)
            {
                return false;
            }
            cli.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
            
           using HttpResponseMessage response = await cli.PostAsync($"{BaseUrl}/logout",null);
           if(response.IsSuccessStatusCode) return true;
           else return false;
        }
        public static async Task<bool> DeletePreset(int id)
        {
            using HttpClient cli = new();
             string? token = DBmng.GetSessionToken();
            if (token == null)
            {
                return false;
            }
             cli.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
            using HttpResponseMessage response = await cli.DeleteAsync($"{BaseUrl}/deletepreset/{id}");
            if(response.IsSuccessStatusCode) return true;
           else return false;
        }
        
        public static async Task<PresetView[]>? GetUsersPresets()
        {
            using HttpClient cli = new();
               string? token = DBmng.GetSessionToken();
            if (token == null)
            {
                return null;
            }
             cli.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
            using  HttpResponseMessage response = await cli.GetAsync($"{BaseUrl}/getuserspresets");
          PresetView[] presetViews = await response.Content.ReadFromJsonAsync<PresetView[]>();
          return presetViews;
           
        }
    }
}