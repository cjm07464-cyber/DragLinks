using System;

namespace DragLinks.Board
{
    /// <summary>좌표별 타일을 소유하는 실제 보드 상태의 Source of Truth다.</summary>
    public sealed class BoardState
    {
        private readonly TileState[,] tiles;

        public int Width { get; }
        public int Height { get; }

        public BoardState(int width, int height)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            Width = width;
            Height = height;
            tiles = new TileState[width, height];
        }

        public bool Contains(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

        public TileState GetTile(int x, int y)
        {
            ValidateCoordinates(x, y);
            return tiles[x, y];
        }

        public TileState GetTile(BoardCoordinate coordinate) => GetTile(coordinate.X, coordinate.Y);

        public void SetTile(int x, int y, TileState tile)
        {
            ValidateCoordinates(x, y);
            tiles[x, y] = tile;
        }

        public void SetTile(BoardCoordinate coordinate, TileState tile) => SetTile(coordinate.X, coordinate.Y, tile);

        public TileState RemoveTile(BoardCoordinate coordinate)
        {
            var tile = GetTile(coordinate);
            SetTile(coordinate, null);
            return tile;
        }

        private void ValidateCoordinates(int x, int y)
        {
            if (!Contains(x, y))
                throw new ArgumentOutOfRangeException(nameof(x), $"Board coordinate ({x}, {y}) is outside {Width}x{Height}.");
        }
    }
}
