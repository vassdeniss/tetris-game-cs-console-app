using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace TetrisCsConsole
{
    internal class Program
    {
        // Settings
        static string ScoreFileName = "YourHighScores.txt";
        static int GameFieldRows = 20;
        static int GameFieldCols = 10;
        static int InfoFieldCols = 10;
        static int ConsoleHeight = 1 + GameFieldRows + 1;
        static int ConsoleWidth = 1 + GameFieldCols + 1 + InfoFieldCols + 1;
        static List<bool[,]> Tetrominos = new List<bool[,]>()
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
        static Random RandomTetromino = new Random();
        static int HighScore = 0;
        static int Score = 0;
        static int Frame = 0;
        static int MoveFrame = 20;
        static bool[,] CurrentTetromino = null;
        static int TetrominoRow = 0;
        static int TetrominoCol = 0;
        static bool[,] GameField = new bool[GameFieldRows, GameFieldCols];
        static int[] LineScores = { 0, 40, 100, 300, 1200 };

        static void Main(string[] args)
        {
            if (File.Exists(ScoreFileName))
            {
                string[] scores = File.ReadAllLines(ScoreFileName);

                foreach (string score in scores)
                {
                    Match scorePattern = Regex.Match(score, @" => (?<score>[0-9]+)");
                    HighScore = Math.Max(HighScore, int.Parse(scorePattern.Groups["score"].Value));
                }
            }

            Console.Title = "Tetris in the Console";
            Console.CursorVisible = false;
            Console.SetWindowSize(ConsoleWidth, ConsoleHeight + 1);
            Console.SetBufferSize(ConsoleWidth, ConsoleHeight + 1);

            CurrentTetromino = Tetrominos[RandomTetromino.Next(0, Tetrominos.Count)];
            while (true)
            {
                Frame++;

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
                            TetrominoRow++;
                            Frame = 1;
                            Score++;
                            break;
                        case ConsoleKey.A:
                        case ConsoleKey.LeftArrow:
                            if (TetrominoCol > 0 && !CollisionSideLeft()) TetrominoCol--;
                            break;
                        case ConsoleKey.D:
                        case ConsoleKey.RightArrow:
                            if (TetrominoCol < GameFieldCols - CurrentTetromino.GetLength(1) && !CollisionSideRight())
                                TetrominoCol++;
                            break;
                    }
                }

                // Update Game State
                if (Frame % MoveFrame == 0)
                {
                    TetrominoRow++;
                    Frame = 0;
                }

                if (Collision(CurrentTetromino))
                {
                    AddToGameField();
                    int lines = CheckFullLines();
                    Score += LineScores[lines];
                    CurrentTetromino = Tetrominos[RandomTetromino.Next(0, Tetrominos.Count)];
                    TetrominoCol = 0;
                    TetrominoRow = 0;

                    if (Collision(CurrentTetromino))
                    {
                        File.AppendAllLines(ScoreFileName, new List<string>
                        {
                            $"[{DateTime.Now}] {Environment.UserName} => {Score}"
                        });

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
            for (int row = 0; row < CurrentTetromino.GetLength(0); row++)
            {
                for (int col = 0; col < CurrentTetromino.GetLength(1); col++)
                {
                    if (TetrominoCol + col - 1 > -1 && TetrominoCol + col + 1 < GameField.GetLength(1))
                    {
                        if (GameField[TetrominoRow + row, TetrominoCol + col - 1])
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
            for (int row = 0; row < CurrentTetromino.GetLength(0); row++)
            {
                for (int col = 0; col < CurrentTetromino.GetLength(1); col++)
                {
                    if (TetrominoCol + col - 1 > -1 && TetrominoCol + col + 1 < GameField.GetLength(1))
                    {
                        if (GameField[TetrominoRow + row, TetrominoCol + col + 1])
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
            if (TetrominoCol > GameFieldCols - tetromino.GetLength(1)) return true;

            if (TetrominoRow + tetromino.GetLength(0) == GameFieldRows) return true;

            for (int row = 0; row < tetromino.GetLength(0); row++)
            {
                for (int col = 0; col < tetromino.GetLength(1); col++)
                {
                    if (tetromino[row, col] && GameField[TetrominoRow + row + 1, TetrominoCol + col])
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

            for (int row = 0; row < GameField.GetLength(0); row++)
            {
                bool isLineFull = true;
                for (int col = 0; col < GameField.GetLength(1); col++)
                {
                    if (!GameField[row, col])
                    {
                        isLineFull = false;
                        break;
                    }
                }

                if (isLineFull)
                {
                    for (int rowMove = row; rowMove >= 1; rowMove--)
                    {
                        for (int col = 0; col < GameField.GetLength(1); col++)
                        {
                            GameField[rowMove, col] = GameField[rowMove - 1, col];
                        }
                    }

                    lines++;
                }
            }

            return lines;
        }

        static void AddToGameField()
        {
            for (int row = 0; row < CurrentTetromino.GetLength(0); row++)
            {
                for (int col = 0; col < CurrentTetromino.GetLength(1); col++)
                {
                    if (CurrentTetromino[row, col])
                    {
                        GameField[TetrominoRow + row, TetrominoCol + col] = true;
                    }
                }
            }
        }

        static void RotateTetromino()
        {
            bool[,] rotatedTetromino = new bool[CurrentTetromino.GetLength(1), CurrentTetromino.GetLength(0)];

            for (int row = 0; row < CurrentTetromino.GetLength(0); row++)
            {
                for (int col = 0; col < CurrentTetromino.GetLength(1); col++)
                {
                    rotatedTetromino[col, CurrentTetromino.GetLength(0) - row - 1] = CurrentTetromino[row, col];
                }
            }

            if (!Collision(rotatedTetromino)) CurrentTetromino = rotatedTetromino;
        }

        static void DrawField()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            for (int row = 0; row < GameField.GetLength(0); row++)
            {
                StringBuilder sb = new StringBuilder();
                for (int col = 0; col < GameField.GetLength(1); col++)
                {
                    if (GameField[row, col])
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
            for (int row = 0; row < CurrentTetromino.GetLength(0); row++)
            {
                StringBuilder sb = new StringBuilder();
                for (int col = 0; col < CurrentTetromino.GetLength(1); col++)
                {
                    if (CurrentTetromino[row, col])
                        sb.Append("*");
                    else
                        sb.Append(" ");
                }

                Write(sb.ToString(), 1 + TetrominoCol, row + 1 + TetrominoRow);
            }
        }

        static void PrintGameOver()
        {
            string scoreAsString = Score.ToString();
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
            if (Score > HighScore) HighScore = Score;

            Console.ForegroundColor = ConsoleColor.Red;

            Write("Score:", GameFieldCols + 3, 1);
            Write(Score.ToString(), GameFieldCols + 3, 2);

            Write("Best:", GameFieldCols + 3, 4);
            Write(HighScore.ToString(), GameFieldCols + 3, 5);

            Write("Frame:", GameFieldCols + 3, 7);
            Write(Frame.ToString(), GameFieldCols + 3, 8);

            Write("Pos:", GameFieldCols + 3, 10);
            Write($"{TetrominoRow}, {TetrominoCol}", GameFieldCols + 3, 11);

            Write("Controls:", GameFieldCols + 3, 13);
            Write("   ^", GameFieldCols + 3, 15);
            Write(" < v >", GameFieldCols + 3, 16);
        }

        static void Write(string text, int col, int row)
        {
            Console.SetCursorPosition(col, row);
            Console.Write(text);
        }
    }
}
