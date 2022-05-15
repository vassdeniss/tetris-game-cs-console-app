namespace TetrisCsConsole
{
    public interface ILogic
    {
        Tetromino CurrentTetromino { get; set; }

        int CurrentTetrominoCol { get; set; }

        int CurrentTetrominoRow { get; set; }

        int GameColumns { get; }

        bool[,] GameField { get; }

        int GameRows { get; }

        int Level { get; }

        void UpdateLevel(int score);
        
        void AddToGameField();

        int CheckFullLines();

        void GenerateRandomTetromino();

        bool Collision(Tetromino tetromino);

        bool CanMoveLeft();

        bool CanMoveRight();
    }
}
