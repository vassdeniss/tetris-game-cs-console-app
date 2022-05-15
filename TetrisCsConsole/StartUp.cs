namespace TetrisCsConsole
{
    public static class StartUp
    {
        public static void Main()
        {
            int gameRows = 20;
            int gameColumns = 10;

            //new MusicPlayer().Play();

            GameEngine engine = new GameEngine(
                new GameLogic(gameRows, gameColumns),
                new ConsoleInputHandler(),
                new ConsoleRender(gameRows, gameColumns, tetroChar: '█'),
                new ScoreManager("YourHighScores.txt"));
            engine.Run();
        }
    }
}
