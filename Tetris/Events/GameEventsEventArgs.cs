using System;

namespace Tetris.Game
{
    public class GameEventsEventArgs : EventArgs
    {

        #region Ctor

        public GameEventsEventArgs(GameEvent gameEvent)
        {
            Event = gameEvent;
        }

        #endregion

        #region Public Properties

        public GameEvent Event { get; set; } 

        #endregion

    }
}