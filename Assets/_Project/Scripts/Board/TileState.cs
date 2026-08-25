using System;

namespace DragLinks.Board
{
    public enum TileKind
    {
        Number,
        Operator
    }

    public enum OperatorType
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }

    /// <summary>보드 규칙이 사용하는 타일의 런타임 상태다.</summary>
    public sealed class TileState
    {
        public TileKind Kind { get; }
        public int CurrentValue { get; }
        public int NumberIdentity { get; }
        public OperatorType OperatorType { get; }
        public bool HasGem { get; }
        public bool HasHammer { get; }

        private TileState(
            TileKind kind,
            int currentValue,
            int numberIdentity,
            OperatorType operatorType,
            bool hasGem,
            bool hasHammer)
        {
            Kind = kind;
            CurrentValue = currentValue;
            NumberIdentity = numberIdentity;
            OperatorType = operatorType;
            HasGem = hasGem;
            HasHammer = hasHammer;
        }

        public static TileState CreateNumber(int currentValue, int numberIdentity, bool hasGem = false)
        {
            if (numberIdentity < 1 || numberIdentity > 9)
                throw new ArgumentOutOfRangeException(nameof(numberIdentity), "NumberIdentity must be between 1 and 9.");

            return new TileState(TileKind.Number, currentValue, numberIdentity, default, hasGem, false);
        }

        public static TileState CreateOperator(OperatorType operatorType, bool hasHammer = false)
        {
            if (!Enum.IsDefined(typeof(OperatorType), operatorType))
                throw new ArgumentOutOfRangeException(nameof(operatorType));

            return new TileState(TileKind.Operator, 0, 0, operatorType, false, hasHammer);
        }
    }
}
