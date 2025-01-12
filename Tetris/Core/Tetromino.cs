using System.Collections.Generic;
using System.Linq;
using Tetris.Game.Results;

namespace Tetris.Game.Tetriminoes
{

    internal abstract class Tetromino
    {

        #region Private Variables

        private readonly Deck deck;

        private readonly byte tetrominoWidthHeight;

        #endregion

        #region Private Methods

        private List<Block> InverseVisibleBlocks()
        {
            var changedBlocks = new List<Block>();
            foreach (var block in VisibleBlocks)
            {
                changedBlocks.Add(new Block(block, BlockStatus.Hidden));
            }
            return changedBlocks;
        }

        private Block[] AddVisibleBlocksToChangedBlocks(List<Block> changedBlocks)
        {
            foreach (var item in VisibleBlocks)
            {
                if (changedBlocks.Contains(item))
                {
                    changedBlocks.Remove(item);
                }
            }
            changedBlocks.AddRange(VisibleBlocks);
            return changedBlocks.ToArray();
        }

        private void CreateBlocks()
        {
            Blocks = new Block[tetrominoWidthHeight * tetrominoWidthHeight];
            for (var i = 0; i < tetrominoWidthHeight; i++)
            {
                for (var j = 0; j < tetrominoWidthHeight; j++)
                {
                    Blocks[i * tetrominoWidthHeight + j] = new Block(deck.Width / 2 - tetrominoWidthHeight / 2 + i, j - tetrominoWidthHeight);
                }
            }
            SpecifyVisibleBlocks();
        }

        private Block[] CalculateBaseBlocks()
        {
            var blockIndex = 0;
            var blockList = new List<Block>();
            for (var i = 0; i < tetrominoWidthHeight; i++)
            {
                for (var j = 0; j < tetrominoWidthHeight; j++)
                {
                    if (Blocks[blockIndex].Status != BlockStatus.Hidden)
                    {
                        blockList.Add(new Block(i, j, Blocks[blockIndex].Status));
                    }
                    blockIndex++;
                }
            }
            return blockList.ToArray();
        }

        private bool GameOver()
        {
            foreach (Block block in VisibleBlocks)
            {
                if (deck.GameOver(block.Y)) return true;
            }
            return false;
        }

        private ChangeResult[] MoveRightAndRotate()
        {
            var changeResults = new List<ChangeResult>
            {
                MoveRight()
            };
            if (CanRotate())
            {
                changeResults.AddRange(Rotate());
                return changeResults.ToArray();
            }
            MoveLeft();
            return new ChangeResult[0];
        }

        private ChangeResult[] MoveLeftAndRotate()
        {
            var changeResults = new List<ChangeResult>
            {
                MoveLeft()
            };
            if (CanRotate())
            {
                changeResults.AddRange(Rotate());
                return changeResults.ToArray();
            }
            MoveRight();
            return new ChangeResult[0];
        }

        private void RotateTetromino()
        {
            var counter = 0;
            var sPoint = tetrominoWidthHeight - 1;
            var tempBlocks = new Block[Blocks.Length];
            var index = sPoint;
            for (var i = 0; i < tetrominoWidthHeight; i++)
            {
                for (var j = 0; j < tetrominoWidthHeight; j++)
                {
                    tempBlocks[counter] = new Block(Blocks[counter], Blocks[index].Status);
                    counter++;
                    index += tetrominoWidthHeight;
                }
                sPoint--;
                index = sPoint;
            }

            for (var i = 0; i < Blocks.Length; i++)
            {
                Blocks[i] = tempBlocks[i];
            }
        }

        #endregion

        #region Ctor

        public Tetromino(Deck deck, byte tetrominoWidthHeight)
        {
            this.deck = deck;
            this.tetrominoWidthHeight = tetrominoWidthHeight;
            CreateBlocks();
        }

        #endregion

        #region Public Properties

        public Block[] VisibleBlocks { get { return Blocks.Where(s => s.Status != BlockStatus.Hidden).ToArray(); } }

        public Block[] BaseBlocks { get { return CalculateBaseBlocks(); } }

        #endregion

        #region Public Methods

        public bool CanMoveDown()
        {
            foreach (var block in VisibleBlocks)
            {
                if (deck.Collision(block.X, block.Y + 1)) return false;
            }
            return true;
        }

        public ChangeResult MoveRight()
        {
            if (!CanMoveRight()) return null;

            var inversedBlocks = InverseVisibleBlocks();
            foreach (var block in Blocks)
            {
                block.MoveRight();
            }
            var changedBlocks = AddVisibleBlocksToChangedBlocks(inversedBlocks);
            return new ChangeResult { ChangedBlocks = changedBlocks };
        }

        public ChangeResult MoveLeft()
        {
            if (!CanMoveLeft()) { return null; }

            var inversedBlocks = InverseVisibleBlocks();

            foreach (var block in Blocks)
            {
                block.MoveLeft();
            }

            var changedBlocks = AddVisibleBlocksToChangedBlocks(inversedBlocks);
            return new ChangeResult() { ChangedBlocks = changedBlocks };
        }

        public MoveDownResult MoveDown()
        {
            if (!CanMoveDown())
            {
                if (GameOver())
                {
                    return new MoveDownResult { GameOver = true };
                }
                deck.FixBlocks(VisibleBlocks);
                var currentTetrominosRows = VisibleBlocks.Select(s => s.Y).Distinct().ToArray();
                var vanishRowResult = deck.VanishRows(currentTetrominosRows);
                return new MoveDownResult
                {
                    VanishRowResult = vanishRowResult
                };
            }
            else
            {
                var inversedBlocks = InverseVisibleBlocks();
                foreach (var block in Blocks)
                {
                    block.MoveDown();
                }
                var changedBlocks = AddVisibleBlocksToChangedBlocks(inversedBlocks);
                return new MoveDownResult { ChangedBlocks = changedBlocks };
            }
        }

        public virtual ChangeResult[] Rotate()
        {
            if (!CanRotate())
            {
                var canMoveRight = CanMoveRight();
                var canMoveLeft = CanMoveLeft();

                if (canMoveRight && !canMoveLeft)
                {
                    return MoveRightAndRotate();
                }

                if (!canMoveRight && canMoveLeft)
                {
                    return MoveLeftAndRotate();
                }
                return new ChangeResult[0];
            }
            var inversedBlocks = InverseVisibleBlocks();
            RotateTetromino();
            var changedBlocks = AddVisibleBlocksToChangedBlocks(inversedBlocks);
            return new[] { new ChangeResult() { ChangedBlocks = changedBlocks } };
        }

        public void ResetToTopMiddle()
        {
            for (var i = 0; i < tetrominoWidthHeight; i++)
            {
                for (var j = 0; j < tetrominoWidthHeight; j++)
                {
                    Blocks[i * tetrominoWidthHeight + j].X = deck.Width / 2 - tetrominoWidthHeight / 2 + i;
                    Blocks[i * tetrominoWidthHeight + j].Y = j - tetrominoWidthHeight;                    
                }
            }
        }

        #endregion

        #region Protected Properties

        protected Block[] Blocks { get; private set; }

        #endregion

        #region Protected Methods

        protected bool CanMoveLeft()
        {
            foreach (var block in VisibleBlocks)
            {
                if (deck.Collision(block.X - 1, block.Y)) return false;
            }
            return true;
        }

        protected bool CanMoveRight()
        {
            foreach (var block in VisibleBlocks)
            {
                if (deck.Collision(block.X + 1, block.Y)) return false;
            }
            return true;
        }

        protected bool CanRotate()
        {
            var cnt = 0;
            var sPoint = tetrominoWidthHeight - 1;
            var index = sPoint;
            for (var i = 0; i < tetrominoWidthHeight; i++)
            {
                for (var j = 0; j < tetrominoWidthHeight; j++)
                {
                    if (Blocks[index].Status != BlockStatus.Hidden && deck.Collision(Blocks[cnt].X, Blocks[cnt].Y))
                    {
                        return false;
                    }
                    cnt++;
                    index += tetrominoWidthHeight;
                }
                sPoint--;
                index = sPoint;
            }
            return true;
        }

        protected abstract void SpecifyVisibleBlocks();

        #endregion

    }
}
