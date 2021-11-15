using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TetrisCsConsole
{
    internal class Program
    {
        // Settings
        static int GameFieldRow = 20;
        static int GameFieldCol = 10;
        static int InfoFieldCol = 10;
        static int ConsoleHeight = 1 + GameFieldRow + 1;
        static int ConsoleWidth = 1 + GameFieldCol + 1 + InfoFieldCol + 1;
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
        static int Score = 0;
        static int Frame = 0;
        static int MoveFrame = 20;
        static int CurrentTetromino = 4;
        static int TetrominoRow = 0;
        static int TetrominoCol = 0;

        static void Main(string[] args)
        {
            SetConsoleSettings("Tetris in the Console", ConsoleWidth, ConsoleHeight, false);
            DrawBorder();
            DrawInfo();
            while (true)
            {
                Frame++;

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
                            TetrominoCol--; // Out of bounds check
                            break;
                        case ConsoleKey.D:
                        case ConsoleKey.RightArrow:
                            // TODO: MoveTetrominoRight()
                            TetrominoCol++; // Out of bounds check
                            break;
                    }
                }

                if (Frame % MoveFrame == 0)
                {
                    TetrominoRow++;
                    Frame = 0;
                    Score++;
                }

                // TODO:
                // if collision
                // add piece to field array
                // check for lines 
                //  - increment score 

                DrawBorder();
                DrawInfo();
                // TODO: DrawField();
                DrawTetromino();
                Thread.Sleep(50);
            }
        }

        private static void DrawTetromino()
        {
            bool[,] tetromino = Tetrominos[CurrentTetromino];

            for (int row = 0; row < tetromino.GetLength(0); row++)
            {
                for (int col = 0; col < tetromino.GetLength(1); col++)
                {
                    if (tetromino[row, col])
                    {
                        Write("*", col + 1 + TetrominoCol, row + 1 + TetrominoRow, ConsoleColor.Yellow);
                    }
                }
            }
        }

        static void SetConsoleSettings(string name, int consoleWidth,
                                       int consoleHeight, bool isCursorVisible = false)       
        {
            Console.Title = name;
            Console.CursorVisible = isCursorVisible;
            Console.SetWindowSize(consoleWidth, consoleHeight + 1);
            Console.SetBufferSize(consoleWidth, consoleHeight + 1);
        }
        
        static void DrawBorder()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(0, 0);

            // Top Line
            StringBuilder topLine = new StringBuilder();
            topLine.Append('╔');
            topLine.Append('═', GameFieldCol);
            topLine.Append('╦');
            topLine.Append('═', InfoFieldCol);
            topLine.Append('╗');
            Console.WriteLine(topLine);

            // Middle Lines
            for (int i = 0; i < GameFieldRow; i++)
            {
                StringBuilder midLine = new StringBuilder();
                midLine.Append('║');
                midLine.Append(new string(' ', GameFieldCol));
                midLine.Append('║');
                midLine.Append(new string(' ', InfoFieldCol));
                midLine.Append('║');
                Console.WriteLine(midLine);
            }

            // Bottom Line
            StringBuilder bottomLine = new StringBuilder();
            bottomLine.Append('╚');
            bottomLine.Append('═', GameFieldCol);
            bottomLine.Append('╩');
            bottomLine.Append('═', InfoFieldCol);
            bottomLine.Append('╝');
            Console.WriteLine(bottomLine);

            Console.ResetColor();
        }

        static void DrawInfo()
        {
            Write("Score:", GameFieldCol + 3, 1, ConsoleColor.Red);
            Write(Score.ToString(), GameFieldCol + 3, 2, ConsoleColor.Red);
            Write("Frame:", GameFieldCol + 3, 3, ConsoleColor.Red);
            Write(Frame.ToString(), GameFieldCol + 3, 4, ConsoleColor.Red);
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
