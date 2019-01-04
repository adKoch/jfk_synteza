using System.Runtime.CompilerServices;

namespace Playground
{
    using System;
    using static System.Console;

    public static class Class
    {
            public static void Main()
            {
                int dec = 0;
                foreach (int i in YieldTest())
                {
                    Console.WriteLine(i);
                }
                foreach (double i in YieldGenTest())
                {
                    Console.WriteLine(i);
                }
                Console.ReadKey();
            }

            public static IEnumerable<int> YieldTest()
            {
                int[] a = { 2, 5, 2, 5, 32, 4 };
                foreach (var item in a)
{
    yield return item;
}
            }

            public static IEnumerable<double> YieldGenTest()
            {
                double[] a = { 2.0, 3.6, 4.96, 3.0, 7.2, 6.6 };
                foreach (double item in a)
{
    yield return item;
}
            }
        
    }
}

