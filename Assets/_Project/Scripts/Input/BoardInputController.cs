using DragLinks.Board;
using DragLinks.Turn;
using DragLinks.UI;
using UnityEngine;

namespace DragLinks.Input
{
    public sealed class BoardInputController : MonoBehaviour
    {
        [SerializeField] private BoardView boardView;

        private DragController dragController;
        private GameplayActionController actionController;
        private BoardState board;

        public void Initialize(BoardState boardState, int maxDragLength, GameplayActionController gameplayActionController)
        {
            board = boardState ?? throw new System.ArgumentNullException(nameof(boardState));
            actionController = gameplayActionController ?? throw new System.ArgumentNullException(nameof(gameplayActionController));
            dragController = new DragController(board, maxDragLength);
        }

        private void OnEnable()
        {
            if (boardView == null) return;
            boardView.TilePointerDown += HandlePointerDown;
            boardView.TilePointerEnter += HandlePointerEnter;
            boardView.TilePointerUp += HandlePointerUp;
        }

        private void OnDisable()
        {
            if (boardView == null) return;
            boardView.TilePointerDown -= HandlePointerDown;
            boardView.TilePointerEnter -= HandlePointerEnter;
            boardView.TilePointerUp -= HandlePointerUp;
        }

        private void HandlePointerDown(BoardCoordinate coordinate)
        {
            if (dragController == null || actionController == null || !actionController.TryBeginDrag()) return;

            var result = dragController.TryEnter(coordinate);
            if (result != DragStepResult.Added)
            {
                actionController.CancelDrag();
                dragController.Reset();
                return;
            }

            boardView.SetSelected(coordinate, true);
        }

        private void HandlePointerEnter(BoardCoordinate coordinate)
        {
            if (dragController == null || !dragController.IsDragging || !actionController.IsInputAllowed) return;
            var previousLast = dragController.Path.Last;
            var result = dragController.TryEnter(coordinate);

            if (result == DragStepResult.Added) boardView.SetSelected(coordinate, true);
            else if (result == DragStepResult.Backtracked) boardView.SetSelected(previousLast, false);
        }

        private void HandlePointerUp()
        {
            if (dragController == null || !dragController.IsDragging) return;

            if (dragController.IsValidExpression())
            {
                var result = actionController.ResolveExpression(dragController.Path.Coordinates);
                foreach (var wave in result.Linking.Waves)
                    Debug.Log($"LINKING! Wave: {wave.WaveNumber}, Lines: {wave.Detection.LineCount}", this);
                if (result.Linking.TotalLinkingLineCount > 0)
                    Debug.Log($"LINKING RESOLVED. Total Lines: {result.Linking.TotalLinkingLineCount}", this);
                foreach (var step in result.ChainCombo.Steps)
                    Debug.Log($"CHAIN COMBO! Activated Stack: {step.ActivatedStack}, " +
                              $"Current Stack: {step.CurrentStackAfterStep}", this);
                if (result.Linking.SafetyLimitReached)
                    Debug.LogError(
                        $"LINKING safety limit reached after {result.Linking.Waves.Count} waves. " +
                        "This is a technical infinite-loop guard, not a gameplay chain limit.", this);
                dragController.Reset();
                boardView.Render(board);
                return;
            }

            actionController.CancelDrag();
            dragController.Reset();
            boardView.ClearSelection();
        }

        private void OnValidate()
        {
            if (boardView == null) boardView = GetComponent<BoardView>();
        }
    }
}
