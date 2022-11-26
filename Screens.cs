using SFML.Graphics;
using SFML.Learning;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cards
{
    internal class Screens : Game
    {
        private string textureText;
        private string chooseIconTexture;
        private Dictionary<int, int> DifficultyLevelCoordinates = new Dictionary<int, int>() {
            {1, 270},
            {2, 310},
            {3, 350}
        };               

        private int _tmpDifficultyLevel = 1;
        private Screens() {}
        private static Screens Instance { get; set; }
        public static Screens GetInstance()
        {
            if (Instance == null)
                Instance = new Screens();

            return Instance;
        }
        public void Initialize()
        {
            InitWindow(800, 600, "Find Couple");
            SetFont(@"./res/comic.ttf");
            textureText = LoadTexture(@"./res/CheckPair_Text.png");
            chooseIconTexture = LoadTexture(@"./res/ChooseIcon.png");            
        }
        public void DisposeTextures()
        {
            textureText = null;
            chooseIconTexture = null;
        }
        public void ProcessSettingsWindow()
        {
            if (GetKeyDown(Keyboard.Key.S) || GetKeyDown(Keyboard.Key.Down))
                _tmpDifficultyLevel = _tmpDifficultyLevel < 3 ? _tmpDifficultyLevel + 1 : _tmpDifficultyLevel;
            else if (GetKeyDown(Keyboard.Key.W) || GetKeyDown(Keyboard.Key.Up))
                _tmpDifficultyLevel = _tmpDifficultyLevel > 1 ? _tmpDifficultyLevel - 1 : _tmpDifficultyLevel;

            if (Keyboard.IsKeyPressed(Keyboard.Key.Return))
                Settings.difficultyLevel = _tmpDifficultyLevel;
            else if (Keyboard.IsKeyPressed(Keyboard.Key.P))
            {
                Kernel.GetInstance().Initialize();
                Kernel.GetInstance().SetGameMode(GameMode.PreviewCards);
            }                
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
                Kernel.GetInstance().Exit();
        }
        public void DrawSettingsWindow()
        {

            void WriteColoredTextSettingsWindow(int difLevel, string textOfDifLevel)
            {
                if (Settings.difficultyLevel == difLevel)
                    SetFillColor(Color.Red);
                DrawText(270, DifficultyLevelCoordinates[difLevel], textOfDifLevel, 30);
                SetFillColor(Color.White);
            }

            DrawSprite(textureText, 160, 60);
            DrawText(200, 200, "Выберите уровень сложности:", 35);

            WriteColoredTextSettingsWindow(1, "Легкий");
            WriteColoredTextSettingsWindow(2, "Средний");
            WriteColoredTextSettingsWindow(3, "Сложный");

            DrawSprite(chooseIconTexture, 200, DifficultyLevelCoordinates[_tmpDifficultyLevel]);

            if (Settings.difficultyLevel != 0)
            {
                DrawText(140, 450, "Для старты игры нажмите P", 25);
                DrawText(140, 480, "Для выхода из игры нажмите Q", 25);
            }
        }
        public void ProcessGameOverWindow()
        {
            
        }
        public void DrawGameOverWindow()
        {
            string backgroundTexture = LoadTexture(@"./res/GameOverTexture.png");
            Kernel kernel = Kernel.GetInstance();
            
             // New Game
             if (Keyboard.IsKeyPressed(Keyboard.Key.R))
            {
                kernel.Initialize();
                kernel.SetGameMode(GameMode.PreviewCards);
                return;
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
                System.Environment.Exit(0);

            ClearWindow(SFML.Graphics.Color.Black);

            DrawSprite(backgroundTexture, 10, 10);

            SetFillColor(255, 255, 255);
            DrawText(240, 270, $"Партия игры завершена!", 20);
            DrawText(240, 300, $"Текущие очки: {kernel.Score}", 20);
            DrawText(240, 330, $"Рекорд по очкам: {kernel.LastScoreRecord}", 20);

            if (kernel.SetNewRecord)
                DrawText(240, 370, $"Поздравляю! Вы установили новый рекорд!", 20);

            DrawText(240, 420, "Нажмите R для рестарта игры", 20);
            DrawText(240, 440, "Нажмите Q для выхода из игры", 20);                            
        }
        public void DrawScoreAndTimer()
        {
            SetFillColor(Color.White);
            DrawText(20, 20, $"Оставшееся время: {Kernel.GetInstance().GetLeftTime()}");
            DrawText(20, 40, $"Текущие очки: {Kernel.GetInstance().Score}");
        }
    }
}
