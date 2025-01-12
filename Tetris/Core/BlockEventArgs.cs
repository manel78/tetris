using System;

namespace Tetris.Game
{

    public class BlockEventArgs : EventArgs
    {

        #region Ctor

        public BlockEventArgs(Block[] blocks)
        {
            Blocks = blocks;
        }

        #endregion

        #region public Properties

        public Block[] Blocks { get; private set; }

        #endregion

    }
}
