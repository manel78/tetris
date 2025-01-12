using System;
using System.Collections.Generic;
using System.Linq;
using Tetris.Game.Results;
using Tetris.Game.Tetriminoes;

namespace Tetris.Game
{

    internal class TetrominoHandler
    {

        #region Private Variables

        private Tetromino current;

        private Queue<Tetromino> next;

        private readonly Deck deck;

        private Block[] ghostBlocks;

        private bool holdIsPossible;

        private Tetromino held;

        private readonly Tetrominos7BagRandomizer tetrominos7Bag;

        private readonly int nextTetrominoesQueueLenght = 5;

        #endregion

        #region Private Methods

        private Tetromino GenerateNewTetromino()
        {
            return tetrominos7Bag.GetNewTetromino();
        }

        private void CalculateGhostBlock(ChangeResult moveResult)
        {
            if (moveResult == null || !GhostBlocksActiveStatus) return;
            var changedGhostBlocks = new List<Block>();
            foreach (var ghostBlock in ghostBlocks)
            {
                changedGhostBlocks.Add(new Block(ghostBlock, BlockStatus.Hidden));
            }
            ghostBlocks = deck.GetGhostBlocks(current.VisibleBlocks);
            changedGhostBlocks.AddRange(ghostBlocks);
            moveResult.GhostBlocks = changedGhostBlocks.ToArray();
        }

        private void SetLastMove(ChangeResult moveResult)
        {
            if (moveResult == null) return;

            moveResult.LastMove = !current.CanMoveDown();
        }

        #endregion

        #region Ctor

        public TetrominoHandler(Deck deck)
        {
            this.deck = deck;
            tetrominos7Bag = new Tetrominos7BagRandomizer(deck);
            next = new Queue<Tetromino>(nextTetrominoesQueueLenght);
            holdIsPossible = true;
        }

        #endregion

        #region Public Properties

        public bool GhostBlocksActiveStatus { get; private set; } = true;

        #endregion

        #region Public Methods

        public TetrominoInitializationResult Initialize()
        {
            current = GenerateNewTetromino();
            for (var i = 0; i < nextTetrominoesQueueLenght; i++)
            {
                next.Enqueue(GenerateNewTetromino());
            }
            ghostBlocks = deck.GetGhostBlocks(current.VisibleBlocks);

            return new TetrominoInitializationResult
            {
                ChangedBlocks = current.VisibleBlocks,
                NextTetrominoes = next.Select(s => s.BaseBlocks).ToList(),
                GhostBlocks = ghostBlocks
            };
        }

        public ChangeResult MoveRight()
        {
            var moveResult = current.MoveRight();
            SetLastMove(moveResult);
            CalculateGhostBlock(moveResult);
            return moveResult;
        }

        public MoveDownResult MoveDown()
        {
            var moveDownResult = current.MoveDown();

            if (moveDownResult.GameOver || moveDownResult.ChangedBlocks != null)
            {
                SetLastMove(moveDownResult);
                return moveDownResult;
            }

            current = next.Dequeue();
            next.Enqueue(GenerateNewTetromino());

            moveDownResult.ChangedBlocks = current.VisibleBlocks;

            moveDownResult.NextTetrominoes = next.Select(s => s.BaseBlocks).ToList();
            CalculateGhostBlock(moveDownResult);
            holdIsPossible = true;
            SetLastMove(moveDownResult);
            return moveDownResult;
        }

        public ChangeResult MoveLeft()
        {
            var moveResult = current.MoveLeft();
            SetLastMove(moveResult);
            CalculateGhostBlock(moveResult);
            return moveResult;
        }

        public ChangeResult[] Rotate()
        {
            var rotateResult = current.Rotate();
            if (rotateResult.Length != 0)
            {
                CalculateGhostBlock(rotateResult.Last());
                SetLastMove(rotateResult.Last());
            }
            return rotateResult;
        }

        public ChangeResult ActiveGhostBlocks()
        {
            GhostBlocksActiveStatus = true;
            ghostBlocks = deck.GetGhostBlocks(current.VisibleBlocks);
            return new ChangeResult { ChangedBlocks = ghostBlocks };
        }

        public ChangeResult DeactiveGhostBlocks()
        {
            GhostBlocksActiveStatus = false;
            var hiddenGhostBlocks = new List<Block>();
            foreach (var item in ghostBlocks)
            {
                hiddenGhostBlocks.Add(new Block(item, BlockStatus.Hidden));
            }
            return new ChangeResult { ChangedBlocks = hiddenGhostBlocks.ToArray() };
        }

        public HoldResult Hold()
        {
            if (!holdIsPossible)
            {
                return null;
            }
            
            var changedBlocks = new List<Block>();
            
            foreach (var item in current.VisibleBlocks)
            {
                changedBlocks.Add(new Block(item, BlockStatus.Hidden));
            }

            var holdResult = new HoldResult();

            if (held == null)
            {    
                held = current;
                current = next.Dequeue();
                next.Enqueue(GenerateNewTetromino());  
                holdResult.NextTetrominoes =  next.Select(s => s.BaseBlocks).ToList();
            }
            else
            {
                var tempTetro = held;
                held = current;
                current = tempTetro;
            }

            held.ResetToTopMiddle();
            foreach (var item in current.VisibleBlocks)
            {
                changedBlocks.Add(new Block(item));
            }

            holdResult.HoldBlocks = held.BaseBlocks;
            holdResult.ChangedBlocks = changedBlocks.ToArray();
            CalculateGhostBlock(holdResult);
            SetLastMove(holdResult);
            holdIsPossible = false;
            return holdResult;
        }

        #endregion

    }
}
