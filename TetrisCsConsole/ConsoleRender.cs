using System;
using System.Text;

namespace TetrisCsConsole
{
    public class ConsoleRender
    {
        private int gameRows;
        private int gameColumns;
        private int infoColumns;
        private int consoleRows;
        private int consoleColumns;

        public ConsoleRender(int gameRows = 20, int gameColumns = 10, int infoColumns = 11)
        {
            this.gameRows = gameRows;
            this.gameColumns = gameColumns;
            this.infoColumns = infoColumns;
            this.consoleRows = 1 + gameRows + 1;
            this.consoleColumns = 1 + gameColumns + 1 + infoColumns + 1;

            Console.Title = "Tetris in the Console";
            Console.CursorVisible = false;
            Console.SetWindowSize(consoleColumns, consoleRows + 1);
            Console.SetBufferSize(consoleColumns, consoleRows + 1);
        }

        public void RedrawUI(GameState state, ScoreManager manager)
        {
            this.DrawBorder();
            this.DrawInfo(3 + this.gameColumns, state, manager);
            this.DrawField(state.GameField);
            this.DrawTetromino(state.CurrentTetromino, state.TetrominoRow, state.TetrominoCol);
        }

        public void DrawInfo(int column, GameState state, ScoreManager manager)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            this.Write("Level:", column, 1);
            this.Write($"{state.Level} / 10", column, 2);

            this.Write("Score:", column, 4);
            this.Write(manager.Score.ToString(), column, 5);

            this.Write("Best:", column, 7);
            this.Write(manager.HighScore.ToString(), column, 8);

            this.Write("Frame:", column, 10);
            this.Write($"{state.Frame} / {state.MoveFrame - state.Level}", column, 11);

            this.Write("Pos:", column, 13);
            this.Write($"{state.TetrominoRow}, {state.TetrominoCol}", column, 14);

            this.Write("Controls:", column, 16);
            this.Write("   ^", column, 17);
            this.Write(" <   >", column, 18);
            this.Write("   V", column, 19);
            this.Write(" Space", column, 20);
        }

        public void DrawBorder()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.SetCursorPosition(0, 0);

            // Top Line
            StringBuilder topLine = new StringBuilder();
            topLine.Append('╔');
            topLine.Append('═', this.gameColumns);
            topLine.Append('╦');
            topLine.Append('═', this.infoColumns);
            topLine.Append('╗');
            Console.WriteLine(topLine);

            // Middle Lines
            for (int i = 0; i < this.gameRows; i++)
            {
                StringBuilder midLine = new StringBuilder();
                midLine.Append('║');
                midLine.Append(new string(' ', this.gameColumns));
                midLine.Append('║');
                midLine.Append(new string(' ', this.infoColumns));
                midLine.Append('║');
                Console.WriteLine(midLine);
            }

            // Bottom Line
            StringBuilder bottomLine = new StringBuilder();
            bottomLine.Append('╚');
            bottomLine.Append('═', this.gameColumns);
            bottomLine.Append('╩');
            bottomLine.Append('═', this.infoColumns);
            bottomLine.Append('╝');
            Console.WriteLine(bottomLine);
        }

        public void DrawGameOver(int score)
        {
            int row = (gameColumns + 3 + infoColumns) / 2 - 5;
            int col = gameRows / 2 - 3;

            string scoreAsString = score.ToString();
            scoreAsString += new string(' ', col + 2 - scoreAsString.Length);

            // Print border
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            this.Write("╔═════════╗", row, col);
            this.Write("║ ", row, col + 1); this.Write("    ║", row + 6, col + 1);
            this.Write("║  ", row, col + 2); this.Write("  ║", row + 8, col + 2);
            this.Write("║ ", row, col + 3); this.Write("  ║", row + 8, col + 3);
            this.Write("║ ", row, col + 4); this.Write("║", row + 10, col + 4);
            this.Write("╚═════════╝", row, col + 5);

            // Print game over
            Console.ForegroundColor = ConsoleColor.Red;
            this.Write("Game", row + 2, col + 1);
            this.Write("Over!", row + 3, col + 2);

            // Print score
            Console.ForegroundColor = ConsoleColor.Green;
            this.Write("Score:", row + 2, col + 3);
            this.Write($"{scoreAsString}", row + 2, col + 4);
        }

        public void DrawField(bool[,] field)    
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            for (int row = 0; row < field.GetLength(0); row++)
            {
                StringBuilder sb = new StringBuilder();
                for (int col = 0; col < field.GetLength(1); col++)
                {
                    if (field[row, col]) sb.Append("*");
                    else sb.Append(" ");
                }

                this.Write(sb.ToString(), 1, row + 1);
            }
        }

        public void DrawTetromino(Tetromino tetromino, int tetroRow, int tetroColumn)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            for (int row = 0; row < tetromino.Shape.GetLength(0); row++)
            {
                for (int col = 0; col < tetromino.Shape.GetLength(1); col++)
                {
                    if (tetromino.Shape[row, col])
                    {
                        Write("*", 1 + tetroColumn + col, row + 1 + tetroRow);
                    }
                }
            }
        }

        private void Write(string text, int col, int row)
        {
            Console.SetCursorPosition(col, row);
            Console.Write(text);
        }
    }
}
