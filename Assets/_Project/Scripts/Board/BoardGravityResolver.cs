using System;
using System.Collections.Generic;

namespace DragLinks.Board
{
    public sealed class BoardGravityResolver
    {
        public BoardGravityResult Resolve(BoardState board)
        {
            if (board == null) throw new ArgumentNullException(nameof(board));
            var movements = new List<TileMovement>();

            for (var x = 0; x < board.Width; x++)
            {
                var writeY = 0;
                for (var readY = 0; readY < board.Height; readY++)
                {
                    var tile = board.GetTile(x, readY);
                    if (tile == null) continue;

                    if (readY != writeY)
                    {
                        var from = new BoardCoordinate(x, readY);
                        var to = new BoardCoordinate(x, writeY);
                        board.SetTile(to, tile);
                        board.SetTile(from, null);
                        movements.Add(new TileMovement(tile, from, to));
                    }

                    writeY++;
                }
            }

            return new BoardGravityResult(movements);
        }
    }
}
