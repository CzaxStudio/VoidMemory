using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ProCheatEngineCLI
{
    public class ProcessManager
    {
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int count);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        public List<ProcessInfo> GetRunningProcesses()
        {
            var visibleProcesses = new Dictionary<int, ProcessInfo>();

            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                if (process.MainWindowHandle == IntPtr.Zero) continue;

                try
                {
                    System.Text.StringBuilder windowTitle = new System.Text.StringBuilder(256);
                    GetWindowText(process.MainWindowHandle, windowTitle, windowTitle.Capacity);

                    if (!string.IsNullOrEmpty(windowTitle.ToString()) &&
                        windowTitle.ToString().Trim().Length > 0)
                    {
                        visibleProcesses[process.Id] = new ProcessInfo(process);
                    }
                }
                catch { }
            }

            return visibleProcesses.Values
                .Where(pi => pi.MainModuleName != null)
                .OrderBy(pi => pi.ProcessName)
                .ToList();
        }

        public void DisplayProcesses(List<ProcessInfo> processes)
        {
            Console.WriteLine("Visible Applications (with windows):");
            Console.WriteLine("====================================");
            for (int i = 0; i < processes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {processes[i].ProcessName}");
            }
            if (processes.Count == 0)
                Console.WriteLine("No visible apps found!");
            Console.WriteLine();
        }

        public ProcessInfo FindProcess(List<ProcessInfo> processes, string gameName)
        {
            var matches = processes.Where(p =>
                p.ProcessName.ToLower().Contains(gameName.ToLower())).ToList();

            if (matches.Count == 0)
            {
                // Fallback: search by partial name even if not visible
                Console.WriteLine("No visible match. Searching all processes...");
                var allProcesses = GetRunningProcesses();
                matches = allProcesses.Where(p =>
                    p.ProcessName.ToLower().Contains(gameName.ToLower())).ToList();
            }

            if (matches.Count == 0)
                return null;

            if (matches.Count == 1)
                return matches[0];

            Console.WriteLine("Multiple matches:");
            for (int i = 0; i < matches.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {matches[i].ProcessName} (PID: {matches[i].Id})");
            }

            Console.Write("Select (1-{0}): ", matches.Count);
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= matches.Count)
                return matches[choice - 1];

            return null;
        }
    }
}