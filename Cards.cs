using SFML.Learning;
using SFML.System;
using SFML.Window;
using System;
using System.Linq;

namespace Cards
{
    internal class Cards : Game
    {
        internal static int AmountCards { get; private set; }
        internal static int RemainingCards { get; private set; }
        internal static int OpenCardAmount { get; private set; }
        internal static int FirstOpenCardIndex { get; private set; }
        internal static int SecondOpenCardIndex { get; private set; }
        private bool isDelay = false;
        private Clock _clockForOpenCards = new Clock();
        private Clock _timeLeftClock = new Clock();
        private static string CardClickSound = LoadSound(@"./res/CardClick.ogg");
        private static string CardDestroySound = LoadSound(@"./res/DestroyCardSound.ogg");        

        private int[,] cards;
        private Cards() { }
        private static Cards Instance;

        public static Cards GetInstance()
        {
            if (Instance == null)
                Instance = new Cards();
            
            return Instance;
        }
        internal void InitializeCards()
        {
            cards = new int[Settings.cardCount, Settings.numbersOfTextures];
            int[] iconId = new int[cards.GetLength(0)];
            int[] usedIndexes = new int[Settings.numbersOfTextures];
            RemainingCards = Settings.cardCount;
            OpenCardAmount = 0;
            FirstOpenCardIndex = -1;
            SecondOpenCardIndex = -1;

            int id = 0;
            Random rnd = new Random();
            
            for (int i = 0; i < cards.GetLength(0); i++)
            {
                // Generate new pair of pictures
                if (i % 2 == 0)
                {
                    // Checking for repeated cards, because of randomizer. 
                    // For every step we check tmp-array "Used Indexes". If we find our number, we repeat this step in order to generate new digit. 
                    // If tmp-array is full, we clean it and continue algorythm. 
                    while (true)
                    {
                        id = rnd.Next(1, Settings.numbersOfTextures + 1); // +1 - for catching last texture

                        bool usedIndexesArrayIsFull = usedIndexes.Select(index => index).Where(index => index == 0).Count() == 0;
                        bool IndexWasUsed = false;

                        if (usedIndexesArrayIsFull)
                        {
                            for (int j = 0; j < usedIndexes.Length; j++)
                                usedIndexes[j] = 0;
                        }
                        else
                            IndexWasUsed = usedIndexes.Select(index => index).Where(index => index == id).Count() > 0;

                        if (!IndexWasUsed)
                        {
                            usedIndexes[id - 1] = id;
                            break;
                        }
                    }
                }

                iconId[i] = id;
            }

            ShuffleArrayOfIcons(iconId);

            for (int i = 0; i < cards.GetLength(0); i++)
            {
                cards[i, 0] = (int)State.Opened; // Default state = Opened
                cards[i, 1] = (i % Settings.countPerLine) * (Settings.cardWidth + Settings.space) + Settings.leftOffset; // posX
                cards[i, 2] = (i / Settings.countPerLine) * (Settings.cardHeight + Settings.space) + Settings.topOffset; // posY
                cards[i, 3] = Settings.cardWidth; // width
                cards[i, 4] = Settings.cardHeight; // height
                cards[i, 5] = iconId[i]; // id
            }

            for (int i = 0; i < cards.GetLength(0); i++)
            {
                Console.Write($"{cards[i, 5]} ");
            }
            _timeLeftClock.Restart();            
        }

        private void ShuffleArrayOfIcons(int[] array)
        {
            Random rnd = new Random();

            for (int i = array.Length - 1; i >= 1; i--)
            {
                int j = rnd.Next(1, i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }

        internal void CloseAllCards()
        {
            for (int i = 0; i < cards.GetLength(0); i++)
                SetStateToCard(i, State.Closed);
        }
        private void SetStateToCard(int index, State state)
        {
            cards[index, 0] = (int)state;
        }

        public void DrawCards()
        {
            Kernel kernel = Kernel.GetInstance();

            for (int i = 0; i < cards.GetLength(0); i++)
            {
                if (cards[i, 0] == (int)State.Opened) 
                {
                    DrawSprite(kernel.Textures[cards[i, 5]], cards[i, 1], cards[i, 2]);
                }

                else if (cards[i, 0] == (int)State.Closed) 
                {
                    DrawSprite(kernel.Textures[0], cards[i, 1], cards[i, 2]);
                }
            }
        }
        public void ProcessCards()
        {
            // Do nothing in Delay mode
            if (isDelay && _clockForOpenCards.ElapsedTime.AsSeconds() <= 1f)
                return;            

            if (RemainingCards == 0)
                Kernel.GetInstance().SetGameMode(GameMode.GameOver);

            if (OpenCardAmount == 2)
            {
                _clockForOpenCards.Restart();
                isDelay = !isDelay;

                if (isDelay)
                    return;

                // Same indexes
                if (cards[FirstOpenCardIndex, 5] == cards[SecondOpenCardIndex, 5])
                {
                   // Destory both cards
                   cards[FirstOpenCardIndex, 0] = (int)State.Destroyed;
                   cards[SecondOpenCardIndex, 0] = (int)State.Destroyed;
                   RemainingCards -= 2;
                   Kernel.GetInstance().SetScore(1);
                    PlaySound(CardDestroySound);
                }
                else
                {
                    cards[FirstOpenCardIndex, 0] = (int)State.Closed;
                    cards[SecondOpenCardIndex, 0] = (int)State.Closed;
                    Kernel.GetInstance().SetScore(-1);
                }

                OpenCardAmount = 0;
                FirstOpenCardIndex = -1;
                SecondOpenCardIndex = -1;
            }

            if (GetMouseButtonDown(Mouse.Button.Left))
            {
                int index = GetIndexCardByMousePosition();

                if (index != -1 && index != FirstOpenCardIndex && cards[index, 0] != (int)State.Destroyed)
                {
                    PlaySound(CardClickSound);
                    cards[index, 0] = (int)State.Opened;
                    OpenCardAmount++;

                    switch (OpenCardAmount)
                    {
                        case 1:
                            FirstOpenCardIndex = index;
                            break;
                        case 2:
                            SecondOpenCardIndex = index;
                            break;
                        default:
                            break;
                    }                    
                }
            }
        }

        int GetIndexCardByMousePosition()
        {
            for (int i = 0; i < cards.GetLength(0); i++)
            {
                if (MouseX >= cards[i, 1] && MouseX <= cards[i, 1] + cards[i, 3]
                    && MouseY >= cards[i, 2] && MouseY <= cards[i, 2] + cards[i, 4])
                {
                    return i;
                }
            }

            return -1;        
        }
    }
}