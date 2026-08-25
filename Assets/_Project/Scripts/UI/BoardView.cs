using System;
using System.Collections.Generic;
using DragLinks.Board;
using UnityEngine;

namespace DragLinks.UI
{
    public sealed class BoardView : MonoBehaviour
    {
        [SerializeField] private RectTransform boardRoot;
        [SerializeField] private TileView tilePrefab;
        [SerializeField] private Vector2 tileSize = new Vector2(80f, 80f);
        [SerializeField] private Vector2 spacing = new Vector2(8f, 8f);

        private readonly List<TileView> spawnedTiles = new List<TileView>();
        private readonly Dictionary<BoardCoordinate, TileView> tilesByCoordinate = new Dictionary<BoardCoordinate, TileView>();

        public event Action<BoardCoordinate> TilePointerDown;
        public event Action<BoardCoordinate> TilePointerEnter;
        public event Action TilePointerUp;

        public void Render(BoardState board)
        {
            if (board == null) throw new System.ArgumentNullException(nameof(board));
            if (boardRoot == null || tilePrefab == null)
                throw new System.InvalidOperationException("BoardRoot and TilePrefab must be assigned.");

            Clear();
            var step = tileSize + spacing;
            var boardSize = new Vector2(
                board.Width * tileSize.x + (board.Width - 1) * spacing.x,
                board.Height * tileSize.y + (board.Height - 1) * spacing.y);
            boardRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, boardSize.x);
            boardRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, boardSize.y);

            for (var y = 0; y < board.Height; y++)
            for (var x = 0; x < board.Width; x++)
            {
                var view = Instantiate(tilePrefab, boardRoot);
                var rect = (RectTransform)view.transform;
                rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = tileSize;
                rect.anchoredPosition = new Vector2(
                    (x - (board.Width - 1) * 0.5f) * step.x,
                    (y - (board.Height - 1) * 0.5f) * step.y);
                var coordinate = new BoardCoordinate(x, y);
                view.Initialize(coordinate, HandlePointerDown, HandlePointerEnter, HandlePointerUp);
                view.Render(board.GetTile(x, y));
                spawnedTiles.Add(view);
                tilesByCoordinate.Add(coordinate, view);
            }
        }

        public void SetSelected(BoardCoordinate coordinate, bool selected)
        {
            if (tilesByCoordinate.TryGetValue(coordinate, out var tile)) tile.SetSelected(selected);
        }

        public void ClearSelection()
        {
            foreach (var tile in spawnedTiles)
                if (tile != null) tile.SetSelected(false);
        }

        public void Clear()
        {
            foreach (var tile in spawnedTiles)
                if (tile != null) Destroy(tile.gameObject);
            spawnedTiles.Clear();
            tilesByCoordinate.Clear();
        }

        private void HandlePointerDown(BoardCoordinate coordinate) => TilePointerDown?.Invoke(coordinate);
        private void HandlePointerEnter(BoardCoordinate coordinate) => TilePointerEnter?.Invoke(coordinate);
        private void HandlePointerUp() => TilePointerUp?.Invoke();
    }
}
