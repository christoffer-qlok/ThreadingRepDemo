using System.Collections.Concurrent;

namespace ThreadingRepDemo
{
    internal class Program
    {
        static ConcurrentDictionary<string, int> threadValues = new ConcurrentDictionary<string, int>();
        static Dictionary<string, ThreadInfo> currentThreads = new Dictionary<string, ThreadInfo>();
        static void Main(string[] args)
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("Thread menu:");
                Console.WriteLine("1. List all threads");
                Console.WriteLine("2. Create new thread");
                Console.WriteLine("3. Kill thread");
                Console.WriteLine("4. Exit");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        ListAllThreads();
                        break;
                    case "2":
                        SpawnThread();
                        break;
                    case "3":
                        KillThread();
                        break;
                    case "4":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Bad input! Press enter to go back to menu");
                        Console.ReadLine();
                        break;
                }
            }

            // Cancel all running threads so we can exit cleanly
            foreach (var threadInfo in currentThreads.Values)
            {
                threadInfo.Cts.Cancel();
            }
        }

        static void KillThread()
        {
            Console.WriteLine("Current threads active:");
            foreach (var thread in currentThreads.Keys) {
                Console.WriteLine(thread);
            }
            Console.Write("Enter thread name to kill:");
            string input = Console.ReadLine();

            if(currentThreads.ContainsKey(input))
            {
                ThreadInfo info = currentThreads[input];

                // Tell thread to stop running
                info.Cts.Cancel();

                // Wait for it to finalize
                info.CurrentThread.Join();

                Console.WriteLine($"{input} killed. Final value {threadValues[input]}");
                currentThreads.Remove(input);
            } else
            {
                Console.WriteLine("No such thread");
            }
            Console.Write("Press enter to go back to menu");
            Console.ReadLine();
        }

        static void ListAllThreads()
        {
            Console.WriteLine("Current threads:");
            foreach (var threadName in currentThreads.Keys)
            {
                int value = threadValues.GetValueOrDefault(threadName);
                Console.WriteLine($"{threadName} has value {value}");
            }
            Console.Write("Press enter to go back to menu");
            Console.ReadLine();
        }
        static void SpawnThread()
        {
            Console.Write("Enter thread name:");
            string input = Console.ReadLine();

            if(currentThreads.ContainsKey(input))
            {
                Console.WriteLine("Thread with that name already exists!");
            } else
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                CancellationToken ct = cts.Token;
                Thread newThread = new Thread(() => DoWork(input, ct));
                currentThreads[input] = new ThreadInfo()
                {
                    Cts = cts,
                    CurrentThread = newThread
                };
                newThread.Start();
                Console.WriteLine("Started new thread!");
            }

            Console.Write("Press enter to go back to menu");
            Console.ReadLine();
        }

        static void DoWork(string threadName, CancellationToken ct)
        {
            Random r = new Random();

            while(true)
            {
                if(ct.IsCancellationRequested)
                {
                    break;
                }
                threadValues.AddOrUpdate(threadName, 1, (key, oldValue) => oldValue + 1);
                Thread.Sleep(r.Next(1000));
            }
        }
    }
}