using System;

namespace DragLinks.Board
{
    /// <summary>ScriptableObject에서 복사되어 생성기가 사용하는 불변 런타임 설정이다.</summary>
    public sealed class BoardGenerationSettings
    {
        public int Width { get; }
        public int Height { get; }
        public int BaseMaxDragLength { get; }
        public float NumberCategoryWeight { get; }
        public float OperatorCategoryWeight { get; }
        public float[] NumberWeights { get; }
        public float[] OperatorWeights { get; }

        public BoardGenerationSettings(int width, int height, int baseMaxDragLength,
            float numberCategoryWeight, float operatorCategoryWeight,
            float[] numberWeights, float[] operatorWeights)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
            if (baseMaxDragLength <= 0) throw new ArgumentOutOfRangeException(nameof(baseMaxDragLength));
            if (numberWeights == null || numberWeights.Length != 9) throw new ArgumentException("Exactly 9 number weights are required.", nameof(numberWeights));
            if (operatorWeights == null || operatorWeights.Length != 4) throw new ArgumentException("Exactly 4 operator weights are required.", nameof(operatorWeights));

            Width = width;
            Height = height;
            BaseMaxDragLength = baseMaxDragLength;
            NumberCategoryWeight = ValidateWeight(numberCategoryWeight, nameof(numberCategoryWeight));
            OperatorCategoryWeight = ValidateWeight(operatorCategoryWeight, nameof(operatorCategoryWeight));
            NumberWeights = CopyAndValidate(numberWeights, nameof(numberWeights));
            OperatorWeights = CopyAndValidate(operatorWeights, nameof(operatorWeights));

            if (NumberCategoryWeight + OperatorCategoryWeight <= 0f)
                throw new ArgumentException("At least one category weight must be positive.");
            EnsurePositiveTotal(NumberWeights, nameof(numberWeights));
            EnsurePositiveTotal(OperatorWeights, nameof(operatorWeights));
        }

        private static float[] CopyAndValidate(float[] source, string parameterName)
        {
            var copy = new float[source.Length];
            for (var i = 0; i < source.Length; i++) copy[i] = ValidateWeight(source[i], parameterName);
            return copy;
        }

        private static float ValidateWeight(float weight, string parameterName)
        {
            if (float.IsNaN(weight) || float.IsInfinity(weight) || weight < 0f)
                throw new ArgumentOutOfRangeException(parameterName, "Weights must be finite and non-negative.");
            return weight;
        }

        private static void EnsurePositiveTotal(float[] weights, string parameterName)
        {
            var total = 0f;
            foreach (var weight in weights) total += weight;
            if (total <= 0f) throw new ArgumentException("At least one weight must be positive.", parameterName);
        }
    }
}
