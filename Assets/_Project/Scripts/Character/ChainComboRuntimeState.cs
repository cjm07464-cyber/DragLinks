using System;

namespace DragLinks.Character
{
    /// <summary>턴을 넘어 유지되는 한가은 연쇄 콤보 Runtime State다.</summary>
    public sealed class ChainComboRuntimeState
    {
        public const int FiveStackThreshold = 5;

        public int CurrentStack { get; private set; }
        public int PendingComboTriggers { get; private set; }

        public ChainComboRuntimeState(int currentStack = 0, int pendingComboTriggers = 0)
        {
            if (currentStack < 0 || currentStack >= FiveStackThreshold)
                throw new ArgumentOutOfRangeException(nameof(currentStack), "CurrentStack must remain between 0 and 4.");
            if (pendingComboTriggers < 0)
                throw new ArgumentOutOfRangeException(nameof(pendingComboTriggers));

            CurrentStack = currentStack;
            PendingComboTriggers = pendingComboTriggers;
        }

        public void AddPendingTriggers(int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            checked { PendingComboTriggers += count; }
        }

        internal bool TryConsumeNext(out int activatedStack)
        {
            if (PendingComboTriggers == 0)
            {
                activatedStack = 0;
                return false;
            }

            PendingComboTriggers--;
            activatedStack = CurrentStack + 1;
            CurrentStack = activatedStack == FiveStackThreshold ? 0 : activatedStack;
            return true;
        }
    }
}
