using System;
using System.Collections.Generic;

namespace DragLinks.Board
{
    public sealed class BoardRefillResolver
    {
        private readonly ITileGenerator generator;

        public BoardRefillResolver(ITileGenerator generator)
        {
            this.generator = generator ?? throw new ArgumentNullException(nameof(generator));
        }

        public BoardRefillResult Resolve(BoardState board, BoardGenerationSettings settings)
        {
            if (board == null) throw new ArgumentNullException(nameof(board));
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (board.Width != settings.Width || board.Height != settings.Height)
                throw new ArgumentException("Board dimensions must match the generation settings.", nameof(settings));

            var spawns = new List<TileSpawn>();
            for (var x = 0; x < board.Width; x++)
            {
                var spawnOrder = 0;
                for (var y = 0; y < board.Height; y++)
                {
                    if (board.GetTile(x, y) != null) continue;
                    var tile = generator.GenerateTile(settings);
                    var coordinate = new BoardCoordinate(x, y);
                    board.SetTile(coordinate, tile);
                    spawns.Add(new TileSpawn(tile, coordinate, spawnOrder++));
                }
            }

            return new BoardRefillResult(spawns);
        }
    }
}
