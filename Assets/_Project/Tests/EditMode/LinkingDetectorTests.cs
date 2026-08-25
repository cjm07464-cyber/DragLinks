using DragLinks.Board;
using DragLinks.Linking;
using NUnit.Framework;

namespace DragLinks.Tests.EditMode
{
    public sealed class LinkingDetectorTests
    {
        [Test]
        public void DetectsAllNumberRowRegardlessOfValueAndIdentity()
        {
            var board = CreateOperatorBoard(4, 3);
            board.SetTile(0, 1, TileState.CreateNumber(1, 1));
            board.SetTile(1, 1, TileState.CreateNumber(7, 5));
            board.SetTile(2, 1, TileState.CreateNumber(21, 2));
            board.SetTile(3, 1, TileState.CreateNumber(42, 9));

            var result = new LinkingDetector().Detect(board);

            Assert.That(result.LineCount, Is.EqualTo(1));
            Assert.That(result.Lines[0].Type, Is.EqualTo(LinkingLineType.Row));
            Assert.That(result.UniqueCoordinates.Count, Is.EqualTo(4));
        }

        [Test]
        public void DetectsAllNumberColumn()
        {
            var board = CreateOperatorBoard(3, 4);
            for (var y = 0; y < board.Height; y++)
                board.SetTile(2, y, TileState.CreateNumber(y + 10, y + 1));

            var result = new LinkingDetector().Detect(board);

            Assert.That(result.LineCount, Is.EqualTo(1));
            Assert.That(result.Lines[0].Type, Is.EqualTo(LinkingLineType.Column));
        }

        [Test]
        public void OperatorInsideLinePreventsLinking()
        {
            var board = CreateOperatorBoard(4, 2);
            for (var x = 0; x < board.Width; x++)
                board.SetTile(x, 0, TileState.CreateNumber(x + 1, x + 1));
            board.SetTile(2, 0, TileState.CreateOperator(OperatorType.Add));

            Assert.That(new LinkingDetector().Detect(board).LineCount, Is.Zero);
        }

        [Test]
        public void NumberDiagonalDoesNotCreateLinking()
        {
            var board = CreateOperatorBoard(3, 3);
            for (var index = 0; index < 3; index++)
                board.SetTile(index, index, TileState.CreateNumber(index + 1, index + 1));

            Assert.That(new LinkingDetector().Detect(board).LineCount, Is.Zero);
        }

        [Test]
        public void CrossingRowAndColumnCountAsTwoButShareRemovalCoordinateOnce()
        {
            var board = CreateOperatorBoard(3, 3);
            for (var x = 0; x < 3; x++) board.SetTile(x, 1, TileState.CreateNumber(x + 1, x + 1));
            for (var y = 0; y < 3; y++) board.SetTile(1, y, TileState.CreateNumber(y + 4, y + 4));

            var result = new LinkingDetector().Detect(board);

            Assert.That(result.LineCount, Is.EqualTo(2));
            Assert.That(result.UniqueCoordinates.Count, Is.EqualTo(5));
            var centerOccurrences = 0;
            foreach (var coordinate in result.UniqueCoordinates)
                if (coordinate == new BoardCoordinate(1, 1)) centerOccurrences++;
            Assert.That(centerOccurrences, Is.EqualTo(1));
        }

        [Test]
        public void DetectsMultipleRowsAndColumnsAtOnce()
        {
            var board = new BoardState(3, 2);
            for (var y = 0; y < board.Height; y++)
            for (var x = 0; x < board.Width; x++)
                board.SetTile(x, y, TileState.CreateNumber(x + y + 1, (x + y) % 9 + 1));

            var result = new LinkingDetector().Detect(board);

            Assert.That(result.LineCount, Is.EqualTo(5));
            Assert.That(result.UniqueCoordinates.Count, Is.EqualTo(6));
        }

        private static BoardState CreateOperatorBoard(int width, int height)
        {
            var board = new BoardState(width, height);
            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                board.SetTile(x, y, TileState.CreateOperator(OperatorType.Add));
            return board;
        }
    }
}
