using System;

namespace DragLinks.Board
{
    public readonly struct BoardCoordinate : IEquatable<BoardCoordinate>
    {
        public int X { get; }
        public int Y { get; }

        public BoardCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(BoardCoordinate other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is BoardCoordinate other && Equals(other);
        public override int GetHashCode() => (X * 397) ^ Y;
        public override string ToString() => $"({X}, {Y})";
        public static bool operator ==(BoardCoordinate left, BoardCoordinate right) => left.Equals(right);
        public static bool operator !=(BoardCoordinate left, BoardCoordinate right) => !left.Equals(right);
    }
}
