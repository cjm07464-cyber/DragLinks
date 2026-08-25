using DragLinks.Board;
using UnityEngine;

namespace DragLinks.Data
{
    [CreateAssetMenu(fileName = "BoardConfig", menuName = "Drag Links/Board Config")]
    public sealed class BoardConfig : ScriptableObject
    {
        [Header("Board (temporary test defaults)")]
        [SerializeField, Min(1)] private int width = 7;
        [SerializeField, Min(1)] private int height = 6;
        [SerializeField, Min(1)] private int baseMaxDragLength = 5;

        [Header("Category weights")]
        [SerializeField, Min(0f)] private float numberCategoryWeight = 3f;
        [SerializeField, Min(0f)] private float operatorCategoryWeight = 1f;

        [Header("Number weights (1 to 9)")]
        [SerializeField] private float[] numberWeights = { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };

        [Header("Operator weights (+, -, ×, ÷)")]
        [SerializeField] private float[] operatorWeights = { 1f, 1f, 1f, 1f };

        public BoardGenerationSettings CreateRuntimeSettings()
        {
            return new BoardGenerationSettings(width, height, baseMaxDragLength,
                numberCategoryWeight, operatorCategoryWeight, numberWeights, operatorWeights);
        }
    }
}
