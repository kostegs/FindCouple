using SFML.Learning;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cards
{
    internal class Kernel : Game
    {
        public int Score { get; private set; } = 0;
        public int LastScoreRecord { get; private set; } = 0;
        public bool SetNewRecord = false;
        private Clock _clock;
        private Clock _timeLeftClock;
        private Kernel() {}
        private static Kernel Instance { get; set; }
        public static Kernel GetInstance()
        {
            if (Instance == null)
                Instance = new Kernel();

            return Instance;
        }
        internal string[] Textures { get; private set; }
        public void Initialize()
        {
            InitializeDifficultyLevel();
            LoadTextures();            
            _clock = new Clock();
            _clock.Restart();
            _timeLeftClock = new Clock();
            
            if (SetNewRecord)
                LastScoreRecord = Score;
            Score = 0;
            SetNewRecord = false;
            Cards.GetInstance().InitializeCards();
        }
        public void SetGameMode(GameMode gameMode)
        {
            Settings.lastGameMode = Settings.gameMode;
            Settings.gameMode = gameMode;
        }
        private void LoadTextures()
        {
            Textures = new string[7];
            Textures[0] = LoadTexture(@"./res/Icon_close.png");

            for (int i = 1; i < Textures.Length; i++)
            {
                Textures[i] = LoadTexture($@"./res/Icon_{i}.png");
            }
        }
        public void GameLogic()
        {
            Screens screens = Screens.GetInstance();

            if (Settings.gameMode == GameMode.PreviewCards && Settings.lastGameMode != GameMode.PreviewCards)
            {
                _clock.Restart();
                Settings.lastGameMode = GameMode.PreviewCards;
            } 

            if (Settings.gameMode == GameMode.Settings)
                screens.ProcessSettingsWindow();


            // After few seconds of preview we should close all cards
            if (Settings.gameMode == GameMode.PreviewCards && _clock.ElapsedTime.AsSeconds() >= Settings.timeForCardsPreview)
            {
                _clock.Restart();
                Cards.GetInstance().CloseAllCards();
                SetGameMode(GameMode.PlayMode);                
            }

            if (Settings.gameMode == GameMode.PlayMode)
            {
                if (Settings.lastGameMode != GameMode.PlayMode)
                {
                    _timeLeftClock.Restart();
                    Settings.lastGameMode = GameMode.PlayMode;
                }
                    

                if (GetLeftTime() == 0)
                    SetGameMode(GameMode.GameOver);
                else
                    Cards.GetInstance().ProcessCards();
            }
                
            else if (Settings.gameMode == GameMode.GameOver)
            {
                if (Settings.lastGameMode == GameMode.PlayMode)
                {
                    Settings.lastGameMode = GameMode.GameOver;  
                    
                    if (LastScoreRecord < Score)                    
                        SetNewRecord = true;
                }
                screens.ProcessGameOverWindow();
            }
        }
        public void Draw()
        {
            Screens screens = Screens.GetInstance();

            if (Settings.gameMode == GameMode.Settings)
                screens.DrawSettingsWindow();
            else if (Settings.gameMode == GameMode.PlayMode || Settings.gameMode == GameMode.PreviewCards)
                Cards.GetInstance().DrawCards();
            else if (Settings.gameMode == GameMode.GameOver)
                screens.DrawGameOverWindow();

            if (Settings.gameMode == GameMode.PlayMode)
            {
                screens.DrawScoreAndTimer();
            }
        }
        public int GetLeftTime()
        {
            float currentTime = _timeLeftClock.ElapsedTime.AsSeconds();
            float lastTime = (Settings.timeForLevel - currentTime);
            lastTime = lastTime < 0 ? 0 : lastTime;

            return (int)lastTime;
        }
        public void Exit()
        {
            System.Environment.Exit(0);
        }
        internal void SetScore(int multiply)
        {
            Score += 2 * multiply;
            Score = Score < 0 ? 0 : Score;
        }
        internal void InitializeDifficultyLevel()
        {
            switch (Settings.difficultyLevel)
            {
                case 1:
                    Settings.cardCount = 12;
                    Settings.timeForLevel = 40;
                    break;
                case 2:
                    Settings.cardCount = 16;
                    Settings.timeForLevel = 50;
                    break;
                case 3:
                    Settings.cardCount = 20;
                    Settings.timeForLevel = 60;
                    Settings.countPerLine = 5;
                    break;
            }
        }
    }
}
