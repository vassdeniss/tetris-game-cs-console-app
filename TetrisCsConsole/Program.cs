using System;
using System.Text;
using System.Threading;

namespace TetrisCsConsole
{
    internal class Program
    {
        // Settings
        static int GameFieldHeight = 20;
        static int GameFieldWidth = 10;
        static int InfoFieldWidth = 10;
        static int ConsoleHeight = 1 + GameFieldHeight + 1;
        static int ConsoleWidth = 1 + GameFieldWidth + 1 + InfoFieldWidth + 1;

        // States 
        static int Score = 0;

        static void Main(string[] args)
        {
            SetConsoleSettings("Tetris in the Console", ConsoleWidth, ConsoleHeight, false, ConsoleColor.Green);
            DrawBorder();
            DrawInfo();
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Escape)
                        return;
                }

                Score++;
                DrawBorder();
                DrawInfo();
                Thread.Sleep(35);
            }
        }

        static void SetConsoleSettings(string name,
                                       int consoleWidth,
                                       int consoleHeight,
                                       bool isCursorVisible = false,
                                       ConsoleColor color = ConsoleColor.Green)
        {
            Console.Title = name;
            Console.CursorVisible = isCursorVisible;
            Console.SetWindowSize(consoleWidth, consoleHeight + 1);
            Console.SetBufferSize(consoleWidth, consoleHeight + 1);
            Console.ForegroundColor = color;
        }
        
        static void DrawBorder()
        {
            Console.SetCursorPosition(0, 0);

            // Top Line
            StringBuilder topLine = new StringBuilder();
            topLine.Append('╔');
            topLine.Append('═', GameFieldWidth);
            topLine.Append('╦');
            topLine.Append('═', InfoFieldWidth);
            topLine.Append('╗');
            Console.WriteLine(topLine);

            // Middle Lines
            for (int i = 0; i < GameFieldHeight; i++)
            {
                StringBuilder midLine = new StringBuilder();
                midLine.Append('║');
                midLine.Append(new string(' ', GameFieldWidth));
                midLine.Append('║');
                midLine.Append(new string(' ', InfoFieldWidth));
                midLine.Append('║');
                Console.WriteLine(midLine);
            }

            // Bottom Line
            StringBuilder bottomLine = new StringBuilder();
            bottomLine.Append('╚');
            bottomLine.Append('═', GameFieldWidth);
            bottomLine.Append('╩');
            bottomLine.Append('═', InfoFieldWidth);
            bottomLine.Append('╝');
            Console.WriteLine(bottomLine);
        }

        static void DrawInfo()
        {
            Write("Score:", GameFieldWidth + 3, 1);
            Write(Score.ToString(), GameFieldWidth + 3, 2);
        }

        static void Write(string text, int col, int row)
        {
            Console.SetCursorPosition(col, row);
            Console.Write(text);
        }
    }
}
