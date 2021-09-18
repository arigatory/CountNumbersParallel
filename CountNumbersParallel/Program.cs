using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CountNumbersParallel
{
    class Program
    {
        static int sumOfDigits(int n)
        {
            int sum = 0;
            while (n != 0)
            {
                sum += n % 10;
                n /= 10;
            }
            return sum;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Main запущен.");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Task<int> task = Task<int>.Factory.StartNew(CountNumbers);
            var result = task.Result;
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Console.WriteLine($"Результат: {task.Result}. Заняло времени: {elapsedTime}");

            Console.ReadLine();
        }

        private static int CountNumbers()
        {
            Console.WriteLine("Выполняем подсчет...");
            int total = 0;
            for (int i = 1_000_000_000; i < 2_000_000_000; i++)
            {
                int lastDigit = i % 10;
                if (lastDigit != 0)
                {
                    if (sumOfDigits(i) % lastDigit == 0)
                    {
                        total += 1;
                    }
                }
            }

            return total;
        }
    }
}
