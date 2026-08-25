using System;
using System.Collections.Generic;

namespace DragLinks.Character
{
    public sealed class ChainComboSettlementResult
    {
        public IReadOnlyList<ChainComboStepResult> Steps { get; }
        public int FinalStack { get; }
        public int RemainingPendingTriggers { get; }

        public ChainComboSettlementResult(IReadOnlyList<ChainComboStepResult> steps,
            int finalStack, int remainingPendingTriggers)
        {
            Steps = steps ?? throw new ArgumentNullException(nameof(steps));
            FinalStack = finalStack;
            RemainingPendingTriggers = remainingPendingTriggers;
        }
    }
}
