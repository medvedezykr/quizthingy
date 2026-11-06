using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;
namespace QuizOnline
{
    public static class View
    {
        
                public static void quiz()
        {
            
            Console.WriteLine("which one? ('##' to go back)");
string[] names = DBmng.GetAllQuizNames();
int namescount = 0;
foreach(string name in names)
                        {
namescount++; 
Console.WriteLine($"{namescount}. {name}");
                        }
                        AnswerMe[] questions;
                        while(true){
                
string choice = Console.ReadLine();
if (choice == "##")return;   
if(!string.IsNullOrWhiteSpace(choice) && int.TryParse(choice, out int intchoice) && intchoice>0 && intchoice <=names.Count())
                {
                    questions =  DBmng.GetQuestionsForQuiz(DBmng.GetIdByNamePreset(names[intchoice-1]));
                   break;
                }
                else
                {
                    Console.WriteLine("invalid input");
                }
                        
}
int count = 0;
int optioncount = 0;
int asked = 0;
int correct=0;
foreach(AnswerMe question in questions)
            {
                count++;
                Console.WriteLine();
                Console.WriteLine($"{count}. Answer me, {question.name}");
                optioncount = 0;
                foreach(string option in question.options)
                {
                    optioncount++;
                    Console.WriteLine($"{optioncount}. {option}");
                }
                string answer = Console.ReadLine()?.Trim() ?? "";
                
                
                if(int.TryParse(answer, out int intanswer) && intanswer>0 && intanswer<=question.options.Count())
                {
                    if(question.options[intanswer-1] == question.correct)
                    {
                        Console.WriteLine("<------------>");
                        correct++;
                        asked++;
                    }
                    else if (question.options[intanswer-1] != question.correct)
                    {
                         Console.WriteLine("<------------>");
                        asked++;
                    }
                }
            }
            Console.WriteLine($"END! u answered {correct} outta {asked} right");
        }


        public static void AddPreset()
        {
            
            Console.WriteLine("Name the preset u wanna add OR type '##' to go back ");
            while(true){
           string name = Console.ReadLine();
            if(string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("invalid input, try again or '##' to go back");
            }
            else if(name == "##"){
                    break;
                }
            else{ 
                 DBmng.AddPreset(name); 
break;
            }

        }
        }
public static void AddQuestion()
        {
            AnswerMe questiontoadd = new AnswerMe();
            Console.WriteLine("question:");
            questiontoadd.name = Console.ReadLine();
            Console.WriteLine("options:");
            List<string> temporaryOptions = new List<string>();
string currentInput;
 Console.WriteLine("Enter an answer option ('##' to finish):");
while (true)
{
      currentInput = Console.ReadLine();
       if (currentInput == "##")
    {
        Console.WriteLine("Are you sure you want to keep this arrangement of options? (y/n, default: n)");
        string confirmation = Console.ReadLine();

        if (confirmation == "y")
        {
            questiontoadd.options = temporaryOptions.ToArray();
            
            break; 
        }
        else
        {
            Console.WriteLine("started over, enter new options");
            temporaryOptions.Clear();
            continue; 
        }
    }
    else
    {
        temporaryOptions.Add(currentInput);
    }



        }
        int count = 0;
 Console.WriteLine($"which option should be the correct one? ('##' to go back)");
foreach(string deargod in temporaryOptions)
                        {
                            count += 1;
                            Console.WriteLine($"{count}. {deargod}");

                        }
                      while(0==0){
                        string whichopshun= Console.ReadLine();
                        if(whichopshun == "##")return;
                        if(int.TryParse(whichopshun, out int intwhich)&&intwhich>0&&intwhich<=temporaryOptions.Count){
                        string choosenopshun = temporaryOptions[intwhich-1];
                        questiontoadd.correct = choosenopshun; 
                        break;
                        }
            else
            {
                Console.WriteLine("invalidinput");
            }
                      }

Console.WriteLine("where does it belong? OR '##' to go back");
int askercount = 0;
string[] quiznames =  DBmng.GetAllQuizNames();
     foreach(string deargod in quiznames)
                    {
                        askercount++;
                        Console.WriteLine($"{askercount}. {deargod}"); 
                    }
            while(1==1){
                    string whichquiz  =Console.ReadLine();
if(whichquiz == "##")break;
            if (int.TryParse(whichquiz, out int intwhichquiz)&&intwhichquiz>0&&intwhichquiz<=quiznames.Count())
            {
  DBmng.AddQuestion(questiontoadd, quiznames[intwhichquiz-1]);//something here returns null i think
 break;
            }
            else
            {
                Console.WriteLine("invalid input"); 
            }
            }


    }
public static void DeletePreset()
        {
            Console.WriteLine("what to delete?(num)");

string[] names =  DBmng.GetAllQuizNames();
            if(names == null || names.Count()== 0){
                 Console.WriteLine("theres nothing to delete");
                 return;
            }
int count = 0;
            foreach (string qname in names)
            {
                count++;
                Console.WriteLine($"{count}. {qname}");
            }
            string ChoosenOne= Console.ReadLine();
           
            if (int.TryParse(ChoosenOne, out int ChoosenOneint) && ChoosenOneint <= names.Count() && ChoosenOneint > 0)
            {
            string topass = names[ChoosenOneint-1];
            DBmng.DeleteAPresetInternal(topass);
            }
            else if(!string.IsNullOrWhiteSpace(ChoosenOne) && names.Contains(ChoosenOne))
            {
                DBmng.DeleteAPresetInternal(ChoosenOne);
            }
            else Console.WriteLine("this thingy does not exist!");
        }
 public static void DeleteQuestion()
        {
AnswerMe[] questions = DBmng.GetAllQuestions();
Console.WriteLine("enter an id to delete");
List<int> ids = new();
foreach(AnswerMeWithPreset question in questions)
            {
                Console.WriteLine($"id:{question.id}. {question.name}. (from:{question.PresetName})");
                ids.Add(question.id);
            }
            string choice = Console.ReadLine();
            if(int.TryParse(choice, out int intchoice))
            {
                if(ids.Contains(intchoice))
                {
                    DBmng.DeleteQuestion(intchoice);
                }
                else
                {
                    Console.WriteLine("this thingy does not exist");
                }
            }
            else
            {
                 Console.WriteLine("invaid input");
            }
        }
        public static async Task GetQuizTuck()
        {
            (PresetView[]? names, bool IsSuccess) = await ServerTalk.FetchQuiznames();
            if (!IsSuccess)
            {
                Console.WriteLine("there were troubles connecting to the server");
                return;
            }
            
int counter = 0 ;
Console.WriteLine("which?(num) or '##' to go back");
foreach( PresetView thing in names)
 {
  counter++;
            Console.WriteLine($"{counter}. {thing.preset} (by: {thing.user})");
        }
        while(true){
        string answer2 = Console.ReadLine();
        if(answer2== "##") break;
        if(int.TryParse(answer2, out int intanswer)&&intanswer>0&&intanswer<= names.Count()){
            Preset quiztoget = await ServerTalk.GetQuiz(names[intanswer-1].id);
            DBmng.AddPreset(quiztoget.name);
            DBmng.AddQuestionsList(quiztoget.questions, quiztoget.name);
            break;
        }
        else Console.WriteLine("invalid input");
        }
        }
         public static async Task UploadQuiz()
        {
            string[] names = DBmng.GetAllQuizNames();
            int count=0;
            Console.WriteLine("which? or '##' to go back");
            foreach(string name in names)
            {count++;
             Console.WriteLine($"{count}. {name}");
            }
            while(true){
string answer = Console.ReadLine();
if(answer == "##") break;
if(int.TryParse(answer, out int intanswer) && intanswer > 0 && intanswer <= names.Count())
            {
                Preset thingtopost = DBmng.GetQuiz(names[intanswer-1]);
                
                bool IsSuccess = await ServerTalk.PostQuiz(thingtopost);
                    if (!IsSuccess)
                    {
                        Console.WriteLine("unauthorizes");
                        continue;
                    }
                break;
            }
            else Console.WriteLine("invalid input");
            }
        }
       public static async Task CreateAccount()
        {
            User user = new();
            string usernamePattern = @"^[a-zA-Z0-9_-]{3,16}$";
        
            Console.WriteLine("username:");
while(1==1){
            string usrname = Console.ReadLine();
            if(Regex.IsMatch(usrname, usernamePattern))
                {
                    string Name = usrname;
                   user.name = usrname;
                    break;
                }
             else Console.WriteLine("Not valid username, allowed things:\n - Roman characters a-z/A-Z\n - nums 0-9\n - underscores '_'\n - dashes '-'");

            }
                        Console.WriteLine("create a password:");
            while (9 == 9)
            {
                string inppassword = Console.ReadLine();
                string croppedpassword = inppassword.Trim();
                bool eightchars = croppedpassword.Length >= 6 ;
                bool hasNum = croppedpassword.Any(c => char.IsDigit(c)); 
                bool haslower = croppedpassword.Any(c => char.IsLower(c));
                bool hasupper = croppedpassword.Any(c => char.IsUpper(c));
                bool fourunique = croppedpassword.Distinct().Count() >= 4;
                if (haslower && hasNum && hasupper && fourunique && eightchars)
                {
                    user.password = croppedpassword;
                    break;
                }
                else
                {
                     Console.WriteLine("has to have an uppercase, a lowercase, a num, have at least 4 uniques characters and be 6 chars long at mininmum");
                }
            }
var result = await ServerTalk.Register(user);
            if (result.IsSuccess) Console.WriteLine(" SUCCESS: Account created!");
            else Console.WriteLine($" FAILURE: {result.ErrorMessage}");
        }
        public static async Task Login()
        {
            
            LoginRequest loginRequest = new();
            while(true){
             Console.WriteLine("enter a username u want to log in as:");
            while(true){
           

           string input = Console.ReadLine().Trim();
            if (string.IsNullOrWhiteSpace(input))
            {
             Console.WriteLine("invalid input");
            }
             loginRequest.username = input;
             break;
            }
              while(true){
            Console.WriteLine($"password for {loginRequest.username}:");

           string input = Console.ReadLine().Trim();
            if (string.IsNullOrWhiteSpace(input))
            {
             Console.WriteLine("invalid input");
            }
             loginRequest.password = input;
             break;
            }

            (string? err,TokenAndTime? token) = await ServerTalk.Login(loginRequest);
            if (err != null)
            {
                Console.WriteLine(err);
                loginRequest.username =null;
                loginRequest.password=null;
                continue;
            }
            DBmng.WipeTokens();
            DBmng.AddToken(token);
            break;
            }
}
public static async Task Logout()
        {
            bool SuccessfulyLogedOut = await ServerTalk.Logout();
            if(!SuccessfulyLogedOut)
            { Console.WriteLine("nothing to log out from");
             return;
            }
            DBmng.WipeTokens();
            Console.WriteLine("logged out sucessufuly");
        }
        public static async Task DeletePresetServer()
        {
            
            PresetView[]? presets = await ServerTalk.GetUsersPresets();
            if( presets==null)
            {
                 Console.WriteLine("there were troubles connecting");
                 return;
            }
if( presets.Count() ==0)
            {
                 Console.WriteLine("u dont have anything posted");
                 return;
            }
            Console.WriteLine("which one?");
            int count =0;
            foreach(PresetView preset in presets)
            {
                count++;
                Console.WriteLine($"{count}. {preset.preset} (by: {preset.user})");
            }
            while(true){
            string answer = Console.ReadLine();
            int validanswer;
            if(!int.TryParse(answer, out validanswer)|| validanswer<0 ||validanswer> presets.Count()){
Console.WriteLine("invalid input");
continue;
            }
           bool IsSuccessuful = await ServerTalk.DeletePreset(presets[validanswer-1].id);
if(!IsSuccessuful) Console.WriteLine("sometihng went wrong");
break;
            }
        }
}
}