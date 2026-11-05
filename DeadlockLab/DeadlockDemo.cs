using System;
using System.Threading;

namespace DeadlockLab
{
    public class DeadlockDemo
    {
        public static void Run()
        {
            Console.WriteLine("=== DEADLOCK DEMO (TODO 4: 3 потока, циклическая зависимость) ===");

            object lockA = new();
            object lockB = new();
            object lockC = new();

            using Barrier barrier = new(3);

            Thread threadA = new(() =>
            {
                Console.WriteLine("Поток A: ожидает ресурс A");
                lock (lockA)
                {
                    Console.WriteLine("Поток A: захватил ресурс A");
                    barrier.SignalAndWait();

                    Console.WriteLine("Поток A: ожидает ресурс B");
                    lock (lockB)
                    {
                        Console.WriteLine("Поток A: получил ресурсы A и B");
                    }
                }
            });

            Thread threadB = new(() =>
            {
                Console.WriteLine("Поток B: ожидает ресурс B");
                lock (lockB)
                {
                    Console.WriteLine("Поток B: захватил ресурс B");
                    barrier.SignalAndWait(); 

                    Console.WriteLine("Поток B: ожидает ресурс C");
                    lock (lockC)
                    {
                        Console.WriteLine("Поток B: получил ресурсы B и C");
                    }
                }
            });

            Thread threadC = new(() =>
            {
                Console.WriteLine("Поток C: ожидает ресурс C");
                lock (lockC)
                {
                    Console.WriteLine("Поток C: захватил ресурс C");
                    barrier.SignalAndWait();

                    Console.WriteLine("Поток C: ожидает ресурс A");
                    lock (lockA)
                    {
                        Console.WriteLine("Поток C: получил ресурсы C и A");
                    }
                }
            });

            threadA.IsBackground = true;
            threadB.IsBackground = true;
            threadC.IsBackground = true;

            threadA.Start();
            threadB.Start();
            threadC.Start();

            bool completedA = threadA.Join(5000);
            bool completedB = threadB.Join(5000);
            bool completedC = threadC.Join(5000);

            Console.WriteLine();

            if (!completedA || !completedB || !completedC)
            {
                Console.WriteLine("DEADLOCK ОБНАРУЖЕН!");
                Console.WriteLine("Один или несколько потоков не завершились за 5 секунд.");
            }
            else
            {
                Console.WriteLine("Все потоки успешно завершились.");
            }
        }
    }
}