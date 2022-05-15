using System.Threading;

namespace TetrisCsConsole
{
    public class GameEngine
    {
        private readonly ILogic logic;
        private readonly IInputHandler inputHandler;
        private readonly ConsoleRender renderer;
        private readonly ScoreManager manager;

        public GameEngine(ILogic logic, IInputHandler inputHandler, ConsoleRender renderer, ScoreManager manager)
        {
            this.logic = logic;
            this.inputHandler = inputHandler;
            this.renderer = renderer;
            this.manager = manager;
        }

        public void Run()
        {
            while (true)
            {
                this.renderer.Frame++;
                this.logic.UpdateLevel(this.manager.Score);

                GameInput input = this.inputHandler.GetInput();
                switch (input)
                {
                    case GameInput.Down:
                        this.logic.CurrentTetrominoRow++;
                        this.renderer.Frame = 1;
                        this.manager.AddScore(0, this.logic.Level);
                        break;
                    case GameInput.Left:
                        if (this.logic.CanMoveLeft()) this.logic.CurrentTetrominoCol--;
                        break;
                    case GameInput.Right:
                        if (this.logic.CanMoveRight()) this.logic.CurrentTetrominoCol++;
                        break;
                    case GameInput.Rotate:
                        Tetromino rotatedTetromino = this.logic.CurrentTetromino.GetRotation();
                        if (!this.logic.Collision(rotatedTetromino)) this.logic.CurrentTetromino = rotatedTetromino;
                        break;
                    case GameInput.Exit:
                        return;
                }

                if (this.renderer.Frame % (this.renderer.MoveFrame - this.logic.Level) == 0)
                {
                    this.logic.CurrentTetrominoRow++;
                    this.renderer.Frame = 0;
                }

                if (this.logic.Collision(this.logic.CurrentTetromino))
                {
                    this.logic.AddToGameField();
                    this.manager.AddScore(this.logic.CheckFullLines(), this.logic.Level);

                    this.logic.GenerateRandomTetromino();
                    if (this.logic.Collision(this.logic.CurrentTetromino))
                    {
                        this.manager.PostHighScore();
                        this.renderer.RedrawUI(logic, manager);
                        this.renderer.DrawGameOver(this.manager.Score);
                        Thread.Sleep(100000);
                        return;
                    }
                }

                this.renderer.RedrawUI(logic, manager);
                Thread.Sleep(50);
            }
        }
    }
}
