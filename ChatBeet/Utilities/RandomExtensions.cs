using System;
using System.Linq;

namespace ChatBeet.Utilities
{
    public static class RandomExtensions
    {
        /// <summary>
        /// Get a random integer approxomating normal distribution
        /// </summary>
        /// <remarks>Aims for bell curve with a series of coin tosses instead of one random generation</remarks>
        /// <param name="rng">Random number generator</param>
        /// <param name="minValue">Minimum value</param>
        /// <param name="maxValue">Maximum value</param>
        /// <returns>A random integer between the minimum and maximum</returns>
        public static int NormalNext(this Random rng, int minValue, int maxValue) => Enumerable
            .Repeat(1, maxValue - minValue)
            .Select(_ => rng.Next(0, 2))
            .Sum() + minValue;
    }
}
