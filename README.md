# VoidMemory

**VoidMemory** is a lightweight, fast, and beginner-friendly memory scanner and game hacking tool built in C#.
It provides an intuitive CLI experience for scanning, modifying, and freezing in-game values.

---

<img width="1536" height="1024" alt="VoidMemory" src="https://github.com/user-attachments/assets/7617ba76-570b-44e1-afcb-5b9e895adc54">
                                               **Void Memory**                      


## Features

### Memory Scanning

* Scan for exact values (int, float, byte, 2-byte)
* Fast chunk-based memory reading
* Supports large memory regions efficiently

### Next Scan Filtering

* Narrow results using:

  * Exact value
  * Increased value
  * Decreased value
  * Changed / Unchanged

### Freeze System

* Lock values in memory
* Maintain constant values (e.g., infinite health, ammo)

### Auto Health Scan (Beginner Mode)

* Guided scanning process
* Automatically filters values based on player actions
* Designed for ease-of-use compared to traditional tools

---

## Usage

### 1. Run the Tool

> Run as Administrator

```bash
VoidMemory.exe
```

---

### 2. Attach to a Process

Select a running process (e.g., a game).

---

### 3. Choose an Option

#### Basic Scan

1. Enter value (e.g., `100`)
2. Select type (4byte, float, etc.)
3. Use **Next Scan** to narrow results

---

#### Auto Scan (Recommended)

1. Select **Auto Scan (Health)**
2. Follow prompts:

   * Start at full health
   * Take damage when instructed
3. Tool automatically filters correct memory addresses

---

#### Freeze Value

1. Select an address from results
2. Enter value to freeze
3. Value will remain constant in-game

---

## Important Notes

* Must be run as **Administrator**
* Antivirus software may flag this tool due to memory access behavior

---

## Built With(Only if you download the C# source files to edit)

* **C#**
* **.NET 8**
* Windows API:

  * `OpenProcess`
  * `ReadProcessMemory`
  * `WriteProcessMemory`
  * `VirtualQueryEx`

---

## Build Instructions(Only if you download the C# source files)

###  Development Build

```bash
dotnet build -c Release
```

### Release (Portable EXE)

```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true
```

---

##  Example Workflow

1. Scan health (e.g., `100`)
2. Take damage → scan `90`
3. Narrow results
4. Freeze value to `999`
5. Enjoy infinite health 

---

##  Why VoidMemory?

Unlike traditional tools, VoidMemory focuses on:

*  Speed
*  Simplicity
*  Beginner-friendly automation
*  Clean CLI experience

---

##  Disclaimer

This project is intended for **educational purposes only**.
Do not use this tool in online or multiplayer games.

---

##  Author

**CzaxStudio**

---

## Support

If you like this project:

*  Star the repository
*  Fork it
*  Contribute improvements

---

##  Future Plans

* Auto scan for ammo, money, etc.
* Pointer scanning
* GUI version (WinForms / WPF)
* Advanced memory analysis

---

**VoidMemory — Control Memory. Control Reality.**
