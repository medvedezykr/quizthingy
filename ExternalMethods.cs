
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Mainquizthing{
  public static class externalmethods{
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
 Console.WriteLine($"which option should be the correct one?:");
foreach(string deargod in temporaryOptions)
                        {
                            count += 1;
                            Console.WriteLine($"{count}. {deargod}");

                        }
                        int whichopshun= int.Parse(Console.ReadLine());
                        string choosenopshun = temporaryOptions[whichopshun-1];



questiontoadd.correct = choosenopshun;
Console.WriteLine("where does it belong?");
int askercount = 0;
List<string> quiznames = DBmng.GetAllQuizNames();
     foreach(string deargod in quiznames)
                    {
                        askercount++;
                        Console.WriteLine($"{askercount}. {deargod}"); 
                    }
int whichquizname1 = int.Parse(Console.ReadLine());

DBmng.AddQuestionInternal(questiontoadd,  DBmng.GetIdByNamePreset(quiznames[whichquizname1-1]));

    }



 public static void quiz()
        {
              Console.WriteLine("quizes, quizes, select.");
     int askercount =0;
   
     List<string> quiznames = DBmng.GetAllQuizNames();
     foreach(string deargod in quiznames)
                    {
                        askercount++;
                        Console.WriteLine($"{askercount}. {deargod}");
                    }

                    int whichquizname = int.Parse(Console.ReadLine());
string thingtopass = DBmng.GetIdByNamePreset(quiznames[whichquizname-1]);
/*int count=-1;
bool exists = false;
foreach( string thing in quiznames)
            {
                count++;
                if (thingtopass== DBmng.GetIdByNamePreset(quiznames[count]))
                {
                    exists = true;
                }
            } */

//if(exists){
List<AnswerMe> choosenquizquestions = DBmng.GetQuestionsForQuiz(thingtopass); //somethings
  int asked = 0;
     int correct=0;
foreach(AnswerMe deargod in choosenquizquestions)
                    {
                       int askercount1 =-1;
                       Console.WriteLine($"Answer me, Jia Baoyu,{deargod.name}"); 
                       foreach(string uwaaah in deargod.options){
askercount1++;
        Console.WriteLine($"{askercount1+1}. {deargod.options[askercount1]}"); //
        
       
                    }
                    string answer = Console.ReadLine().ToLower();
                     if (answer == deargod.correct.ToLower())
                    {
                        Console.WriteLine("<------------>");
                        correct++;
                        asked++;
                    }
                    else
                    {
                         Console.WriteLine("<------------>");
                        asked++;
                    }
                    }
                    Console.WriteLine($"answered: {correct} out of {asked} right");
        
//}
//else Console.WriteLine("this thingy does not exist!!");
        }
        
public static void RemovePresetExternal()
        {
            Console.WriteLine("what to delete?(num)");

List<string> names =  DBmng.GetAllQuizNames();
int count = 0;
            foreach (string qname in names)
            {
                count++;
                Console.WriteLine($"{count}. {qname}");
            }
            int ChoosenOne= int.Parse(Console.ReadLine());
            string topass = names[ChoosenOne-1];
            DBmng.DeleteAPresetInternal(topass);

        }
public static void AddPresetExternal()
        {
            Console.WriteLine("Enter a name of a preset to add:");
            string presetname = Console.ReadLine();
            DBmng.AddQuizPreset(presetname);
            Console.WriteLine("wanna add questions to it right away?(y/n, default:n)");
            var answer = Console.ReadLine();
if(answer == "y")
            {
                while (true)
                {
                    AddQuestion();
                     Console.WriteLine("another one?");
                     var answer1 = Console.ReadLine();
if(answer == "y") continue;
else return;
                          
            }
                }       
            else return;
                
            
            
        }
       public static void RemoveQuestion()
        {List<AnswerMe> questions = DBmng.GetAllQuestions();
             Console.WriteLine("which one to delete?:");
foreach(AnswerMe question in questions)
            {
                Console.WriteLine($"id: {question.id}. {question.name}   (from:{ DBmng.GetPresetNameOfAQuestion(question.id)})  ");
            }
            int choosenone = int.Parse(Console.ReadLine());
DBmng.RemoveQuestionInternal(choosenone);

        }





  
  }
}