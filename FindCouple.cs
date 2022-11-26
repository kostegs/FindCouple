using System;
using SFML;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Learning;
using System.Runtime.InteropServices;
using SFML.Window;

namespace Cards
{
    internal class FindCouple : Game
    {
       
        static void Main(string[] args)
        {
            Kernel kernel = Kernel.GetInstance();
            kernel.Initialize();
            kernel.SetGameMode(GameMode.Settings);            

            Screens.GetInstance().Initialize();

            while (true)
            {
                DispatchEvents();

                kernel.GameLogic();
                
                
                
                ClearWindow();
                kernel.Draw();
               
                DisplayWindow();

                Delay(1);
            }

            Console.WriteLine("Выиграл");
            Delay(5000);

        }
    }
}
