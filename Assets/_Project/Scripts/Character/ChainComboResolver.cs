using System;
using DragLinks.Linking;

namespace DragLinks.Character
{
    public sealed class ChainComboStepResult
    {
        public int ActivatedStack { get; }
        public bool IsFiveStack => ActivatedStack == ChainComboRuntimeState.FiveStackThreshold;
        public int CurrentStackAfterStep { get; }
        public int RemainingPendingTriggers { get; }

        public ChainComboStepResult(int activatedStack, int currentStackAfterStep, int remainingPendingTriggers)
        {
            ActivatedStack = activatedStack;
            CurrentStackAfterStep = currentStackAfterStep;
            RemainingPendingTriggers = remainingPendingTriggers;
        }
    }

    public sealed class ChainComboResolver
    {
        public void RegisterLinkingResult(ChainComboRuntimeState state, LinkingResolutionResult linking)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            if (linking == null) throw new ArgumentNullException(nameof(linking));
            state.AddPendingTriggers(linking.TotalLinkingLineCount);
        }

        /// <summary>Pending을 정확히 하나만 소비한다. 5스택 외부 처리 전후 재호출할 수 있다.</summary>
        public bool TryResolveNextStep(ChainComboRuntimeState state, out ChainComboStepResult result)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            if (!state.TryConsumeNext(out var activatedStack))
            {
                result = null;
                return false;
            }

            result = new ChainComboStepResult(
                activatedStack,
                state.CurrentStack,
                state.PendingComboTriggers);
            return true;
        }
    }
}
