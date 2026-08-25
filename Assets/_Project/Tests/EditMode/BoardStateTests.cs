using System;
using DragLinks.Board;
using NUnit.Framework;

namespace DragLinks.Tests.EditMode
{
    public sealed class BoardStateTests
    {
        [Test]
        public void StoresAndReturnsTileByCoordinate()
        {
            var board = new BoardState(3, 2);
            var tile = TileState.CreateNumber(11, 1);
            board.SetTile(2, 1, tile);
            Assert.That(board.GetTile(2, 1), Is.SameAs(tile));
        }

        [Test]
        public void RejectsCoordinatesOutsideBoard()
        {
            var board = new BoardState(3, 2);
            Assert.Throws<ArgumentOutOfRangeException>(() => board.GetTile(3, 0));
        }

        [Test]
        public void NumberKeepsCurrentValueSeparateFromIdentity()
        {
            var tile = TileState.CreateNumber(11, 1);
            Assert.That(tile.CurrentValue, Is.EqualTo(11));
            Assert.That(tile.NumberIdentity, Is.EqualTo(1));
            Assert.That(tile.HasHammer, Is.False);
        }
    }
}
