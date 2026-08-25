using DragLinks.Board;
using DragLinks.Input;
using NUnit.Framework;

namespace DragLinks.Tests.EditMode
{
    public sealed class DragRuleResolverTests
    {
        [TestCase(0, 1)]
        [TestCase(0, -1)]
        [TestCase(1, 0)]
        [TestCase(-1, 0)]
        [TestCase(1, 1)]
        [TestCase(-1, 1)]
        [TestCase(1, -1)]
        [TestCase(-1, -1)]
        public void AllowsAllEightAdjacentDirections(int dx, int dy)
        {
            Assert.That(new DragRuleResolver().IsAdjacent(
                new BoardCoordinate(2, 2), new BoardCoordinate(2 + dx, 2 + dy)), Is.True);
        }

        [TestCase(2, 0)]
        [TestCase(0, -2)]
        [TestCase(2, 2)]
        public void RejectsCoordinatesMoreThanOneCellAway(int dx, int dy)
        {
            Assert.That(new DragRuleResolver().IsAdjacent(
                new BoardCoordinate(2, 2), new BoardCoordinate(2 + dx, 2 + dy)), Is.False);
        }

        [Test]
        public void AllowsNumberOperatorAndOperatorNumberButRejectsSameKinds()
        {
            var board = CreateAlternatingRow(4);
            var controller = new DragController(board, 5);

            Assert.That(controller.TryEnter(new BoardCoordinate(0, 0)), Is.EqualTo(DragStepResult.Added));
            Assert.That(controller.TryEnter(new BoardCoordinate(1, 0)), Is.EqualTo(DragStepResult.Added));
            Assert.That(controller.TryEnter(new BoardCoordinate(2, 0)), Is.EqualTo(DragStepResult.Added));

            controller.Reset();
            Assert.That(controller.TryEnter(new BoardCoordinate(2, 0)), Is.EqualTo(DragStepResult.Added));
            Assert.That(controller.TryEnter(new BoardCoordinate(0, 0)), Is.EqualTo(DragStepResult.Rejected));

            var sameOperators = new BoardState(2, 1);
            sameOperators.SetTile(0, 0, TileState.CreateNumber(1, 1));
            sameOperators.SetTile(1, 0, TileState.CreateNumber(2, 2));
            var sameController = new DragController(sameOperators, 5);
            sameController.TryEnter(new BoardCoordinate(0, 0));
            Assert.That(sameController.TryEnter(new BoardCoordinate(1, 0)), Is.EqualTo(DragStepResult.Rejected));

            var operatorBoard = new BoardState(3, 1);
            operatorBoard.SetTile(0, 0, TileState.CreateNumber(1, 1));
            operatorBoard.SetTile(1, 0, TileState.CreateOperator(OperatorType.Add));
            operatorBoard.SetTile(2, 0, TileState.CreateOperator(OperatorType.Subtract));
            var operatorController = new DragController(operatorBoard, 5);
            operatorController.TryEnter(new BoardCoordinate(0, 0));
            operatorController.TryEnter(new BoardCoordinate(1, 0));
            Assert.That(operatorController.TryEnter(new BoardCoordinate(2, 0)), Is.EqualTo(DragStepResult.Rejected));
        }

        [Test]
        public void RejectsReuseButBacktracksToImmediatelyPreviousTile()
        {
            var controller = new DragController(CreateAlternatingRow(4), 5);
            controller.TryEnter(new BoardCoordinate(0, 0));
            controller.TryEnter(new BoardCoordinate(1, 0));
            controller.TryEnter(new BoardCoordinate(2, 0));

            Assert.That(controller.TryEnter(new BoardCoordinate(0, 0)), Is.EqualTo(DragStepResult.Rejected));
            Assert.That(controller.TryEnter(new BoardCoordinate(1, 0)), Is.EqualTo(DragStepResult.Backtracked));
            Assert.That(controller.Path.Count, Is.EqualTo(2));
        }

        [Test]
        public void EnforcesConfiguredMaximumLength()
        {
            var controller = new DragController(CreateAlternatingRow(6), 5);
            for (var x = 0; x < 5; x++)
                Assert.That(controller.TryEnter(new BoardCoordinate(x, 0)), Is.EqualTo(DragStepResult.Added));

            Assert.That(controller.TryEnter(new BoardCoordinate(5, 0)), Is.EqualTo(DragStepResult.Rejected));
            Assert.That(controller.Path.Count, Is.EqualTo(5));
        }

        [Test]
        public void RejectsSelfIntersectingDiagonalSegment()
        {
            var board = new BoardState(2, 2);
            board.SetTile(0, 0, TileState.CreateNumber(1, 1));
            board.SetTile(1, 1, TileState.CreateOperator(OperatorType.Add));
            board.SetTile(0, 1, TileState.CreateNumber(2, 2));
            board.SetTile(1, 0, TileState.CreateOperator(OperatorType.Subtract));
            var controller = new DragController(board, 5);

            controller.TryEnter(new BoardCoordinate(0, 0));
            controller.TryEnter(new BoardCoordinate(1, 1));
            controller.TryEnter(new BoardCoordinate(0, 1));
            Assert.That(controller.TryEnter(new BoardCoordinate(1, 0)), Is.EqualTo(DragStepResult.Rejected));
        }

        [Test]
        public void ValidatesCompleteExpressionAndRejectsIncompleteExpression()
        {
            var controller = new DragController(CreateAlternatingRow(3), 5);
            controller.TryEnter(new BoardCoordinate(0, 0));
            controller.TryEnter(new BoardCoordinate(1, 0));
            Assert.That(controller.IsValidExpression(), Is.False);

            controller.TryEnter(new BoardCoordinate(2, 0));
            Assert.That(controller.IsValidExpression(), Is.True);
        }

        private static BoardState CreateAlternatingRow(int width)
        {
            var board = new BoardState(width, 1);
            for (var x = 0; x < width; x++)
                board.SetTile(x, 0, x % 2 == 0
                    ? TileState.CreateNumber(x + 1, x % 9 + 1)
                    : TileState.CreateOperator(OperatorType.Add));
            return board;
        }
    }
}
