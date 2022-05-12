using System;

namespace TetrisCsConsole
{
    public class GameState
    {
        public GameState(int rows, int columns)
        {
            this.GameField = new bool[rows, columns];

            this.Level = 1;
            this.Frame = 0;
            this.MoveFrame = 16;
            this.CurrentTetromino = null;
            this.TetrominoRow = 0;
            this.TetrominoCol = 0;
            this.LineScores = new int[] { 0, 40, 100, 300, 1200 };
        }

        public bool[,] GameField { get; private set; }

        public int Level { get; private set; }

        public int Frame { get; set; }

        public int MoveFrame { get; private set; }

        public Tetromino CurrentTetromino { get; set; }

        public int TetrominoRow { get; set; }

        public int TetrominoCol { get; set; }

        public int[] LineScores { get; private set; }

        public void UpdateLevel(int score)
        {
            if (score <= 0)
            {
                this.Level = 1;
                return;
            }

            this.Level = (int)Math.Log10(score) - 1;

            if (this.Level < 1)
            {
                this.Level = 1;
            }

            if (this.Level > 10)
            {
                this.Level = 10;
            }
        }
    }
}
