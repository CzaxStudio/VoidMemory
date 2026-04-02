#!/usr/bin/env bash

# =========================================

# VoidMemory Build Script

# =========================================

set -e

echo "========================================="
echo " Building VoidMemory (Release)"
echo "========================================="

# Clean previous builds

echo "[*] Cleaning project..."
dotnet clean

# Restore dependencies

echo "[*] Restoring dependencies..."
dotnet restore

# Build project

echo "[*] Building..."
dotnet build -c Release

# Publish self-contained EXE

echo "[*] Publishing (self-contained, single file)..."
dotnet publish -c Release -r win-x64 
--self-contained true 
/p:PublishSingleFile=true 
/p:PublishTrimmed=true

echo "========================================="
echo " Build Complete!"
echo "========================================="

echo "[+] Output located at:"
echo "bin/Release/net8.0/win-x64/publish/"
