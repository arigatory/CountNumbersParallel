using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CountNumbersParallel
{
    public class State
    {
        public int start;
        public int end;
        public int count;
        public int coreNum;

        public State(int start, int end, int core)
        {
            this.start = start;
            this.end = end;
            this.count = 0;
            this.coreNum = core;
        }
    }

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
            int cores = Environment.ProcessorCount;
            Console.WriteLine($"Main запущен. Процессоров: {cores}");
            Task[] tasks = new Task[cores];
            State[] states = new State[cores];
            int start = 1_000_000_000;
            int end = 2_000_000_000;
            int steps = (end - start + 1) / cores;
            int current = start;


            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();


            for (int i = 0; i < cores; i++)
            {
                current = start + steps;
                if (i == cores - 1)
                {
                    current = end;
                }
                states[i] = new State(start, current, i+1);
                tasks[i] = new Task(CountNumbersTask, states[i]);
                tasks[i].Start();
                start += steps;
            }

            Task.WaitAll(tasks);

            int total = 0;
            for (int i = 0; i < cores; i++)
            {
                Console.WriteLine($"Поток {states[i].coreNum} считал с {states[i].start} до {states[i].end}. Результат: {states[i].count}.");
                total += states[i].count;
            }


            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Console.WriteLine($"Результат: {total}. Заняло времени: {elapsedTime}");

            Console.ReadLine();
        }

        private static void CountNumbersTask(object o)
        {
            State s = (State)o;
            int total = 0;
            Console.WriteLine($"Выполняем подсчет в потоке {s.coreNum}. Числа с {s.start} до {s.end}.");
            for (int i = s.start; i < s.end; i++)
            {
                int lastDigit = i % 10;
                if (lastDigit != 0)
                {
                    if (sumOfDigits(i) % lastDigit == 0)
                    {
                        Interlocked.Increment(ref total);
                    }
                }
            }
            s.count = total;
            Console.WriteLine($"Поток {s.coreNum} завершил работу. Результат: {s.count}");
        }
    }
}
