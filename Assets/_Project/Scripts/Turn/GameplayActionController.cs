using System;
using System.Collections.Generic;
using DragLinks.Board;
using DragLinks.Linking;
using DragLinks.Character;

namespace DragLinks.Turn
{
    public enum TurnPhase
    {
        Idle,
        Dragging,
        ResolvingAction,
        SettlingBoard,
        ResolvingLinking,
        ResolvingChainCombo
    }

    public sealed class GameplayActionResult
    {
        public IReadOnlyList<BoardCoordinate> RemovedCoordinates { get; }
        public BoardGravityResult Gravity { get; }
        public BoardRefillResult Refill { get; }
        public LinkingResolutionResult Linking { get; }
        public ChainComboSettlementResult ChainCombo { get; }

        public GameplayActionResult(IReadOnlyList<BoardCoordinate> removedCoordinates,
            BoardGravityResult gravity, BoardRefillResult refill, LinkingResolutionResult linking,
            ChainComboSettlementResult chainCombo)
        {
            RemovedCoordinates = removedCoordinates;
            Gravity = gravity;
            Refill = refill;
            Linking = linking;
            ChainCombo = chainCombo;
        }
    }

    /// <summary>현재 범위의 유효 Action 처리 순서만 명시적으로 지휘한다.</summary>
    public sealed class GameplayActionController
    {
        private readonly BoardState board;
        private readonly BoardGenerationSettings settings;
        private readonly BoardGravityResolver gravityResolver;
        private readonly BoardRefillResolver refillResolver;
        private readonly LinkingResolver linkingResolver;
        private readonly ChainComboRuntimeState chainComboState;
        private readonly ChainComboResolver chainComboResolver;

        public TurnPhase Phase { get; private set; } = TurnPhase.Idle;
        public bool IsInputAllowed => Phase == TurnPhase.Idle || Phase == TurnPhase.Dragging;
        public ChainComboRuntimeState ChainComboState => chainComboState;

        public GameplayActionController(BoardState board, BoardGenerationSettings settings,
            BoardGravityResolver gravityResolver, BoardRefillResolver refillResolver,
            LinkingResolver linkingResolver, ChainComboRuntimeState chainComboState,
            ChainComboResolver chainComboResolver)
        {
            this.board = board ?? throw new ArgumentNullException(nameof(board));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.gravityResolver = gravityResolver ?? throw new ArgumentNullException(nameof(gravityResolver));
            this.refillResolver = refillResolver ?? throw new ArgumentNullException(nameof(refillResolver));
            this.linkingResolver = linkingResolver ?? throw new ArgumentNullException(nameof(linkingResolver));
            this.chainComboState = chainComboState ?? throw new ArgumentNullException(nameof(chainComboState));
            this.chainComboResolver = chainComboResolver ?? throw new ArgumentNullException(nameof(chainComboResolver));
        }

        public bool TryBeginDrag()
        {
            if (Phase != TurnPhase.Idle) return false;
            Phase = TurnPhase.Dragging;
            return true;
        }

        public void CancelDrag()
        {
            if (Phase == TurnPhase.Dragging) Phase = TurnPhase.Idle;
        }

        public GameplayActionResult ResolveExpression(IReadOnlyList<BoardCoordinate> path)
        {
            if (Phase != TurnPhase.Dragging) throw new InvalidOperationException("An expression can only resolve while dragging.");
            if (path == null || path.Count == 0) throw new ArgumentException("A non-empty path is required.", nameof(path));

            Phase = TurnPhase.ResolvingAction;
            var removed = new List<BoardCoordinate>(path.Count);
            foreach (var coordinate in path)
            {
                board.RemoveTile(coordinate);
                removed.Add(coordinate);
            }

            Phase = TurnPhase.SettlingBoard;
            var gravity = gravityResolver.Resolve(board);
            var refill = refillResolver.Resolve(board, settings);

            Phase = TurnPhase.ResolvingLinking;
            var linking = linkingResolver.Resolve(board, settings);

            Phase = TurnPhase.ResolvingChainCombo;
            chainComboResolver.RegisterLinkingResult(chainComboState, linking);
            var comboSteps = new List<ChainComboStepResult>();
            // 현재는 실제 단계 효과가 없으므로 계속 진행한다. 향후 5스택에서는 IsFiveStack에서
            // 중단하고 보드 특수 Resolution 후 TryResolveNextStep 호출을 재개한다.
            while (chainComboResolver.TryResolveNextStep(chainComboState, out var step))
                comboSteps.Add(step);
            var chainCombo = new ChainComboSettlementResult(
                comboSteps, chainComboState.CurrentStack, chainComboState.PendingComboTriggers);

            Phase = TurnPhase.Idle;
            return new GameplayActionResult(removed, gravity, refill, linking, chainCombo);
        }
    }
}
