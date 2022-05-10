using System;

namespace TetrisCsConsole
{
    public class GameState
    {
        public GameState(int rows, int columns)
        {
            GameField = new bool[rows, columns];

            HighScore = 0;
            Score = 0;
            Level = 1;
            Frame = 0;
            MoveFrame = 16;
            CurrentTetromino = null;
            TetrominoRow = 0;
            TetrominoCol = 0;
            LineScores = new int[] { 0, 40, 100, 300, 1200 };
        }

        public bool[,] GameField { get; private set; }

        public int HighScore { get; set; }

        public int Score { get; set; }

        public int Level { get; private set; }

        public int Frame { get; set; }

        public int MoveFrame { get; private set; }

        public bool[,] CurrentTetromino { get; set; }

        public int TetrominoRow { get; set; }

        public int TetrominoCol { get; set; }

        public int[] LineScores { get; private set; }

        public void UpdateLevel()
        {
            if (Score <= 0)
            {
                Level = 1;
                return;
            }

            Level = (int)Math.Log10(Score) - 1;

            if (Level < 1)
            {
                Level = 1;
            }

            if (Level > 10)
            {
                Level = 10;
            }
        }
    }
}
