using DragLinks.Board;
using NUnit.Framework;

namespace DragLinks.Tests.EditMode
{
    public sealed class BoardGeneratorTests
    {
        [Test]
        public void GeneratesConfiguredBoardDimensionsAndValidTiles()
        {
            var settings = new BoardGenerationSettings(4, 3, 5, 3f, 1f,
                new[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f },
                new[] { 1f, 1f, 1f, 1f });
            var board = new BoardGenerator(new SeededRandomSource(42)).Generate(settings);

            Assert.That(board.Width, Is.EqualTo(4));
            Assert.That(board.Height, Is.EqualTo(3));
            for (var y = 0; y < board.Height; y++)
            for (var x = 0; x < board.Width; x++)
            {
                var tile = board.GetTile(x, y);
                Assert.That(tile, Is.Not.Null);
                if (tile.Kind == TileKind.Number)
                    Assert.That(tile.NumberIdentity, Is.InRange(1, 9));
                else
                    Assert.That((int)tile.OperatorType, Is.InRange(0, 3));
            }
        }

        [Test]
        public void SeedProducesReproducibleBoard()
        {
            var settings = new BoardGenerationSettings(5, 2, 5, 1f, 1f,
                new[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f },
                new[] { 1f, 1f, 1f, 1f });
            var first = new BoardGenerator(new SeededRandomSource(7)).Generate(settings);
            var second = new BoardGenerator(new SeededRandomSource(7)).Generate(settings);

            for (var y = 0; y < first.Height; y++)
            for (var x = 0; x < first.Width; x++)
            {
                Assert.That(second.GetTile(x, y).Kind, Is.EqualTo(first.GetTile(x, y).Kind));
                Assert.That(second.GetTile(x, y).CurrentValue, Is.EqualTo(first.GetTile(x, y).CurrentValue));
                Assert.That(second.GetTile(x, y).OperatorType, Is.EqualTo(first.GetTile(x, y).OperatorType));
            }
        }
    }
}
