using System;
using System.Buffers;

namespace ProCheatEngineCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== VoidMemory CLI ===");
            Console.WriteLine("Coded by Tanmay");
            Console.WriteLine("Programming Language --> C#");
            Console.WriteLine("Follow me on GitHub --> https://github.com/CzaxStudio");
            Console.WriteLine();

            ProcessManager processManager = new ProcessManager();
            MemoryManager memoryManager = new MemoryManager();

            while (true)
            {
                var processes = processManager.GetRunningProcesses();
                processManager.DisplayProcesses(processes);

                Console.Write("Enter game name (or 'quit' to exit): ");
                string gameName = Console.ReadLine().Trim();

                if (gameName.ToLower() == "quit")
                    break;

                var targetProcess = processManager.FindProcess(processes, gameName);
                if (targetProcess == null)
                {
                    Console.WriteLine("Process not found!");
                    continue;
                }

                Console.WriteLine($"Found: {targetProcess.ProcessName} (PID: {targetProcess.Id})");
                memoryManager.CheatProcess(targetProcess);
                Console.WriteLine();
            }
        }
    }
}