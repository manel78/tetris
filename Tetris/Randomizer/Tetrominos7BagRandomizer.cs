using System;
using System.Collections.Generic;
using Tetris.Game.Tetriminoes;

namespace Tetris.Game
{

    internal class Tetrominos7BagRandomizer
    {


        #region Private Variables

        private readonly List<Tetromino> tetrominoesBag = new List<Tetromino>(7);

        private readonly Random randomGenerator = new Random();

        private readonly Deck deck;

        #endregion

        #region Private Methods

        private void FillBag()
        {
            tetrominoesBag.Add(new OTetromino(deck));
            tetrominoesBag.Add(new ITetromino(deck));
            tetrominoesBag.Add(new LTetromino(deck));
            tetrominoesBag.Add(new ZTetromino(deck));
            tetrominoesBag.Add(new STetromino(deck));
            tetrominoesBag.Add(new JTetromino(deck));
            tetrominoesBag.Add(new TTetromino(deck));
        }

        #endregion

        #region Ctor

        public Tetrominos7BagRandomizer(Deck deck)
        {
            this.deck = deck;
        }

        #endregion

        #region Public Methods

        public Tetromino GetNewTetromino()
        {
            if (tetrominoesBag.Count == 0)
            {
                FillBag();
            }
            var number = randomGenerator.Next(tetrominoesBag.Count);
            var tetromino = tetrominoesBag[number];
            tetrominoesBag.RemoveAt(number);
            return tetromino;
        }

        #endregion

    }
}