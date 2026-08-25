using System.Collections.Generic;

namespace DragLinks.Board
{
    public readonly struct TileMovement
    {
        public TileState Tile { get; }
        public BoardCoordinate From { get; }
        public BoardCoordinate To { get; }

        public TileMovement(TileState tile, BoardCoordinate from, BoardCoordinate to)
        {
            Tile = tile;
            From = from;
            To = to;
        }
    }

    public readonly struct TileSpawn
    {
        public TileState Tile { get; }
        public BoardCoordinate Coordinate { get; }
        public int SpawnOrderInColumn { get; }

        public TileSpawn(TileState tile, BoardCoordinate coordinate, int spawnOrderInColumn)
        {
            Tile = tile;
            Coordinate = coordinate;
            SpawnOrderInColumn = spawnOrderInColumn;
        }
    }

    public sealed class BoardGravityResult
    {
        public IReadOnlyList<TileMovement> Movements { get; }
        public BoardGravityResult(IReadOnlyList<TileMovement> movements) => Movements = movements;
    }

    public sealed class BoardRefillResult
    {
        public IReadOnlyList<TileSpawn> Spawns { get; }
        public BoardRefillResult(IReadOnlyList<TileSpawn> spawns) => Spawns = spawns;
    }
}
