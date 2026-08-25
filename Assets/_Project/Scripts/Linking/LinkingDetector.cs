using System;
using System.Collections.Generic;
using DragLinks.Board;

namespace DragLinks.Linking
{
    public sealed class LinkingDetector
    {
        public LinkingResult Detect(BoardState board)
        {
            if (board == null) throw new ArgumentNullException(nameof(board));
            var lines = new List<LinkingLine>();
            var uniqueCoordinates = new HashSet<BoardCoordinate>();

            for (var y = 0; y < board.Height; y++)
            {
                var coordinates = CollectRowIfAllNumbers(board, y);
                if (coordinates == null) continue;
                lines.Add(new LinkingLine(LinkingLineType.Row, y, coordinates));
                foreach (var coordinate in coordinates) uniqueCoordinates.Add(coordinate);
            }

            for (var x = 0; x < board.Width; x++)
            {
                var coordinates = CollectColumnIfAllNumbers(board, x);
                if (coordinates == null) continue;
                lines.Add(new LinkingLine(LinkingLineType.Column, x, coordinates));
                foreach (var coordinate in coordinates) uniqueCoordinates.Add(coordinate);
            }

            return new LinkingResult(lines, new List<BoardCoordinate>(uniqueCoordinates));
        }

        private static IReadOnlyList<BoardCoordinate> CollectRowIfAllNumbers(BoardState board, int y)
        {
            var coordinates = new List<BoardCoordinate>(board.Width);
            for (var x = 0; x < board.Width; x++)
            {
                var coordinate = new BoardCoordinate(x, y);
                var tile = board.GetTile(coordinate);
                if (tile == null || tile.Kind != TileKind.Number) return null;
                coordinates.Add(coordinate);
            }
            return coordinates;
        }

        private static IReadOnlyList<BoardCoordinate> CollectColumnIfAllNumbers(BoardState board, int x)
        {
            var coordinates = new List<BoardCoordinate>(board.Height);
            for (var y = 0; y < board.Height; y++)
            {
                var coordinate = new BoardCoordinate(x, y);
                var tile = board.GetTile(coordinate);
                if (tile == null || tile.Kind != TileKind.Number) return null;
                coordinates.Add(coordinate);
            }
            return coordinates;
        }
    }
}
