using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TetrisCsConsole
{
    internal class Program
    {
        // Settings
        static readonly int GameFieldRows = 20;
        static readonly int GameFieldCols = 10;
        static readonly int InfoFieldCols = 10;
        static readonly int ConsoleHeight = 1 + GameFieldRows + 1;
        static readonly int ConsoleWidth = 1 + GameFieldCols + 1 + InfoFieldCols + 1;
        static readonly List<bool[,]> Tetrominos = new List<bool[,]>()
        {
            new bool[,] // I
            {
                { true, true, true, true }
            },
            new bool[,] // O
            {
                { true, true },
                { true, true }
            },
            new bool[,] // T
            {
                { false, true, false },
                { true, true, true }
            },
            new bool[,] // S
            {
                { false, true, true },
                { true, true, false }
            },
            new bool[,] // Z
            {
                { true, true, false },
                { false, true, true }
            },
            new bool[,] // J
            {
                { true, false, false },
                { true, true, true }
            },
            new bool[,] // L
            {
                { false, false, true },
                { true, true, true }
            }
        };

        // States 
        static readonly GameState State = new GameState(GameFieldRows, GameFieldCols);
        static readonly Random RandomTetromino = new Random();

        static void Main()
        {
            MusicPlayer player = new MusicPlayer();
            player.Play();

            ScoreManager manager = new ScoreManager("YourHighScores.txt");
            State.HighScore = manager.GetHighScore();

            Console.Title = "Tetris in the Console";
            Console.CursorVisible = false;
            Console.SetWindowSize(ConsoleWidth, ConsoleHeight + 1);
            Console.SetBufferSize(ConsoleWidth, ConsoleHeight + 1);

            State.CurrentTetromino = Tetrominos[RandomTetromino.Next(0, Tetrominos.Count)];
            while (true)
            {
                State.Frame++;
                State.UpdateLevel();

                // User Input
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();

                    switch (key.Key)
                    {
                        case ConsoleKey.Escape:
                            return;
                        case ConsoleKey.W:
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.Spacebar:
                            RotateTetromino();
                            break;
                        case ConsoleKey.S:
                        case ConsoleKey.DownArrow:
                            State.TetrominoRow++;
                            State.Frame = 1;
                            State.Score += State.Level;
                            break;
                        case ConsoleKey.A:
                        case ConsoleKey.LeftArrow:
                            if (State.TetrominoCol > 0 && !CollisionSideLeft()) State.TetrominoCol--;
                            break;
                        case ConsoleKey.D:
                        case ConsoleKey.RightArrow:
                            if (State.TetrominoCol < GameFieldCols - State.CurrentTetromino.GetLength(1) && !CollisionSideRight())
                                State.TetrominoCol++;
                            break;
                    }
                }

                // Update Game State
                if (State.Frame % (State.MoveFrame - State.Level) == 0)
                {
                    State.TetrominoRow++;
                    State.Frame = 0;
                }

                if (Collision(State.CurrentTetromino))
                {
                    AddToGameField();
                    int lines = CheckFullLines();
                    State.Score += State.LineScores[lines];
                    State.CurrentTetromino = Tetrominos[RandomTetromino.Next(0, Tetrominos.Count)];
                    State.TetrominoCol = 0;
                    State.TetrominoRow = 0;

                    if (Collision(State.CurrentTetromino))
                    {
                        manager.PostHighScore(State.Score);
                        PrintGameOver();
                        Thread.Sleep(100000);
                        return;
                    }
                }

                // Update UI
                DrawBorder();
                DrawInfo();
                DrawField();
                DrawTetromino();
                Thread.Sleep(50);
            }
        }

        static bool CollisionSideLeft()
        {
            for (int row = 0; row < State.CurrentTetromino.GetLength(0); row++)
            {
                for (int col = 0; col < State.CurrentTetromino.GetLength(1); col++)
                {
                    if (State.TetrominoCol + col - 1 > -1 && State.TetrominoCol + col + 1 < State.GameField.GetLength(1))
                    {
                        if (State.GameField[State.TetrominoRow + row, State.TetrominoCol + col - 1])
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        static bool CollisionSideRight()
        {
            for (int row = 0; row < State.CurrentTetromino.GetLength(0); row++)
            {
                for (int col = 0; col < State.CurrentTetromino.GetLength(1); col++)
                {
                    if (State.TetrominoCol + col - 1 > -1 && State.TetrominoCol + col + 1 < State.GameField.GetLength(1))
                    {
                        if (State.GameField[State.TetrominoRow + row, State.TetrominoCol + col + 1])
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        static bool Collision(bool[,] tetromino)
        {
            if (State.TetrominoCol > GameFieldCols - tetromino.GetLength(1)) return true;

            if (State.TetrominoRow + tetromino.GetLength(0) == GameFieldRows) return true;

            for (int row = 0; row < tetromino.GetLength(0); row++)
            {
                for (int col = 0; col < tetromino.GetLength(1); col++)
                {
                    if (tetromino[row, col] && State.GameField[State.TetrominoRow + row + 1, State.TetrominoCol + col])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static int CheckFullLines()
        {
            int lines = 0;

            for (int row = 0; row < State.GameField.GetLength(0); row++)
            {
                bool isLineFull = true;
                for (int col = 0; col < State.GameField.GetLength(1); col++)
                {
                    if (!State.GameField[row, col])
                    {
                        isLineFull = false;
                        break;
                    }
                }

                if (isLineFull)
                {
                    for (int rowMove = row; rowMove >= 1; rowMove--)
                    {
                        for (int col = 0; col < State.GameField.GetLength(1); col++)
                        {
                            State.GameField[rowMove, col] = State.GameField[rowMove - 1, col];
                        }
                    }

                    lines++;
                }
            }

            return lines;
        }

        static void AddToGameField()
        {
            for (int row = 0; row < State.CurrentTetromino.GetLength(0); row++)
            {
                for (int col = 0; col < State.CurrentTetromino.GetLength(1); col++)
                {
                    if (State.CurrentTetromino[row, col])
                    {
                        State.GameField[State.TetrominoRow + row, State.TetrominoCol + col] = true;
                    }
                }
            }
        }

        static void RotateTetromino()
        {
            bool[,] rotatedTetromino = new bool[State.CurrentTetromino.GetLength(1), State.CurrentTetromino.GetLength(0)];

            for (int row = 0; row < State.CurrentTetromino.GetLength(0); row++)
            {
                for (int col = 0; col < State.CurrentTetromino.GetLength(1); col++)
                {
                    rotatedTetromino[col, State.CurrentTetromino.GetLength(0) - row - 1] = State.CurrentTetromino[row, col];
                }
            }

            if (!Collision(rotatedTetromino)) State.CurrentTetromino = rotatedTetromino;
        }

        static void DrawField()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            for (int row = 0; row < State.GameField.GetLength(0); row++)
            {
                StringBuilder sb = new StringBuilder();
                for (int col = 0; col < State.GameField.GetLength(1); col++)
                {
                    if (State.GameField[row, col])
                        sb.Append("*");
                    else
                        sb.Append(" ");
                }

                Write(sb.ToString(), 1, row + 1);
            }
        }

        static void DrawTetromino()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            for (int row = 0; row < State.CurrentTetromino.GetLength(0); row++)
            {
                for (int col = 0; col < State.CurrentTetromino.GetLength(1); col++)
                {
                    if (State.CurrentTetromino[row, col])
                        Write("*", 1 + State.TetrominoCol + col, row + 1 + State.TetrominoRow);
                }
            }
        }

        static void PrintGameOver()
        {
            string scoreAsString = State.Score.ToString();
            scoreAsString += new string(' ', 7 - scoreAsString.Length);

            // Print border
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Write("╔═════════╗", 5, 5);
            Write("║ ", 5, 6); Write("    ║", 11, 6);
            Write("║  ", 5, 7); Write("  ║", 13, 7);
            Write("║ ", 5, 8); Write("  ║", 13, 8);
            Write("║ ", 5, 9); Write("║", 15, 9);
            Write("╚═════════╝", 5, 10);

            // Print game over
            Console.ForegroundColor = ConsoleColor.Red;
            Write("Game", 7, 6);
            Write("Over!", 8, 7);

            // Print score
            Console.ForegroundColor = ConsoleColor.Green;
            Write("Score:", 7, 8);
            Write($"{scoreAsString}", 7, 9);
        }

        static void DrawBorder()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.SetCursorPosition(0, 0);

            // Top Line
            StringBuilder topLine = new StringBuilder();
            topLine.Append('╔');
            topLine.Append('═', GameFieldCols);
            topLine.Append('╦');
            topLine.Append('═', InfoFieldCols);
            topLine.Append('╗');
            Console.WriteLine(topLine);

            // Middle Lines
            for (int i = 0; i < GameFieldRows; i++)
            {
                StringBuilder midLine = new StringBuilder();
                midLine.Append('║');
                midLine.Append(new string(' ', GameFieldCols));
                midLine.Append('║');
                midLine.Append(new string(' ', InfoFieldCols));
                midLine.Append('║');
                Console.WriteLine(midLine);
            }

            // Bottom Line
            StringBuilder bottomLine = new StringBuilder();
            bottomLine.Append('╚');
            bottomLine.Append('═', GameFieldCols);
            bottomLine.Append('╩');
            bottomLine.Append('═', InfoFieldCols);
            bottomLine.Append('╝');
            Console.WriteLine(bottomLine);
        }

        static void DrawInfo()
        {
            if (State.Score > State.HighScore) State.HighScore = State.Score;

            Console.ForegroundColor = ConsoleColor.Red;

            Write("Level:", GameFieldCols + 3, 1);
            Write($"{State.Level} / 10", GameFieldCols + 3, 2);

            Write("Score:", GameFieldCols + 3, 4);
            Write(State.Score.ToString(), GameFieldCols + 3, 5);

            Write("Best:", GameFieldCols + 3, 7);
            Write(State.HighScore.ToString(), GameFieldCols + 3, 8);

            Write("Frame:", GameFieldCols + 3, 10);
            Write($"{State.Frame} / {State.MoveFrame - State.Level}", GameFieldCols + 3, 11);

            Write("Pos:", GameFieldCols + 3, 13);
            Write($"{State.TetrominoRow}, {State.TetrominoCol}", GameFieldCols + 3, 14);

            Write("Controls:", GameFieldCols + 3, 16);
            Write("   ^", GameFieldCols + 3, 17);
            Write(" <   >", GameFieldCols + 3, 18);
            Write("   V", GameFieldCols + 3, 19);
            Write(" Space", GameFieldCols + 3, 20);
        }

        static void Write(string text, int col, int row)
        {
            Console.SetCursorPosition(col, row);
            Console.Write(text);
        }
    }
}
