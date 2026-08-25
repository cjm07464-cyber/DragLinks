using System;
using System.Collections.Generic;
using DragLinks.Board;

namespace DragLinks.Linking
{
    public sealed class LinkingResolver
    {
        // 기술적 무한 반복 보호값이며 게임 밸런스나 최대 연쇄 규칙이 아니다.
        public const int TechnicalSafetyWaveLimit = 1024;

        private readonly LinkingDetector detector;
        private readonly BoardGravityResolver gravityResolver;
        private readonly BoardRefillResolver refillResolver;
        private readonly int safetyWaveLimit;

        public LinkingResolver(LinkingDetector detector, BoardGravityResolver gravityResolver,
            BoardRefillResolver refillResolver, int safetyWaveLimit = TechnicalSafetyWaveLimit)
        {
            this.detector = detector ?? throw new ArgumentNullException(nameof(detector));
            this.gravityResolver = gravityResolver ?? throw new ArgumentNullException(nameof(gravityResolver));
            this.refillResolver = refillResolver ?? throw new ArgumentNullException(nameof(refillResolver));
            this.safetyWaveLimit = safetyWaveLimit > 0
                ? safetyWaveLimit
                : throw new ArgumentOutOfRangeException(nameof(safetyWaveLimit));
        }

        public LinkingResolutionResult Resolve(BoardState board, BoardGenerationSettings settings)
        {
            if (board == null) throw new ArgumentNullException(nameof(board));
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            var waves = new List<LinkingWaveResult>();
            var totalLineCount = 0;
            for (var waveNumber = 1; waveNumber <= safetyWaveLimit; waveNumber++)
            {
                var detection = detector.Detect(board);
                if (!detection.HasLinking)
                    return new LinkingResolutionResult(waves, totalLineCount, false);

                foreach (var coordinate in detection.UniqueCoordinates) board.RemoveTile(coordinate);
                var gravity = gravityResolver.Resolve(board);
                var refill = refillResolver.Resolve(board, settings);
                waves.Add(new LinkingWaveResult(waveNumber, detection, gravity, refill));
                totalLineCount += detection.LineCount;
            }

            var stillHasLinking = detector.Detect(board).HasLinking;
            return new LinkingResolutionResult(waves, totalLineCount, stillHasLinking);
        }
    }
}
