using System;
using System.Collections.Generic;
using DragLinks.Board;

namespace DragLinks.Input
{
    public sealed class DragPath
    {
        private readonly List<BoardCoordinate> coordinates = new List<BoardCoordinate>();

        public int Count => coordinates.Count;
        public IReadOnlyList<BoardCoordinate> Coordinates => coordinates;
        public BoardCoordinate Last => coordinates[coordinates.Count - 1];

        public BoardCoordinate this[int index] => coordinates[index];
        public bool Contains(BoardCoordinate coordinate) => coordinates.Contains(coordinate);

        public void Add(BoardCoordinate coordinate) => coordinates.Add(coordinate);

        public BoardCoordinate RemoveLast()
        {
            if (coordinates.Count == 0) throw new InvalidOperationException("The drag path is empty.");
            var removed = coordinates[coordinates.Count - 1];
            coordinates.RemoveAt(coordinates.Count - 1);
            return removed;
        }

        public void Clear() => coordinates.Clear();
    }
}
