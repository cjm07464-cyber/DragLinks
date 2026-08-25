using System;
using DragLinks.Board;

namespace DragLinks.Input
{
    public sealed class DragController
    {
        private readonly BoardState board;
        private readonly DragRuleResolver rules;
        private readonly ExpressionPathValidator validator;
        private readonly int maxLength;

        public DragPath Path { get; } = new DragPath();
        public bool IsDragging => Path.Count > 0;

        public DragController(BoardState board, int maxLength,
            DragRuleResolver rules = null, ExpressionPathValidator validator = null)
        {
            this.board = board ?? throw new ArgumentNullException(nameof(board));
            this.maxLength = maxLength > 0 ? maxLength : throw new ArgumentOutOfRangeException(nameof(maxLength));
            this.rules = rules ?? new DragRuleResolver();
            this.validator = validator ?? new ExpressionPathValidator();
        }

        public DragStepResult TryEnter(BoardCoordinate coordinate) => rules.TryAdd(board, Path, coordinate, maxLength);
        public bool IsValidExpression() => validator.IsValid(board, Path);
        public void Reset() => Path.Clear();
    }
}
