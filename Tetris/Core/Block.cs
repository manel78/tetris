namespace Tetris.Game
{

    public class Block
    {

        #region Ctor

        public Block(Block block)
        {
            X = block.X;
            Y = block.Y;
            Status = block.Status;
        }

        public Block(Block block , BlockStatus status)
        {
            X = block.X;
            Y = block.Y;
            Status = status;
        }

        public Block(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Block(int x, int y, BlockStatus status)
        {
            X = x;
            Y = y;
            Status = status;
        }

        #endregion

        #region Public Properties

        public int X { get; set; }

        public int Y { get; set; }

        public BlockStatus Status { get; set; }

        #endregion

        #region Public Methods

        public void MoveRight()
        {
            X++;
        }

        public void MoveLeft()
        {
            X--;
        }

        public void MoveDown()
        {
            Y++;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Block inputBlock)) return false;

            return inputBlock.X == X && inputBlock.Y == Y;

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

    }
}
