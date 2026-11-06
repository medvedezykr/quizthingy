using System.Runtime.InteropServices;

namespace QuizOnline
{
    class Program
    {
         static async Task Main()
        {
             DBmng.InitializeDatabase();
            Console.WriteLine("funny quiz prog");
             bool safenet = true;
                 while(safenet){
 Console.WriteLine("whatchu wanna do?\n1.do quizes\n2.add/manage quizes\n3.server things");
 var initiallchoice = Console.ReadLine();
 switch (initiallchoice)
            {
                case "1":
 View.quiz();
                break;

                 case "2":
                 bool safenet2 = true;
                 while(safenet2){
                 Console.WriteLine("do what?\n 1. add questions\n2.remove questions\n3.remove presets\n4. add presets\n5. go back");
                 var answer = Console.ReadLine();

                    switch (answer)
                    {//1
                    case "1":
View.AddQuestion();

                    break;  
case "2":
View.DeleteQuestion();

                    break;
                    case "3":
View.DeletePreset();

                    break;
                       case "4":
View.AddPreset();

                    break;
                    case "5":
                    safenet2 = false;
                    break;
default:
Console.WriteLine("enter a correct number");
break;
                    }//safenet2
                    }
            break;//1

                
            
                 case "3":
                 
                
                 bool safenet3 = true;
                 while(safenet3){
                    string? session = DBmng.GetSessionToken();
                    if(session==null){
 Console.WriteLine("do what?\n1.upload quizes\n2.get quizes\n3.register\n4.login\n5.go back");
                    }
                    else  Console.WriteLine("do what?\n1.upload quizes\n2.get quizes\n3.register\n4.logout\n5.delete posted presets\n6. go back");
var answer = Console.ReadLine();
switch (answer)
{
case "1":
await View.UploadQuiz();

break;

case "2":
await View.GetQuizTuck();

break;

case "3":
await View.CreateAccount();

break;

case "4":
if(session==null)await View.Login();
else await View.Logout();
break;
case "5":
if(session!=null){
    await View.DeletePresetServer();
    
}
else safenet3 =false;
break;
case "6":
 if(session!=null)safenet3 =false;
 else Console.WriteLine("enter a valid number");

break;
}
                 }
                break;
                default:
Console.WriteLine("enter a valid number");
                break;
            }
            
            }//safenet1


        }

    }

}