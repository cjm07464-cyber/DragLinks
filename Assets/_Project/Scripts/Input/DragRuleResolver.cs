using System;
using DragLinks.Board;

namespace DragLinks.Input
{
    public enum DragStepResult
    {
        Rejected,
        Added,
        Backtracked
    }

    public sealed class DragRuleResolver
    {
        public bool IsAdjacent(BoardCoordinate from, BoardCoordinate to)
        {
            var dx = Math.Abs(from.X - to.X);
            var dy = Math.Abs(from.Y - to.Y);
            return dx <= 1 && dy <= 1 && dx + dy > 0;
        }

        public DragStepResult TryAdd(BoardState board, DragPath path, BoardCoordinate candidate, int maxLength)
        {
            if (board == null) throw new ArgumentNullException(nameof(board));
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (maxLength <= 0) throw new ArgumentOutOfRangeException(nameof(maxLength));
            if (!board.Contains(candidate.X, candidate.Y) || board.GetTile(candidate) == null)
                return DragStepResult.Rejected;

            if (path.Count == 0)
            {
                if (board.GetTile(candidate).Kind != TileKind.Number) return DragStepResult.Rejected;
                path.Add(candidate);
                return DragStepResult.Added;
            }

            if (!IsAdjacent(path.Last, candidate)) return DragStepResult.Rejected;

            if (path.Count >= 2 && candidate == path[path.Count - 2])
            {
                path.RemoveLast();
                return DragStepResult.Backtracked;
            }

            if (path.Count >= maxLength || path.Contains(candidate)) return DragStepResult.Rejected;

            var currentTile = board.GetTile(path.Last);
            var candidateTile = board.GetTile(candidate);
            if (currentTile.Kind == candidateTile.Kind) return DragStepResult.Rejected;
            if (WouldSelfIntersect(path, candidate)) return DragStepResult.Rejected;

            path.Add(candidate);
            return DragStepResult.Added;
        }

        private static bool WouldSelfIntersect(DragPath path, BoardCoordinate candidate)
        {
            if (path.Count < 3) return false;
            var newStart = path.Last;

            // The immediately preceding segment shares newStart and is intentionally excluded.
            for (var i = 0; i < path.Count - 2; i++)
                if (SegmentsIntersect(path[i], path[i + 1], newStart, candidate)) return true;

            return false;
        }

        private static bool SegmentsIntersect(BoardCoordinate a, BoardCoordinate b, BoardCoordinate c, BoardCoordinate d)
        {
            var o1 = Orientation(a, b, c);
            var o2 = Orientation(a, b, d);
            var o3 = Orientation(c, d, a);
            var o4 = Orientation(c, d, b);
            return o1 != o2 && o3 != o4;
        }

        private static int Orientation(BoardCoordinate a, BoardCoordinate b, BoardCoordinate c)
        {
            var cross = (long)(b.X - a.X) * (c.Y - a.Y) - (long)(b.Y - a.Y) * (c.X - a.X);
            return cross > 0 ? 1 : cross < 0 ? -1 : 0;
        }
    }
}
