using System;
using System.Collections.Generic;
using System.Data;
using SQLitePCL;



namespace Mainquizthing
{
    class Program
    {
        static void Main()
        {
             DBmng.InitializeDatabase();
            Console.WriteLine("funny quiz prog");
             bool safenet = true;
                 while(safenet){
 Console.WriteLine("whatchu wanna do?\n1.do quizes\n2.add/manage quizes\n3.explode");
 var initiallchoice = Console.ReadLine();
 switch (initiallchoice)
            {
                case "1":
 externalmethods.quiz();
                break;

                 case "2":
                 bool safenet2 = true;
                 while(safenet2){
                 Console.WriteLine("do what?\n 1. add questions\n2.remove questions\n3.remove presets\n4. add presets");
                 var answer = Console.ReadLine();

                    switch (answer)
                    {//1
                    case "1":
externalmethods.AddQuestion();
safenet2= false;
                    break;  
case "2":
externalmethods.RemoveQuestion();
safenet2= false;
                    break;
                    case "3":
externalmethods.RemovePresetExternal();
                    break;
                       case "4":
externalmethods.AddPresetExternal();
                    break;
default:
Console.WriteLine("enter a correct number");
break;
                    }//safenet2
                    }
            break;//1

                
            
                 case "3":

                break;
                default:
Console.WriteLine("enter a valid number");
                break;
            }
            
            }//safenet1


        }
      

}
}

