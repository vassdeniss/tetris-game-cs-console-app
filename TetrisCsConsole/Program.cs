using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace TetrisCsConsole
{
    internal class Program
    {
        // Settings
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
        static int Score = 0;
        static int Frame = 0;
        static int MoveFrame = 20;
        static bool[,] CurrentTetromino = null;
        static int TetrominoRow = 0;
        static int TetrominoCol = 0;
        static bool[,] GameField = new bool[GameFieldRows, GameFieldCols];

        static void Main(string[] args)
        {
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
                            // TODO: RotateTetromino()
                            break;
                        case ConsoleKey.S:
                        case ConsoleKey.DownArrow:
                            // TODO: MoveTetrominoDown()
                            TetrominoRow++; // Out of bounds check
                            Frame = 1;
                            Score++;
                            break;
                        case ConsoleKey.A:
                        case ConsoleKey.LeftArrow:
                            // TODO: MoveTetrominoLeft()
                            if (TetrominoCol > 0) TetrominoCol--;
                            break;
                        case ConsoleKey.D:
                        case ConsoleKey.RightArrow:
                            // TODO: MoveTetrominoRight()
                            if (TetrominoCol < GameFieldCols - CurrentTetromino.GetLength(0) - 1) TetrominoCol++;
                            break;
                    }
                }

                // Update Game State
                if (Frame % MoveFrame == 0)
                {
                    TetrominoRow++;
                    Frame = 0;
                }

                if (Collision())
                {
                    AddToGameField();
                    CurrentTetromino = Tetrominos[RandomTetromino.Next(0, Tetrominos.Count)];
                    TetrominoCol = 0;
                    TetrominoRow = 0;

                    if (Collision())
                    {
                        File.AppendAllLines("Scores.txt", new List<string>
                        {
                            $"[{DateTime.Now}] {Environment.UserName} => {Score}"
                        });

                        PrintGameOver();
                        Thread.Sleep(100000);
                        return;
                    }
                }
                // check for lines 
                //  - increment score 

                // Update UI
                DrawBorder();
                DrawInfo();
                DrawField();
                DrawTetromino();
                Thread.Sleep(50);
            }
        }

        static bool Collision()
        {
            if (TetrominoRow + CurrentTetromino.GetLength(0) == GameFieldRows) return true;

            for (int row = 0; row < CurrentTetromino.GetLength(0); row++)
            {
                for (int col = 0; col < CurrentTetromino.GetLength(1); col++)
                {
                    if (CurrentTetromino[row, col] && (GameField[TetrominoRow + row + 1, TetrominoCol + col] || GameField[TetrominoRow + row, TetrominoCol + col]))
                    {
                        return true;
                    }
                }
            }

            return false;
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

        static void DrawField()
        {
            for (int row = 0; row < GameField.GetLength(0); row++)
            {
                for (int col = 0; col < GameField.GetLength(1); col++)
                {
                    if (GameField[row, col])
                    {
                        Write("*", col + 1, row + 1, ConsoleColor.DarkGreen);
                    }
                }
            }
        }

        static void DrawTetromino()
        {
            for (int row = 0; row < CurrentTetromino.GetLength(0); row++)
            {
                for (int col = 0; col < CurrentTetromino.GetLength(1); col++)
                {
                    if (CurrentTetromino[row, col])
                    {
                        Write($"*", col + 1 + TetrominoCol, row + 1 + TetrominoRow, ConsoleColor.Green);
                    }
                }
            }
        }

        static void PrintGameOver()
        {
            string scoreAsString = Score.ToString();
            scoreAsString += new string(' ', 7 - scoreAsString.Length);

            ConsoleColor borderColor = ConsoleColor.DarkYellow;
            ConsoleColor gameOverColor = ConsoleColor.Red;
            ConsoleColor scoreColor = ConsoleColor.Green;

            Write("╔═════════╗", 5, 5, borderColor);
            Write("║", 5, 6, borderColor);
            Write("Game    ", 7, 6, gameOverColor);
            Write("║", 15, 6, borderColor);
            Write("║", 5, 7, borderColor);
            Write("Over! ", 9, 7, gameOverColor);
            Write("║", 15, 7, borderColor);
            Write("║", 5, 8, borderColor);
            Write("Score:    ", 7, 8, scoreColor);
            Write("║", 15, 8, borderColor);
            Write("║", 5, 9, borderColor);
            Write($"{scoreAsString} ", 9, 9, scoreColor);
            Write("║", 15, 9, borderColor);
            Write("╚═════════╝", 5, 10, borderColor);
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

            Console.ResetColor();
        }

        static void DrawInfo()
        {
            Write("Score:", GameFieldCols + 3, 1, ConsoleColor.Red);
            Write(Score.ToString(), GameFieldCols + 3, 2, ConsoleColor.Red);

            Write("Frame:", GameFieldCols + 3, 4, ConsoleColor.Red);
            Write(Frame.ToString(), GameFieldCols + 3, 5, ConsoleColor.Red);

            Write("Pos:", GameFieldCols + 3, 7, ConsoleColor.Red);
            Write($"{TetrominoRow}, {TetrominoCol}", GameFieldCols + 3, 8, ConsoleColor.Red);

            Write("Controls:", GameFieldCols + 3, 10, ConsoleColor.Red);
            Write("   ^", GameFieldCols + 3, 12, ConsoleColor.Red);
            Write(" < v >", GameFieldCols + 3, 13, ConsoleColor.Red);
        }

        static void Write(string text, int col, int row, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.SetCursorPosition(col, row);
            Console.Write(text);
            Console.ResetColor();
        }
    }
}
