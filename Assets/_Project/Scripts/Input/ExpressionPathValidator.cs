using System;
using DragLinks.Board;

namespace DragLinks.Input
{
    public sealed class ExpressionPathValidator
    {
        public bool IsValid(BoardState board, DragPath path)
        {
            if (board == null) throw new ArgumentNullException(nameof(board));
            if (path == null || path.Count < 3 || path.Count % 2 == 0) return false;

            for (var i = 0; i < path.Count; i++)
            {
                var tile = board.GetTile(path[i]);
                if (tile == null) return false;
                var expected = i % 2 == 0 ? TileKind.Number : TileKind.Operator;
                if (tile.Kind != expected) return false;
            }

            return true;
        }
    }
}
