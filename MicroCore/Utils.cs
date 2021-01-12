using System;

namespace MicroCore
{
    public static class Utils
    {
        public static string IncreaseCpu(long k)
        {
            double root = 0.00002;
            for (int i = 0; i < k * 1000; i++)
            {
                root = root + Math.Sqrt(root);
            }
            return root.ToString();
        }
    }
}