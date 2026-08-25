using DragLinks.Board;
using NUnit.Framework;

namespace DragLinks.Tests.EditMode
{
    public sealed class BoardSettleResolverTests
    {
        [Test]
        public void GravityMovesTileToLowestEmptyCoordinateAndReportsMovement()
        {
            var board = new BoardState(1, 4);
            var bottom = TileState.CreateNumber(1, 1);
            var top = TileState.CreateNumber(9, 9);
            board.SetTile(0, 0, bottom);
            board.SetTile(0, 3, top);

            var result = new BoardGravityResolver().Resolve(board);

            Assert.That(board.GetTile(0, 0), Is.SameAs(bottom));
            Assert.That(board.GetTile(0, 1), Is.SameAs(top));
            Assert.That(board.GetTile(0, 2), Is.Null);
            Assert.That(result.Movements.Count, Is.EqualTo(1));
            Assert.That(result.Movements[0].From, Is.EqualTo(new BoardCoordinate(0, 3)));
            Assert.That(result.Movements[0].To, Is.EqualTo(new BoardCoordinate(0, 1)));
        }

        [Test]
        public void GravityCompactsColumnContainingMultipleGapsInOriginalOrder()
        {
            var board = new BoardState(1, 5);
            var first = TileState.CreateNumber(2, 2);
            var second = TileState.CreateOperator(OperatorType.Multiply);
            var third = TileState.CreateNumber(7, 7);
            board.SetTile(0, 1, first);
            board.SetTile(0, 3, second);
            board.SetTile(0, 4, third);

            new BoardGravityResolver().Resolve(board);

            Assert.That(board.GetTile(0, 0), Is.SameAs(first));
            Assert.That(board.GetTile(0, 1), Is.SameAs(second));
            Assert.That(board.GetTile(0, 2), Is.SameAs(third));
            Assert.That(board.GetTile(0, 3), Is.Null);
            Assert.That(board.GetTile(0, 4), Is.Null);
        }

        [Test]
        public void RefillFillsEveryEmptyCell()
        {
            var settings = CreateSettings(3, 3);
            var board = new BoardState(3, 3);
            board.SetTile(0, 0, TileState.CreateNumber(1, 1));
            var generator = new BoardGenerator(new SeededRandomSource(17));

            var result = new BoardRefillResolver(generator).Resolve(board, settings);

            Assert.That(result.Spawns.Count, Is.EqualTo(8));
            for (var y = 0; y < board.Height; y++)
            for (var x = 0; x < board.Width; x++)
                Assert.That(board.GetTile(x, y), Is.Not.Null);
        }

        private static BoardGenerationSettings CreateSettings(int width, int height)
        {
            return new BoardGenerationSettings(width, height, 5, 3f, 1f,
                new[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f },
                new[] { 1f, 1f, 1f, 1f });
        }
    }
}
