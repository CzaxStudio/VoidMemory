using System;
using System.Diagnostics;

namespace ProCheatEngineCLI
{
    public class ProcessInfo
    {
        public Process Process { get; private set; }
        public string ProcessName { get; private set; }
        public int Id { get; private set; }
        public string MainModuleName { get; private set; }
        public IntPtr MainModuleBase { get; private set; }

        public ProcessInfo(Process process)
        {
            Process = process;
            ProcessName = process.ProcessName;
            Id = process.Id;

            try
            {
                MainModuleName = process.MainModule?.ModuleName;
                MainModuleBase = process.MainModule?.BaseAddress ?? IntPtr.Zero;
            }
            catch
            {
                MainModuleName = null;
                MainModuleBase = IntPtr.Zero;
            }
        }
    }
}