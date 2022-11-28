using SFML.Learning;

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
        }
    }
}
