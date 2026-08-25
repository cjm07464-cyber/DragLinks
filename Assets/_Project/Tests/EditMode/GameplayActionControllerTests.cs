using DragLinks.Board;
using DragLinks.Turn;
using DragLinks.Linking;
using System.Collections.Generic;
using DragLinks.Character;
using NUnit.Framework;

namespace DragLinks.Tests.EditMode
{
    public sealed class GameplayActionControllerTests
    {
        [Test]
        public void ValidActionRemovesPathSettlesBoardAndReturnsToIdle()
        {
            var settings = new BoardGenerationSettings(3, 2, 5, 3f, 1f,
                new[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f },
                new[] { 1f, 1f, 1f, 1f });
            var board = new BoardState(3, 2);
            board.SetTile(0, 0, TileState.CreateNumber(1, 1));
            board.SetTile(1, 0, TileState.CreateOperator(OperatorType.Add));
            board.SetTile(2, 0, TileState.CreateNumber(2, 2));
            var survivor = TileState.CreateNumber(9, 9);
            board.SetTile(0, 1, survivor);
            board.SetTile(1, 1, TileState.CreateOperator(OperatorType.Subtract));
            board.SetTile(2, 1, TileState.CreateOperator(OperatorType.Divide));
            var generator = new OperatorTileGenerator();
            var gravity = new BoardGravityResolver();
            var refill = new BoardRefillResolver(generator);
            var controller = new GameplayActionController(board, settings,
                gravity, refill, new LinkingResolver(new LinkingDetector(), gravity, refill),
                new ChainComboRuntimeState(), new ChainComboResolver());

            Assert.That(controller.TryBeginDrag(), Is.True);
            var result = controller.ResolveExpression(new[]
            {
                new BoardCoordinate(0, 0),
                new BoardCoordinate(1, 0),
                new BoardCoordinate(2, 0)
            });

            Assert.That(result.RemovedCoordinates.Count, Is.EqualTo(3));
            Assert.That(result.Gravity.Movements.Count, Is.EqualTo(3));
            Assert.That(result.Refill.Spawns.Count, Is.EqualTo(3));
            Assert.That(board.GetTile(0, 0), Is.SameAs(survivor));
            Assert.That(result.Linking.TotalLinkingLineCount, Is.Zero);
            Assert.That(result.ChainCombo.FinalStack, Is.Zero);
            Assert.That(controller.Phase, Is.EqualTo(TurnPhase.Idle));
            Assert.That(controller.IsInputAllowed, Is.True);
        }

        [Test]
        public void ChainComboStackPersistsAcrossGameplayActions()
        {
            var settings = CreateSettings();
            var board = new BoardState(3, 2);
            SetExpressionBoard(board);
            var generator = new SequenceTileGenerator(
                Number(3), Number(4), Number(5), Operator(), Operator(), Operator(),
                Number(6), Number(7), Number(8), Operator(), Operator(), Operator());
            var gravity = new BoardGravityResolver();
            var refill = new BoardRefillResolver(generator);
            var state = new ChainComboRuntimeState();
            var controller = new GameplayActionController(board, settings, gravity, refill,
                new LinkingResolver(new LinkingDetector(), gravity, refill), state, new ChainComboResolver());
            var path = new[]
            {
                new BoardCoordinate(0, 0), new BoardCoordinate(1, 0), new BoardCoordinate(2, 0)
            };

            controller.TryBeginDrag();
            var first = controller.ResolveExpression(path);
            Assert.That(first.ChainCombo.FinalStack, Is.EqualTo(1));

            SetExpressionBoard(board);
            controller.TryBeginDrag();
            var second = controller.ResolveExpression(path);

            Assert.That(second.ChainCombo.FinalStack, Is.EqualTo(2));
            Assert.That(state.CurrentStack, Is.EqualTo(2));
            Assert.That(state.PendingComboTriggers, Is.Zero);
        }

        [Test]
        public void LinkingWaveCompletesBeforeControllerReturnsToInputAllowedState()
        {
            var settings = CreateSettings();
            var board = new BoardState(3, 2);
            board.SetTile(0, 0, TileState.CreateNumber(1, 1));
            board.SetTile(1, 0, TileState.CreateOperator(OperatorType.Add));
            board.SetTile(2, 0, TileState.CreateNumber(2, 2));
            for (var x = 0; x < 3; x++) board.SetTile(x, 1, TileState.CreateOperator(OperatorType.Subtract));
            var generator = new SequenceTileGenerator(
                TileState.CreateNumber(3, 3), TileState.CreateNumber(4, 4), TileState.CreateNumber(5, 5),
                TileState.CreateOperator(OperatorType.Add), TileState.CreateOperator(OperatorType.Add),
                TileState.CreateOperator(OperatorType.Add));
            var gravity = new BoardGravityResolver();
            var refill = new BoardRefillResolver(generator);
            var controller = new GameplayActionController(board, settings, gravity, refill,
                new LinkingResolver(new LinkingDetector(), gravity, refill),
                new ChainComboRuntimeState(), new ChainComboResolver());

            controller.TryBeginDrag();
            var result = controller.ResolveExpression(new[]
            {
                new BoardCoordinate(0, 0), new BoardCoordinate(1, 0), new BoardCoordinate(2, 0)
            });

            Assert.That(result.Linking.Waves.Count, Is.EqualTo(1));
            Assert.That(result.Linking.TotalLinkingLineCount, Is.EqualTo(1));
            Assert.That(result.ChainCombo.Steps.Count, Is.EqualTo(1));
            Assert.That(result.ChainCombo.FinalStack, Is.EqualTo(1));
            Assert.That(result.ChainCombo.RemainingPendingTriggers, Is.Zero);
            Assert.That(controller.Phase, Is.EqualTo(TurnPhase.Idle));
            Assert.That(controller.IsInputAllowed, Is.True);
        }

        private static BoardGenerationSettings CreateSettings()
        {
            return new BoardGenerationSettings(3, 2, 5, 3f, 1f,
                new[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f },
                new[] { 1f, 1f, 1f, 1f });
        }

        private static void SetExpressionBoard(BoardState board)
        {
            board.SetTile(0, 0, Number(1));
            board.SetTile(1, 0, TileState.CreateOperator(OperatorType.Add));
            board.SetTile(2, 0, Number(2));
            for (var x = 0; x < 3; x++) board.SetTile(x, 1, Operator());
        }

        private static TileState Number(int value) => TileState.CreateNumber(value, value);
        private static TileState Operator() => TileState.CreateOperator(OperatorType.Subtract);

        private sealed class OperatorTileGenerator : ITileGenerator
        {
            public TileState GenerateTile(BoardGenerationSettings settings) =>
                TileState.CreateOperator(OperatorType.Add);
        }

        private sealed class SequenceTileGenerator : ITileGenerator
        {
            private readonly Queue<TileState> tiles;
            public SequenceTileGenerator(params TileState[] tiles) => this.tiles = new Queue<TileState>(tiles);
            public TileState GenerateTile(BoardGenerationSettings settings) => tiles.Dequeue();
        }
    }
}
