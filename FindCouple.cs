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
        static string[] iconsName;
        static int[,] cards;
        static int[] usedIndexes;
        static int cardCount = 12;
        static int cardWidth = 100;
        static int cardHeight = 100;

        static int countPerLine = 4;
        static int space = 40;
        static int leftOffset = 70;
        static int topOffset = 20;

        static void LoadIcon()
        {
            iconsName = new string[6];
            iconsName[0] = LoadTexture(@"./res/Icon_close.png");

            for (int i = 1; i < iconsName.Length; i++)
            {
                iconsName[i] = LoadTexture($@"./res/Icon_{i}.png");
            }
        }

        static void Shuffle(int[] arr)
        {

            Random rnd = new Random();

            for (int i = arr.Length - 1; i >= 1; i--)
            {
                int j = rnd.Next(1, i + 1);

                int tmp = arr[j];
                arr[j] = arr[i];
                arr[i] = tmp;
            }

        }
        static void InitCard()
        {
            Random rnd = new Random();
            cards = new int[cardCount, 6];

            int[] iconId = new int[cards.GetLength(0)];
            int id = 0;

            for (int i = 0; i < iconId.Length; i++)
            {
                if (i % 2 == 0)
                {
                    while (true)
                    {
                        id = rnd.Next(1, 6);

                        bool indexArrayIsFilled = usedIndexes.Select(index => index).Where(index => index == 0).Count() == 0;
                        bool IndexWasUsed = false;

                        if (indexArrayIsFilled)
                        {
                            for (int j = 0; j < usedIndexes.Length; j++)
                                usedIndexes[j] = 0;
                        }
                        else                        
                            IndexWasUsed = usedIndexes.Select(index => index).Where(index => index == id).Count() > 0;

                        if (!IndexWasUsed)
                        {
                            usedIndexes[id-1] = id;
                            break;
                        }
                            

                        // !!! bad moment
                        // Only 2 cards must be equal
                        // Сделать массив UsedCards, туда добавлять сгенереную карту и в нем как раз проверять,
                        // если весь заполнили, то чистим и заново


                        /*var count = iconId.Select(card => card).Where(card => card == id).Count();

                        if (count < 2)
                            break;*/
                    }                    
                }
                    

                iconId[i] = id;
            }

            Shuffle(iconId);
           
            for (int i = 0; i < cards.GetLength(0); i++)
            {
                cards[i, 0] = 1; // state
                cards[i, 1] = (i % countPerLine) * (cardWidth + space) + leftOffset; // posX
                cards[i, 2] = (i / countPerLine) * (cardHeight+ space) + topOffset; // posY
                cards[i, 3] = cardWidth; // width
                cards[i, 4] = cardHeight; // height
                cards[i, 5] = iconId[i]; // id
            }

            for (int i = 0; i < cards.GetLength(0); i++)
            {             
                Console.Write($"{cards[i, 5]} ");
            }

        }

        static void DrawCards()
        {
            for (int i = 0; i < cards.GetLength(0); i++)
            {
                if (cards[i,0] == 1) // open
                {
                    DrawSprite(iconsName[cards[i,5]], cards[i,1], cards[i,2]);

            /*        if (cards[i, 5] == 1) { SetFillColor(0,   100, 0); FillRectangle(cards[i, 1], cards[i, 2], cards[i, 3], cards[i, 4]); }
                    if (cards[i, 5] == 2) { SetFillColor(0,   100, 100); FillRectangle(cards[i, 1], cards[i, 2], cards[i, 3], cards[i, 4]); }
                    if (cards[i, 5] == 3) { SetFillColor(0,   0,   100); FillRectangle(cards[i, 1], cards[i, 2], cards[i, 3], cards[i, 4]); }
                    if (cards[i, 5] == 4) { SetFillColor(100, 100, 0); FillRectangle(cards[i, 1], cards[i, 2], cards[i, 3], cards[i, 4]); }
                    if (cards[i, 5] == 5) { SetFillColor(100, 100, 100); FillRectangle(cards[i, 1], cards[i, 2], cards[i, 3], cards[i, 4]); }
                    if (cards[i, 5] == 6) { SetFillColor(100, 0,   0); FillRectangle(cards[i, 1], cards[i, 2], cards[i, 3], cards[i, 4]); } */
                } 

                else if (cards[i,0] == 0) // close
                {
                    DrawSprite(iconsName[0], cards[i, 1], cards[i, 2]);
                    /*SetFillColor(30, 30, 30);
                    FillRectangle(cards[i, 1], cards[i, 2], cards[i, 3], cards[i, 4]);*/
                }                 
            }
        }

        static int GetIndexCardByMousePosition()
        {
            for (int i = 0; i < cards.GetLength(0); i++)
            {
                if (MouseX >= cards[i,1] && MouseX <= cards[i,1] + cards[i,3]
                    && MouseY >= cards[i, 2] && MouseY <= cards[i, 2] + cards[i, 4])
                {
                    return i;
                }
            }

            return -1;
        }

        static void SetStateToAllCards(int state)
        {
            for (int i = 0; i < cards.GetLength(0); i++)
                cards[i, 0] = state;
        }

        static void Main(string[] args)
        {
            int openCardAmount = 0;
            int firstOpenCardIndex = -1;
            int secondOpenCardIndex = -1;
            int remainingCard = 0;
            usedIndexes = new int[5];

            LoadIcon();
            InitWindow(800, 600, "Find Couple");

            InitCard();
            remainingCard = cards.GetLength(0);
            SetStateToAllCards(1);

            DrawCards();
            DisplayWindow();

            Delay(5000);

            SetStateToAllCards(0);          

            while (true)
            {
                DispatchEvents();

                if (remainingCard == 0)
                    break;

                if (openCardAmount == 2)
                {
                    if (cards[firstOpenCardIndex, 5] == cards[secondOpenCardIndex, 5])
                    {
                        cards[firstOpenCardIndex, 0] = -1;
                        cards[secondOpenCardIndex, 0] = -1;
                        remainingCard -= 2;
                    }
                    else
                    {
                        cards[firstOpenCardIndex, 0] = 0;
                        cards[secondOpenCardIndex, 0] = 0;

                    }

                    openCardAmount = 0;
                    firstOpenCardIndex = -1;
                    secondOpenCardIndex = -1;

                    Delay(2000);
                }

                if (GetMouseButtonDown(Mouse.Button.Left))
                {
                    int index = GetIndexCardByMousePosition();

                    if (index != -1 && index != firstOpenCardIndex && cards[index, 0] != -1)
                    {
                        cards[index, 0] = 1;
                        openCardAmount++;

                        if (openCardAmount == 1)
                            firstOpenCardIndex = index;

                        if (openCardAmount == 2) 
                            secondOpenCardIndex = index;
                    }
                }
                    
                ClearWindow();
                DrawCards();
                DisplayWindow();

                Delay(1);
            }

            Console.WriteLine("Выиграл");
            Delay(5000);

        }
    }
}
