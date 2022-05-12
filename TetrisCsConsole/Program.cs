using System;
using System.Collections.Generic;
using System.Threading;

namespace TetrisCsConsole
{
    public static class Program
    {
        // Settings
        static readonly int GameRows = 20;
        static readonly int GameColumns = 10;
        static readonly List<Tetromino> Tetrominos = new List<Tetromino>()
        {
            new Tetromino(new bool[,] // I
            {
                { true, true, true, true }
            }),
            new Tetromino(new bool[,] // O
            {
                { true, true },
                { true, true }
            }),
            new Tetromino(new bool[,] // T
            {
                { false, true, false },
                { true, true, true }
            }),
            new Tetromino(new bool[,] // S
            {
                { false, true, true },
                { true, true, false }
            }),
            new Tetromino(new bool[,] // Z
            {
                { true, true, false },
                { false, true, true }
            }),
            new Tetromino(new bool[,] // J
            {
                { true, false, false },
                { true, true, true }
            }),
            new Tetromino(new bool[,] // L
            {
                { false, false, true },
                { true, true, true }
            })
        };

        // States 
        static readonly GameState State = new GameState(GameRows, GameColumns);
        static readonly ConsoleRender Renderer = new ConsoleRender(GameRows, GameColumns);

        public static void Main()
        {
            ScoreManager manager = new ScoreManager("YourHighScores.txt");

            MusicPlayer player = new MusicPlayer();
            player.Play();

            Random randomTetromino = new Random();
            State.CurrentTetromino = Tetrominos[randomTetromino.Next(0, Tetrominos.Count)];
            while (true)
            {
                State.Frame++;
                State.UpdateLevel(manager.Score);

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
                            Tetromino rotatedTetromino = State.CurrentTetromino.GetRotation();
                            if (!Collision(rotatedTetromino))
                                State.CurrentTetromino = rotatedTetromino;
                            break;
                        case ConsoleKey.S:
                        case ConsoleKey.DownArrow:
                            State.TetrominoRow++;
                            State.Frame = 1;
                            manager.AddScore(State.Level);
                            break;
                        case ConsoleKey.A:
                        case ConsoleKey.LeftArrow:
                            if (State.TetrominoCol > 0 && !CollisionSideLeft()) State.TetrominoCol--;
                            break;
                        case ConsoleKey.D:
                        case ConsoleKey.RightArrow:
                            if (State.TetrominoCol < GameColumns - State.CurrentTetromino.Height && !CollisionSideRight())
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
                    manager.AddScore(State.LineScores[lines]);
                    State.CurrentTetromino = Tetrominos[randomTetromino.Next(0, Tetrominos.Count)];
                    State.TetrominoCol = 0;
                    State.TetrominoRow = 0;

                    if (Collision(State.CurrentTetromino))
                    {
                        manager.PostHighScore();
                        Renderer.RedrawUI(State, manager);
                        Renderer.DrawGameOver(manager.Score);
                        Thread.Sleep(100000);
                        return;
                    }
                }

                Renderer.RedrawUI(State, manager);
                Thread.Sleep(50);
            }
        }

        static bool CollisionSideLeft()
        {
            for (int row = 0; row < State.CurrentTetromino.Width; row++)
            {
                for (int col = 0; col < State.CurrentTetromino.Height; col++)
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
            for (int row = 0; row < State.CurrentTetromino.Width; row++)
            {
                for (int col = 0; col < State.CurrentTetromino.Height; col++)
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

        static bool Collision(Tetromino tetromino)
        {
            if (State.TetrominoCol > GameColumns - tetromino.Height) return true;

            if (State.TetrominoRow + tetromino.Width == GameRows) return true;

            for (int row = 0; row < tetromino.Width; row++)
            {
                for (int col = 0; col < tetromino.Height; col++)
                {
                    if (tetromino.Shape[row, col] && State.GameField[State.TetrominoRow + row + 1, State.TetrominoCol + col])
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
            for (int row = 0; row < State.CurrentTetromino.Width; row++)
            {
                for (int col = 0; col < State.CurrentTetromino.Height; col++)
                {
                    if (State.CurrentTetromino.Shape[row, col])
                    {
                        State.GameField[State.TetrominoRow + row, State.TetrominoCol + col] = true;
                    }
                }
            }
        }
    }
}
