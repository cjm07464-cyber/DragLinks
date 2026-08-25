using System;

namespace DragLinks.Board
{
    public interface ITileGenerator
    {
        TileState GenerateTile(BoardGenerationSettings settings);
    }

    public interface IRandomSource
    {
        double NextDouble();
    }

    public sealed class SeededRandomSource : IRandomSource
    {
        private readonly Random random;
        public SeededRandomSource(int seed) => random = new Random(seed);
        public double NextDouble() => random.NextDouble();
    }

    public sealed class BoardGenerator : ITileGenerator
    {
        private readonly IRandomSource random;

        public BoardGenerator(IRandomSource random)
        {
            this.random = random ?? throw new ArgumentNullException(nameof(random));
        }

        public BoardState Generate(BoardGenerationSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            var board = new BoardState(settings.Width, settings.Height);

            for (var y = 0; y < board.Height; y++)
            for (var x = 0; x < board.Width; x++)
                board.SetTile(x, y, GenerateTile(settings));

            return board;
        }

        public TileState GenerateTile(BoardGenerationSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            var category = PickWeighted(new[] { settings.NumberCategoryWeight, settings.OperatorCategoryWeight });
            if (category == 0)
            {
                var identity = PickWeighted(settings.NumberWeights) + 1;
                return TileState.CreateNumber(identity, identity);
            }

            return TileState.CreateOperator((OperatorType)PickWeighted(settings.OperatorWeights));
        }

        private int PickWeighted(float[] weights)
        {
            var total = 0d;
            foreach (var weight in weights) total += weight;

            var value = random.NextDouble() * total;
            for (var i = 0; i < weights.Length; i++)
            {
                value -= weights[i];
                if (value < 0d) return i;
            }

            return weights.Length - 1;
        }
    }
}
