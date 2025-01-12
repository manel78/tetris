using System;
using System.Collections.Generic;
using System.Linq;
using Tetris.Game.Results;

namespace Tetris.Game
{

    internal class Deck
    {

        #region Private Variables

        private BlockStatus[,] Blocks;

        private int lowestRowPostion;

        #endregion

        #region Private Methods

        private int[] GetRowsToVanish(int[] rowsToScan)
        {
            var rowsToVanish = new List<int>();
            foreach (var row in rowsToScan)
            {
                var flag = true;
                for (var i = 0; i < Width; i++)
                {
                    if (Blocks[i, row] == BlockStatus.Hidden)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    rowsToVanish.Add(row);
                }
            }
            return rowsToVanish.ToArray();
        }

        private Block[] GetVanishedBlocks(int[] rowsToVanish)
        {
            var changedBlocks = new List<Block>();
            foreach (var row in rowsToVanish)
            {
                for (var i = 0; i < Width; i++)
                {
                    changedBlocks.Add(new Block(i, row , Blocks[i,row]));
                }
            }
            return changedBlocks.ToArray();
        }

        private Block[] FallDownBlocksByRow(int row)
        {
            var changedBlocks = new List<Block>();
            for (var i = row; i >= lowestRowPostion; i--)
            {
                for (var j = 0; j < 10; j++)
                {
                    if (i == 0)
                    {
                        if (Blocks[j, i] != BlockStatus.Hidden)
                        {
                            Blocks[j, i] = BlockStatus.Hidden;
                            changedBlocks.Add(new Block(j, i));
                        }
                    }
                    else
                    {
                        if (Blocks[j, i] != Blocks[j, i - 1])
                        {
                            Blocks[j, i] = Blocks[j, i - 1];
                            changedBlocks.Add(new Block(j, i, Blocks[j, i]));
                        }
                    }
                }
            }
            lowestRowPostion++;
            return changedBlocks.ToArray();
        }

        #endregion

        #region Ctor

        public Deck(int width, int height)
        {
            Width = width;
            Height = height;
            lowestRowPostion = height;
        }

        #endregion

        #region Public Properties

        public int Width { get; private set; }

        public int Height { get; private set; }

        #endregion

        #region Public Methods

        public Block[] Initialize()
        {
            Blocks = new BlockStatus[Width, Height];
            var initDeckBlocks = new Block[Width * Height];
            var blockIndex = 0;
            for (var i = 0; i < Width; i++)
            {
                for (var j = 0; j < Height; j++)
                {
                    initDeckBlocks[blockIndex++] = new Block(i, j);
                }
            }
            return initDeckBlocks;
        }

        public void FixBlocks(Block[] blocks)
        {
            foreach (var block in blocks)
            {
                Blocks[block.X, block.Y] = block.Status;
            }
            var tempLowestY = blocks.Min(s => s.Y);
            if (tempLowestY < lowestRowPostion)
                lowestRowPostion = tempLowestY;
        }

        public VanishRowResult VanishRows(int[] currentRows)
        {
            var rowsToVanish = GetRowsToVanish(currentRows);
            if (rowsToVanish.Length == 0) return null;

            var vanishedBlocks = GetVanishedBlocks(rowsToVanish);
            Array.Sort(rowsToVanish);

            var changedBlocks = new List<Block>();
            foreach (var row in rowsToVanish)
            {
                var localChangedBlocks = FallDownBlocksByRow(row);

                foreach (var block in localChangedBlocks)
                {
                    if (changedBlocks.Contains(block))
                    {
                        changedBlocks.Remove(block);
                    }
                }
                changedBlocks.AddRange(localChangedBlocks);
            }

            return new VanishRowResult
            {
                ChangedBlocks = changedBlocks.ToArray(),
                VanishedBlocks = vanishedBlocks,
                VanishedRowCount = rowsToVanish.Length
            };
        }

        public bool GameOver(int y)
        {
            return y < 0;
        }

        public bool Collision(int x, int y)
        {
            if (x < 0 || x == Width || y == Height) return true;
            if (y < 0)
            {
                return false;
            }
            return Blocks[x, y] != BlockStatus.Hidden;
        }

        public Block[] GetGhostBlocks(Block[] tetrominoBlocks)
        {
            var ghostBlocks = new List<Block>();
            foreach (var block in tetrominoBlocks)
            {
                ghostBlocks.Add(new Block(block));
            }
            while (true)
            {
                foreach (var ghostBlock in ghostBlocks)
                {
                    if (Collision(ghostBlock.X, ghostBlock.Y + 1))
                    {
                        return ghostBlocks.ToArray();
                    }
                }
                foreach (var ghostBlock in ghostBlocks)
                {
                    ghostBlock.MoveDown();
                }
            }
        }

        #endregion

    }
}
