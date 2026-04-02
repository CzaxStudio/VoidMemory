using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Linq;

namespace ProCheatEngineCLI
{
    public class MemoryManager
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        private static extern bool VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [StructLayout(LayoutKind.Sequential)]
        struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }
        Dictionary<IntPtr, int> frozenValues = new();
        bool freezeRunning = false;
        const int PROCESS_ALL_ACCESS = 0x1F0FFF;
        const uint MEM_COMMIT = 0x1000;

        private IntPtr hProcess;
        private List<IntPtr> foundAddresses = new();

        public void CheatProcess(ProcessInfo processInfo)
        {
            hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, processInfo.Id);
            if (hProcess == IntPtr.Zero)
            {
                Console.WriteLine("Failed to attach to process!");
                return;
            }

            Console.Clear();
            Console.WriteLine("=== VoidMemory ===");
            Console.WriteLine($"Attached to: {processInfo.ProcessName} (PID: {processInfo.Id})");
            Console.WriteLine();

            while (true)
            {
                ShowMenu();
                string? cmd = Console.ReadLine()?.Trim().ToLower();

                switch (cmd)
                {
                    case "1": FirstScan(); break;
                    case "2": NextScan(); break;
                    case "3": ListAddresses(); break;
                    case "4": PatchValue(); break;
                    case "5": foundAddresses.Clear(); Console.WriteLine("Results cleared"); break;
                    case "6": FreezeValue(); break;
                    case "7": UnfreezeAll(); break;
                    case "8": AutoHealthScan(); break;
                    case "q":
                    case "quit": goto end;
                    default: Console.WriteLine("Invalid option! (1-5 or q)"); break;
                }
                Console.WriteLine();
            }
        end:
            CloseHandle(hProcess);
        }

        void ShowMenu()
        {
            Console.WriteLine("=== Pro Cheat Engine CLI ===");
            Console.WriteLine();

            Console.WriteLine("1. First Scan        (search for exact value)");
            Console.WriteLine("2. Next Scan         (narrow results)");
            Console.WriteLine("3. List Results      (show addresses)");
            Console.WriteLine("4. Patch Value       (change memory)");
            Console.WriteLine("5. Clear Results");

            Console.WriteLine();
            Console.WriteLine("=== Advanced ===");

            Console.WriteLine("6. Freeze Value      (lock value)");
            Console.WriteLine("7. Unfreeze All");

            Console.WriteLine();
            Console.WriteLine("=== Auto Features ===");

            Console.WriteLine("8. Auto Scan (Health)");

            Console.WriteLine();
            Console.WriteLine($"q. Quit             (Found: {foundAddresses.Count})");
            Console.Write("Choose: ");
        }
        void AutoHealthScan()
        {
            Console.WriteLine("=== AUTO HEALTH SCAN ===");
            Console.WriteLine("Step 1: Make sure your health is full.");
            Console.WriteLine("Press ENTER to start initial scan...");
            Console.ReadLine();
            foundAddresses.Clear();
            Dictionary<IntPtr, int> currentValues = new();

            Console.WriteLine("[*] Scanning initial values...");

            ScanAllMemoryUnknown(currentValues);

            Console.WriteLine($"[+] Initial scan: {currentValues.Count} candidates");

            for (int round = 1; round <= 3; round++)
            {
                Console.WriteLine($"\n[ROUND {round}] Take damage now!");
                Console.WriteLine("Press ENTER after health decreases...");
                Console.ReadLine();

                var newValues = new Dictionary<IntPtr, int>();
                var filtered = new List<IntPtr>();

                foreach (var kvp in currentValues)
                {
                    int newVal = ReadInt32(kvp.Key);
                    if (newVal < kvp.Value) 
                    {
                        newValues[kvp.Key] = newVal;
                        filtered.Add(kvp.Key);
                    }
                }

                currentValues = newValues;

                Console.WriteLine($"[+] After filter: {currentValues.Count} results");

                if (currentValues.Count < 5)
                    break;
            }

            foundAddresses = currentValues.Keys.ToList();

            Console.WriteLine("\n[+] Auto scan complete!");
            Console.WriteLine($"Possible addresses: {foundAddresses.Count}");

            ListAddresses();
        }
        void ScanAllMemoryUnknown(Dictionary<IntPtr, int> results)
        {
            IntPtr addr = IntPtr.Zero;
            const int chunkSize = 4096;

            while (addr.ToInt64() < 0x7FFFFFFF)
            {
                if (!VirtualQueryEx(hProcess, addr, out MEMORY_BASIC_INFORMATION mbi,
                    (uint)Marshal.SizeOf<MEMORY_BASIC_INFORMATION>()))
                    break;

                if (mbi.State == 0x1000 && mbi.Protect == 0x04)
                {
                    long regionSize = mbi.RegionSize.ToInt64();
                    byte[] buffer = new byte[chunkSize];

                    for (long offset = 0; offset < regionSize; offset += chunkSize)
                    {
                        int bytesToRead = (int)Math.Min(chunkSize, regionSize - offset);
                        IntPtr currentAddr = new IntPtr(mbi.BaseAddress.ToInt64() + offset);

                        if (ReadProcessMemory(hProcess, currentAddr, buffer, bytesToRead, out int bytesRead))
                        {
                            for (int i = 0; i < bytesRead - 4; i++)
                            {
                                int val = BitConverter.ToInt32(buffer, i);
                                results[new IntPtr(currentAddr.ToInt64() + i)] = val;

                                if (results.Count > 200000)
                                    return;
                            }
                        }
                    }
                }

                addr = new IntPtr(mbi.BaseAddress.ToInt64() + (long)mbi.RegionSize);
            }
        }
        void FirstScan()
        {
            Console.Write("Value (dec/hex): ");
            string? valStr = Console.ReadLine();

            Console.Write("Type (4byte/2byte/byte/float): ");
            string type = (Console.ReadLine()?.ToLower()) ?? "4byte";

            bool isFloat = type == "float";

            float floatValue = 0;
            int intValue = 0;

            if (isFloat)
            {
                if (!float.TryParse(valStr, out floatValue))
                {
                    Console.WriteLine("Invalid float!");
                    return;
                }
            }
            else
            {
                if (!ParseValue(valStr, out intValue))
                    return;
            }

            foundAddresses.Clear();
            ScanAllMemory(intValue);

            Console.WriteLine($"[+] First scan complete: {foundAddresses.Count} results");
        }

        void NextScan()
        {
            if (foundAddresses.Count == 0)
            {
                Console.WriteLine("[-] No previous results! Do first scan first.");
                return;
            }

            Console.Write("New value: ");
            string? valStr = Console.ReadLine();

            Console.Write("Type: ");
            string type = (Console.ReadLine()?.ToLower()) ?? "4byte";

            bool isFloat = type == "float";

            float floatValue = 0;
            int intValue = 0;

            if (isFloat)
            {
                if (!float.TryParse(valStr, out floatValue))
                {
                    Console.WriteLine("Invalid float!");
                    return;
                }
            }
            else
            {
                if (!ParseValue(valStr, out intValue))
                    return;
            }

            var newList = new List<IntPtr>();

            foreach (var addr in foundAddresses)
            {
                byte[] buffer = new byte[4];

                if (!ReadProcessMemory(hProcess, addr, buffer, 4, out _))
                    continue;

                bool match = false;

                switch (type)
                {
                    case "byte":
                        match = buffer[0] == intValue;
                        break;

                    case "2byte":
                        match = BitConverter.ToInt16(buffer, 0) == intValue;
                        break;

                    case "4byte":
                        match = BitConverter.ToInt32(buffer, 0) == intValue;
                        break;

                    case "float":
                        float f = BitConverter.ToSingle(buffer, 0);
                        match = Math.Abs(f - floatValue) < 0.01f;
                        break;
                }

                if (match)
                    newList.Add(addr);
            }

            foundAddresses = newList;
            Console.WriteLine($"[+] Next scan complete: {foundAddresses.Count} results");
        }

        void ListAddresses()
        {
            if (foundAddresses.Count == 0)
            {
                Console.WriteLine("[-] No results");
                return;
            }

            Console.WriteLine($"\nAddresses ({foundAddresses.Count}):");
            int maxShow = Math.Min(20, foundAddresses.Count);
            for (int i = 0; i < maxShow; i++)
            {
                int val = ReadInt32(foundAddresses[i]);
                Console.WriteLine($"{i + 1}. 0x{foundAddresses[i]:X8} = {val}");
            }
            if (foundAddresses.Count > 20)
                Console.WriteLine($"... and {foundAddresses.Count - 20} more");
        }
        void FreezeLoop()
        {
            while (freezeRunning)
            {
                foreach (var kvp in frozenValues)
                {
                    byte[] buffer = BitConverter.GetBytes(kvp.Value);
                    WriteProcessMemory(hProcess, kvp.Key, buffer, 4, out _);
                }

                Thread.Sleep(10); 
            }
        }

        void PatchValue()
        {
            if (foundAddresses.Count == 0)
            {
                Console.WriteLine("[-] No results to patch!");
                return;
            }

            Console.Write($"Address # (1-{foundAddresses.Count}): ");
            string? indexStr = Console.ReadLine();
            if (!int.TryParse(indexStr, out int index) || index < 1 || index > foundAddresses.Count)
            {
                Console.WriteLine("Invalid index!");
                return;
            }

            IntPtr addr = foundAddresses[index - 1];
            Console.Write("New value (dec/hex): ");
            string? newValStr = Console.ReadLine();
            if (!ParseValue(newValStr, out int newVal))
                return;

            if (WriteInt32(addr, newVal))
                Console.WriteLine($"[+] Patched 0x{addr:X8} = {newVal} ✓");
            else
                Console.WriteLine("[-] Patch failed!");
        }
        void FreezeValue()
        {
            if (foundAddresses.Count == 0)
            {
                Console.WriteLine("[-] No results to freeze!");
                return;
            }

            Console.Write($"Address # (1-{foundAddresses.Count}): ");
            if (!int.TryParse(Console.ReadLine(), out int index) ||
                index < 1 || index > foundAddresses.Count)
            {
                Console.WriteLine("Invalid index!");
                return;
            }

            IntPtr addr = foundAddresses[index - 1];

            Console.Write("Value to freeze: ");
            if (!int.TryParse(Console.ReadLine(), out int value))
            {
                Console.WriteLine("Invalid value!");
                return;
            }

            frozenValues[addr] = value;

            Console.WriteLine($"[+] Freezing 0x{addr:X} = {value}");

            if (!freezeRunning)
            {
                freezeRunning = true;
                new Thread(FreezeLoop) { IsBackground = true }.Start();
            }
        }
        void UnfreezeAll()
        {
            frozenValues.Clear();
            freezeRunning = false;
            Console.WriteLine("[+] All addresses unfrozen");
        }

        static bool ParseValue(string? input, out int value)
        {
            value = 0;
            if (string.IsNullOrEmpty(input)) return false;

            if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                return int.TryParse(input.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out value);

            return int.TryParse(input, out value);
        }

        int ReadValue(IntPtr addr, int size)
        {
            byte[] buffer = new byte[4];
            ReadProcessMemory(hProcess, addr, buffer, size, out _);
            return size switch
            {
                1 => buffer[0],
                2 => BitConverter.ToInt16(buffer, 0),
                4 => BitConverter.ToInt32(buffer, 0),
                _ => BitConverter.ToInt32(buffer, 0)
            };
        }

        int ReadInt32(IntPtr addr)
        {
            byte[] buffer = new byte[4];
            ReadProcessMemory(hProcess, addr, buffer, 4, out _);
            return BitConverter.ToInt32(buffer, 0);
        }

        bool WriteInt32(IntPtr addr, int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            return WriteProcessMemory(hProcess, addr, buffer, 4, out _);
        }

        void ScanAllMemory(int targetValue)
        {
            foundAddresses.Clear();

            IntPtr addr = IntPtr.Zero;

            const int chunkSize = 4096;

            while (addr.ToInt64() < 0x7FFFFFFF0000)
            {
                if (!VirtualQueryEx(hProcess, addr, out MEMORY_BASIC_INFORMATION mbi,
                    (uint)Marshal.SizeOf<MEMORY_BASIC_INFORMATION>()))
                    break;
                if (mbi.State == 0x1000 && mbi.Protect == 0x04)
                {
                    long regionSize = mbi.RegionSize.ToInt64();

                    byte[] buffer = new byte[chunkSize];

                    for (long offset = 0; offset < regionSize; offset += chunkSize)
                    {
                        int bytesToRead = (int)Math.Min(chunkSize, regionSize - offset);

                        IntPtr currentAddr = new IntPtr(mbi.BaseAddress.ToInt64() + offset);

                        if (ReadProcessMemory(hProcess, currentAddr, buffer, bytesToRead, out int bytesRead))
                        {
                            for (int i = 0; i < bytesRead - 4; i++)
                            {
                                int val = BitConverter.ToInt32(buffer, i);

                                if (val == targetValue)
                                {
                                    foundAddresses.Add(new IntPtr(currentAddr.ToInt64() + i));

                                    if (foundAddresses.Count > 10000)
                                        return;
                                }
                            }
                        }
                    }
                }

                addr = new IntPtr(mbi.BaseAddress.ToInt64() + (long)mbi.RegionSize);
            }

            Console.WriteLine($"[+] Scan done: {foundAddresses.Count} results");
        }

        void ScanRegion(IntPtr baseAddr, int regionSize, int intValue, float floatValue, string type)
        {
            byte[] buffer = new byte[regionSize];
            int bytesRead;

            if (!ReadProcessMemory(hProcess, baseAddr, buffer, regionSize, out bytesRead))
                return;

            int dataSize = type switch
            {
                "byte" => 1,
                "2byte" => 2,
                "float" => 4,
                _ => 4
            };

            for (int i = 0; i < bytesRead - dataSize; i++) 
            {
                bool match = false;

                switch (type)
                {
                    case "byte":
                        match = buffer[i] == intValue;
                        break;

                    case "2byte":
                        short s = BitConverter.ToInt16(buffer, i);
                        match = s == intValue;
                        break;

                    case "4byte":
                        int val = BitConverter.ToInt32(buffer, i);
                        match = val == intValue;
                        break;

                    case "float":
                        float f = BitConverter.ToSingle(buffer, i);
                        match = Math.Abs(f - floatValue) < 0.01f; 
                        break;
                }

                if (match)
                {
                    foundAddresses.Add(new IntPtr(baseAddr.ToInt64() + i));

                    if (foundAddresses.Count > 50000)
                        return;
                }
            }
        }
    }
}