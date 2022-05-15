using System;

namespace TetrisCsConsole
{
    public class ConsoleInputHandler : IInputHandler
    {
        public GameInput GetInput()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.Escape:
                        return GameInput.Exit;
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.Spacebar:
                        return GameInput.Rotate;
                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        return GameInput.Down;
                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        return GameInput.Left;
                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        return GameInput.Right;
                }
            }

            return GameInput.None;
        }
    }

    public enum GameInput
    {
        None,
        Down,
        Left,
        Right,
        Rotate,
        Exit
    }
}
