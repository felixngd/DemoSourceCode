using System;

namespace VoidexSoft.Inventory.DataCore
{
    public static class RandomId
    {
        private static Random s_Random;

        public static int Empty => 0;

        private static Random Random => s_Random ??= new Random();

        public static int Generate() => (Random.Next(1073741824) << 2) |  Random.Next(4);

        public static bool IsIDEmpty(int id) => id == Empty;
    }
}