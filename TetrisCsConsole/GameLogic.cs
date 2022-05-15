using System;
using System.Collections.Generic;

namespace TetrisCsConsole
{
    public class GameLogic : ILogic
    {
        private readonly List<Tetromino> tetrominos = new List<Tetromino>()
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

        private readonly Random random;

        public GameLogic(int gameRows, int gameColumns)
        {
            this.GameRows = gameRows;
            this.GameColumns = gameColumns;

            this.GameField = new bool[gameRows, gameColumns];
            this.Level = 1;
            this.CurrentTetromino = null;
            this.CurrentTetrominoRow = 0;
            this.CurrentTetrominoCol = 0;
            this.random = new Random();

            this.GenerateRandomTetromino();
        }

        public bool[,] GameField { get; private set; }

        public int Level { get; private set; }

        public Tetromino CurrentTetromino { get; set; }

        public int CurrentTetrominoRow { get; set; }

        public int CurrentTetrominoCol { get; set; }

        public int GameRows { get; }

        public int GameColumns { get; }

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

        public void AddToGameField()
        {
            for (int row = 0; row < this.CurrentTetromino.Width; row++)
            {
                for (int col = 0; col < this.CurrentTetromino.Height; col++)
                {
                    if (this.CurrentTetromino.Shape[row, col])
                    {
                        this.GameField[this.CurrentTetrominoRow + row, this.CurrentTetrominoCol + col] = true;
                    }
                }
            }
        }

        public int CheckFullLines()
        {
            int lines = 0;

            for (int row = 0; row < this.GameField.GetLength(0); row++)
            {
                bool isLineFull = true;
                for (int col = 0; col < this.GameField.GetLength(1); col++)
                {
                    if (!this.GameField[row, col])
                    {
                        isLineFull = false;
                        break;
                    }
                }

                if (isLineFull)
                {
                    for (int rowMove = row; rowMove >= 1; rowMove--)
                    {
                        for (int col = 0; col < this.GameField.GetLength(1); col++)
                        {
                            this.GameField[rowMove, col] = this.GameField[rowMove - 1, col];
                        }
                    }

                    lines++;
                }
            }

            return lines;
        }

        public void GenerateRandomTetromino()
        {
            this.CurrentTetromino = this.tetrominos[this.random.Next(0, tetrominos.Count)];
            this.CurrentTetrominoRow = 0;
            this.CurrentTetrominoCol = this.GameColumns / 2 - this.CurrentTetromino.Width / 2;
        }

        public bool Collision(Tetromino tetromino)
        {
            if (this.CurrentTetrominoCol > GameColumns - tetromino.Height) return true;

            if (this.CurrentTetrominoRow + tetromino.Width == GameRows) return true;

            for (int row = 0; row < tetromino.Width; row++)
            {
                for (int col = 0; col < tetromino.Height; col++)
                {
                    if (tetromino.Shape[row, col]
                        && this.GameField[this.CurrentTetrominoRow + row + 1, this.CurrentTetrominoCol + col])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool CanMoveLeft()
        {
            return this.CurrentTetrominoCol >= 1 && !CheckForCollision(-1);
        }

        public bool CanMoveRight()
        {
            return (this.CurrentTetrominoCol < this.GameColumns - this.CurrentTetromino.Height)
                && !CheckForCollision(1);
        }

        private bool CheckForCollision(int direction)
        {
            for (int row = 0; row < this.CurrentTetromino.Width; row++)
            {
                for (int col = 0; col < this.CurrentTetromino.Height; col++)
                {
                    if (this.CurrentTetromino.Shape[row, col] 
                        && this.GameField[this.CurrentTetrominoRow + row, this.CurrentTetrominoCol + col + direction])
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
