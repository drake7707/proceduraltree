using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeGrowingAlgorithm
{
    public static class Extensions
    {
        /// <summary>
        /// Returns a random number between min and max as doubles
        /// </summary>
        /// <param name="rnd"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double Next(this Random rnd, double min, double max)
        {
            return min + rnd.NextDouble() * (max - min);
        }

    }
}
