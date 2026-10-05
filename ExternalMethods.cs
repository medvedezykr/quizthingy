
using System.ComponentModel;
using System.Net.Quic;
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
           
             
     int askercount =0;
   
     List<string> quiznames = DBmng.GetAllQuizNames();
            if (quiznames.Count == 0 || quiznames == null)
            {
                Console.WriteLine("Theres no quizes to run");
                return;
            }

      Console.WriteLine("quizes, quizes, select.");
     foreach(string deargod in quiznames)
                    {
                        askercount++;
                        Console.WriteLine($"{askercount}. {deargod}");
                    }

                    int whichquizname = int.Parse(Console.ReadLine());
                    bool exists = false;    
                     if (whichquizname <= quiznames.Count && whichquizname > 0)
                {
                    exists = true;
                }

 
               
            

if(exists){
    string thingtopass = DBmng.GetIdByNamePreset(quiznames[whichquizname-1]);
List<AnswerMe> choosenquizquestions = DBmng.GetQuestionsForQuiz(thingtopass); //somethings
  int asked = 0;
     int correct=0;
foreach(AnswerMe deargod in choosenquizquestions)
                    {
                       int askercount1 =-1;
                       Console.WriteLine($"Answer me, Jia Baoyu, {deargod.name}"); 
                       foreach(string uwaaah in deargod.options){
askercount1++;
        Console.WriteLine($"{askercount1+1}. {deargod.options[askercount1]}"); //
        
       
                    }
                     
                    string answer = Console.ReadLine().ToLower();
                    string cleanAnswer = answer?.Trim() ?? "";
                   bool isNumber = int.TryParse(answer, out int intanswer);
            

                     if ( isNumber && deargod.options[intanswer-1] == deargod.correct )//figure out how to make it accept names AND numbers
                    {
                        
                        Console.WriteLine("<------------>");
                        correct++;
                        asked++;
                    }
                    else if(!string.IsNullOrWhiteSpace(cleanAnswer) && cleanAnswer.ToLower() == deargod.correct.ToLower()  )
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
        
}
else Console.WriteLine("this thingy does not exist!!");
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
            string ChoosenOne= Console.ReadLine();
           
            if (int.TryParse(ChoosenOne, out int ChoosenOneint) && ChoosenOneint <= names.Count && ChoosenOneint > 0)
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
                     Console.WriteLine("another one?(y/n, default:n)");
                     var answer1 = Console.ReadLine();
if(answer == "y") continue;
else return;
                          
            }
                }       
            else return;
                
            
            
        }
       public static void RemoveQuestion()
        {List<AnswerMe> questions = DBmng.GetAllQuestions();
        if (questions == null || questions.Count ==0 )
        {Console.WriteLine("there is no questions");return;}

             Console.WriteLine("which one to delete?:");
             int count = 0;
foreach(AnswerMe question in questions)
            {
                count++;
                Console.WriteLine($"{count}. id: {question.id}. {question.name}   (from:{ DBmng.GetPresetNameOfAQuestion(question.id)})  ");
            }
            string choosenone = Console.ReadLine();
            if (int.TryParse(choosenone, out int inttowork))
            {
                if(inttowork > 0 && inttowork<= count)
                {
                    DBmng.RemoveQuestionInternal(questions[inttowork-1].name);
                }
                else
                {
                    Console.WriteLine("this tihngy doesnt exist!");
                }
            
            
                
            }
else Console.WriteLine("enter an id");
                







        }





  
  }
}