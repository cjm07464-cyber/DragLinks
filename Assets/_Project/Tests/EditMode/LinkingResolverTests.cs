using System.Collections.Generic;
using DragLinks.Board;
using DragLinks.Linking;
using NUnit.Framework;

namespace DragLinks.Tests.EditMode
{
    public sealed class LinkingResolverTests
    {
        [Test]
        public void LinkingRemovalReusesGravityAndRefillAndFillsBoard()
        {
            var board = CreateOneLinkingRowBoard();
            var survivors = new[] { board.GetTile(0, 1), board.GetTile(1, 1), board.GetTile(2, 1) };
            var generator = new SequenceTileGenerator(
                Operator(), Operator(), Operator());

            var result = CreateResolver(generator).Resolve(board, CreateSettings());

            Assert.That(result.Waves.Count, Is.EqualTo(1));
            Assert.That(result.TotalLinkingLineCount, Is.EqualTo(1));
            Assert.That(result.Waves[0].Gravity.Movements.Count, Is.EqualTo(3));
            Assert.That(result.Waves[0].Refill.Spawns.Count, Is.EqualTo(3));
            for (var x = 0; x < 3; x++) Assert.That(board.GetTile(x, 0), Is.SameAs(survivors[x]));
            AssertBoardIsFull(board);
            Assert.That(new LinkingDetector().Detect(board).HasLinking, Is.False);
        }

        [Test]
        public void RefillCanCreateSecondWaveAndAccumulatesLineCount()
        {
            var board = CreateOneLinkingRowBoard();
            var generator = new SequenceTileGenerator(
                Number(1), Number(2), Number(3),
                Operator(), Operator(), Operator());

            var result = CreateResolver(generator).Resolve(board, CreateSettings());

            Assert.That(result.Waves.Count, Is.EqualTo(2));
            Assert.That(result.Waves[0].Detection.LineCount, Is.EqualTo(1));
            Assert.That(result.Waves[1].Detection.LineCount, Is.EqualTo(1));
            Assert.That(result.TotalLinkingLineCount, Is.EqualTo(2));
            Assert.That(result.SafetyLimitReached, Is.False);
            AssertBoardIsFull(board);
        }

        [Test]
        public void NoLinkingReturnsImmediatelyWithoutChangingBoard()
        {
            var board = new BoardState(3, 2);
            for (var y = 0; y < 2; y++)
            for (var x = 0; x < 3; x++) board.SetTile(x, y, Operator());
            var original = board.GetTile(0, 0);

            var result = CreateResolver(new SequenceTileGenerator()).Resolve(board, CreateSettings());

            Assert.That(result.Waves.Count, Is.Zero);
            Assert.That(result.TotalLinkingLineCount, Is.Zero);
            Assert.That(board.GetTile(0, 0), Is.SameAs(original));
        }

        private static LinkingResolver CreateResolver(ITileGenerator generator)
        {
            return new LinkingResolver(
                new LinkingDetector(),
                new BoardGravityResolver(),
                new BoardRefillResolver(generator));
        }

        private static BoardState CreateOneLinkingRowBoard()
        {
            var board = new BoardState(3, 2);
            for (var x = 0; x < 3; x++)
            {
                board.SetTile(x, 0, Number(x + 1));
                board.SetTile(x, 1, Operator());
            }
            return board;
        }

        private static BoardGenerationSettings CreateSettings()
        {
            return new BoardGenerationSettings(3, 2, 5, 3f, 1f,
                new[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f },
                new[] { 1f, 1f, 1f, 1f });
        }

        private static TileState Number(int value) => TileState.CreateNumber(value, (value - 1) % 9 + 1);
        private static TileState Operator() => TileState.CreateOperator(OperatorType.Add);

        private static void AssertBoardIsFull(BoardState board)
        {
            for (var y = 0; y < board.Height; y++)
            for (var x = 0; x < board.Width; x++) Assert.That(board.GetTile(x, y), Is.Not.Null);
        }

        private sealed class SequenceTileGenerator : ITileGenerator
        {
            private readonly Queue<TileState> tiles;
            public SequenceTileGenerator(params TileState[] tiles) => this.tiles = new Queue<TileState>(tiles);

            public TileState GenerateTile(BoardGenerationSettings settings)
            {
                if (tiles.Count == 0) throw new AssertionException("The deterministic tile sequence was exhausted.");
                return tiles.Dequeue();
            }
        }
    }
}
