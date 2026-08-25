using System;
using System.Collections.Generic;
using DragLinks.Board;

namespace DragLinks.Linking
{
    public enum LinkingLineType
    {
        Row,
        Column
    }

    public sealed class LinkingLine
    {
        public LinkingLineType Type { get; }
        public int Index { get; }
        public IReadOnlyList<BoardCoordinate> Coordinates { get; }

        public LinkingLine(LinkingLineType type, int index, IReadOnlyList<BoardCoordinate> coordinates)
        {
            Type = type;
            Index = index;
            Coordinates = coordinates ?? throw new ArgumentNullException(nameof(coordinates));
        }
    }

    public sealed class LinkingResult
    {
        public IReadOnlyList<LinkingLine> Lines { get; }
        public IReadOnlyList<BoardCoordinate> UniqueCoordinates { get; }
        public int LineCount => Lines.Count;
        public bool HasLinking => LineCount > 0;

        public LinkingResult(IReadOnlyList<LinkingLine> lines, IReadOnlyList<BoardCoordinate> uniqueCoordinates)
        {
            Lines = lines ?? throw new ArgumentNullException(nameof(lines));
            UniqueCoordinates = uniqueCoordinates ?? throw new ArgumentNullException(nameof(uniqueCoordinates));
        }
    }
}
