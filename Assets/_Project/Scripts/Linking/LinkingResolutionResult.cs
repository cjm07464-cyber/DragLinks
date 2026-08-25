using System;
using System.Collections.Generic;
using DragLinks.Board;

namespace DragLinks.Linking
{
    public sealed class LinkingWaveResult
    {
        public int WaveNumber { get; }
        public LinkingResult Detection { get; }
        public BoardGravityResult Gravity { get; }
        public BoardRefillResult Refill { get; }

        public LinkingWaveResult(int waveNumber, LinkingResult detection,
            BoardGravityResult gravity, BoardRefillResult refill)
        {
            WaveNumber = waveNumber;
            Detection = detection ?? throw new ArgumentNullException(nameof(detection));
            Gravity = gravity ?? throw new ArgumentNullException(nameof(gravity));
            Refill = refill ?? throw new ArgumentNullException(nameof(refill));
        }
    }

    public sealed class LinkingResolutionResult
    {
        public IReadOnlyList<LinkingWaveResult> Waves { get; }
        /// <summary>이 Resolution에서 새로 획득한 Pending Combo Trigger 수다. 현재 스택이 아니다.</summary>
        public int TotalLinkingLineCount { get; }
        public bool SafetyLimitReached { get; }

        public LinkingResolutionResult(IReadOnlyList<LinkingWaveResult> waves,
            int totalLinkingLineCount, bool safetyLimitReached)
        {
            Waves = waves ?? throw new ArgumentNullException(nameof(waves));
            TotalLinkingLineCount = totalLinkingLineCount;
            SafetyLimitReached = safetyLimitReached;
        }
    }
}
