
namespace Cards
{
    enum State : int
    {
        Destroyed = -1,
        Closed = 0,
        Opened = 1        
    }
    enum GameMode : byte
    {
        Settings = 0,
        PreviewCards = 1,
        PlayMode = 2,
        GameOver = 3
    }
    internal struct Settings
    {
        internal static int cardCount = 12; // ONLY EVEN-NUMBERED!!!
        internal static int cardWidth = 100;
        internal static int cardHeight = 100;
        internal static int numbersOfTextures = 6; 
        internal static GameMode gameMode;
        internal static GameMode lastGameMode;
        internal static int difficultyLevel = 0;
        internal static int countPerLine = 4;
        internal static int space = 40;
        internal static int leftOffset = 70;
        internal static int topOffset = 60;
        internal static int timeForCardsPreview = 3; // seconds
        internal static int timeForLevel = 30; // seconds
    }
}
