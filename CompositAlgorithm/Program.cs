using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CompositAlgorithm
{
    class Program
    {
        private class Item : IComparable<Item>
        {
            public double Target { get; set; }
            public double Limit { get; set; }
            public int[] targetPath { get; set; }
            public int[] limitPath { get; set; }
            public int CompareTo([AllowNull] Item other)
            {
                if (this == other)
                    return 1;
                else if (this > other)
                    return 1;
                else
                    return -1;
            }

            public Item RecountLimit(double max)
            {
                this.Limit = max - Limit;
                return this;
            }

            public void SetLimitPath(int[] path)
            {
                limitPath = new int[path.Length];
                for (int i = 0; i < path.Length; i++)
                    limitPath[i] = path[i];
            }

            public void SetTargetPath(int[] path)
            {
                targetPath = new int[path.Length];
                for (int i = 0; i < path.Length; i++)
                    targetPath[i] = path[i];
            }

            public static bool operator ==(Item item1, Item item2) => 
                item1.Limit == item2.Limit && item1.Target == item2.Target;
            public static bool operator !=(Item item1, Item item2) =>
                item1.Limit != item2.Limit || item1.Target != item2.Target;
            public static bool operator <(Item item, Item item2) => 
                item.Target < item2.Target && item.Limit > item2.Limit;
            public static bool operator >(Item item, Item item2) => 
                item.Target > item2.Target && item.Limit < item2.Limit;
        }
        static void Main(string[] args)
        {
            Console.Write("Enter count of variables: ");

            int.TryParse(Console.ReadLine(), out int count);
            int realCount = count % 2 != 0 ? count + 1 : count;
            double[] coefficients = new double[realCount];
            double[] limit = new double[realCount];
            int[,] permutation = new int[,] {
                { 1, 1 },
                { 1, 0 },
                { 0, 1 },
                { 0, 0 }
            };
            int[] coef_path = new int[realCount];
            int[] limit_path = new int[realCount];


            Console.WriteLine("Enter the target function coefficient");
            for (int i = 0; i < count; i++)
            {
                Console.Write($"x{i} = ");
                double.TryParse(Console.ReadLine(), out coefficients[i]);

            }

            Console.WriteLine("Enter the limitation");
            for (int i = 0; i < count; i++)
            {
                Console.Write($"x{i} = ");
                double.TryParse(Console.ReadLine(), out limit[i]);
            }

            Console.Write("Limit = ");
            double.TryParse(Console.ReadLine(), out double bound);

            coef_path = Enumerable.Repeat(1, coef_path.Length).ToArray();
            limit_path = Enumerable.Repeat(0, limit_path.Length).ToArray();

            List<Item> answer = new List<Item>();

            for (int i = 1; i < realCount; i += 2)
            {
                List<Item> current_tier = new List<Item>();
                for (int j = 0; j < permutation.GetLength(0); j++)
                {
                    coef_path[i - 1] = permutation[j, 0];
                    coef_path[i] = permutation[j, 1];

                    limit_path[i - 1] = permutation[j, 0];
                    limit_path[i] = permutation[j, 1];

                    current_tier.Add(CalculateCurrentValue(coefficients, limit, coef_path, limit_path).RecountLimit(bound));
                }
                for (int q = 0; q < current_tier.Count; q++)
                {
                    if(current_tier[q].Limit < 0)
                    {
                        current_tier.RemoveAt(q);
                    }
                }
                Item max = current_tier.Max();
                limit_path = max.limitPath;
                coef_path = max.targetPath;
                answer.Add(max);
            }

            answer.ForEach(x =>
            {
                Console.WriteLine($"target: {x.Target}\tlimit: {x.Limit}\npath: ");
                for (int i = 0; i < x.targetPath.Length; i++)
                {
                    Console.WriteLine($"{coefficients[i]} ");
                }
                Console.WriteLine();
            });

            Console.ReadKey();
        }

        private static Item CalculateCurrentValue(double[] target, double[] limit, int[] target_path, int[] limit_path)
        {
            if (target.Length != limit_path.Length)
                throw new Exception("Error");
            Item result = new Item() { Limit = 0, Target = 0/*, targetPath = target_path, limitPath = limit_path*/ };
            result.SetLimitPath(limit_path);
            result.SetTargetPath(target_path);
            for (int i = 0; i < target.Length; i++)
            {
                result.Target += target[i] * target_path[i];
                result.Limit += limit[i] * limit_path[i];
            }
            return result;
        }

    }
}
