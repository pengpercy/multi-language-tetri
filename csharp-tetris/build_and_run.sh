#!/bin/bash

# TetrisSharp Build and Run Script
# This script builds the project and resolves SDL2 library conflicts

set -e

echo "🎮 TetrisSharp - Build and Run Script"
echo "====================================="

# Get the directory where this script is located
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BUILD_DIR="$SCRIPT_DIR/bin/Debug/net9.0"

# Function to display usage
usage() {
    echo "Usage: $0 [options]"
    echo "Options:"
    echo "  -c, --clean     Clean build directory before building"
    echo "  -b, --build     Build only (don't run)"
    echo "  -r, --run       Run only (don't build)"
    echo "  -h, --help      Show this help message"
    exit 1
}

# Parse command line arguments
CLEAN_BUILD=false
BUILD_ONLY=false
RUN_ONLY=false

while [[ $# -gt 0 ]]; do
    case $1 in
        -c|--clean)
            CLEAN_BUILD=true
            shift
            ;;
        -b|--build)
            BUILD_ONLY=true
            shift
            ;;
        -r|--run)
            RUN_ONLY=true
            shift
            ;;
        -h|--help)
            usage
            ;;
        *)
            echo "Unknown option: $1"
            usage
            ;;
    esac
done

# Build phase
if [ "$RUN_ONLY" != true ]; then
    echo "🔨 Building TetrisSharp..."

    cd "$SCRIPT_DIR"

    if [ "$CLEAN_BUILD" = true ]; then
        echo "🧹 Cleaning build directory..."
        dotnet clean
    fi

    # Build the project
    echo "📦 Restoring packages and building..."
    if ! dotnet build; then
        echo "❌ Build failed!"
        exit 1
    fi

    # Fix SDL2 library conflicts
    echo "🔧 Fixing SDL2 library conflicts..."

    # Remove the runtimes directory to prevent conflicts
    if [ -d "$BUILD_DIR/runtimes" ]; then
        echo "📁 Removing conflicting runtimes directory..."
        rm -rf "$BUILD_DIR/runtimes"
    fi

    # Copy SDL2 libraries from the NuGet package cache
    NUGET_CACHE="$HOME/.nuget/packages/ppy.sdl2-cs/1.0.82/runtimes/osx-x64/native"

    if [ -d "$NUGET_CACHE" ]; then
        echo "📦 Copying SDL2 libraries from NuGet cache..."
        cp "$NUGET_CACHE/libSDL2.dylib" "$BUILD_DIR/" 2>/dev/null || {
            echo "⚠️  libSDL2.dylib not found in cache, searching..."
            find ~/.nuget/packages -name "libSDL2.dylib" -path "*/osx-x64/*" | head -1 | while read sdl_lib; do
                if [ -f "$sdl_lib" ]; then
                    echo "📦 Found SDL2 library: $sdl_lib"
                    cp "$sdl_lib" "$BUILD_DIR/"
                fi
            done
        }
    else
        echo "⚠️  NuGet cache not found, searching for SDL2 libraries..."
        find ~/.nuget/packages -name "libSDL2.dylib" -path "*/osx-x64/*" | head -1 | while read sdl_lib; do
            if [ -f "$sdl_lib" ]; then
                echo "📦 Found SDL2 library: $sdl_lib"
                cp "$sdl_lib" "$BUILD_DIR/"
            fi
        done
    fi

    # Create a symbolic link with the expected name for SDL2_ttf
    cd "$BUILD_DIR"
    if [ -f "libSDL2.dylib" ]; then
        ln -sf "libSDL2.dylib" "libSDL2-2.0.0.dylib"
        echo "🔗 Created symbolic link for SDL2_ttf compatibility"
    fi

    echo "✅ Build completed successfully!"

    if [ "$BUILD_ONLY" = true ]; then
        echo "📁 Build output location: $BUILD_DIR"
        echo "🎯 Run the game with: ./build_and_run.sh --run"
        exit 0
    fi
fi

# Run phase
if [ "$BUILD_ONLY" != true ]; then
    echo ""
    echo "🚀 Starting TetrisSharp..."
    echo "========================"

    # Check if the application was built
    if [ ! -f "$BUILD_DIR/TetrisSharp.dll" ]; then
        echo "❌ TetrisSharp.dll not found. Please build the project first:"
        echo "   ./build_and_run.sh --build"
        exit 1
    fi

    # Set environment variables to prevent library conflicts
    export DYLD_LIBRARY_PATH="$BUILD_DIR"
    export DYLD_FALLBACK_LIBRARY_PATH="$BUILD_DIR"

    echo "📁 Application directory: $BUILD_DIR"
    echo "🔧 Using isolated library environment"
    echo ""
    echo "🎮 Game Controls:"
    echo "   ← → : Move piece left/right"
    echo "   ↓   : Soft drop"
    echo "   ↑   : Rotate piece"
    echo "   Q   : Quit game"
    echo ""
    echo "Starting game..."
    echo "=================="

    # Change to the application directory and run
    cd "$BUILD_DIR"
    if ! dotnet TetrisSharp.dll; then
        echo ""
        echo "❌ Game exited with an error."
        echo ""
        echo "🔍 Troubleshooting:"
        echo "   1. Try rebuilding: ./build_and_run.sh --clean --build"
        echo "   2. Check system requirements: macOS with .NET 9.0"
        echo "   3. Ensure display is available (not running over SSH without X11)"
        exit 1
    fi

    echo ""
    echo "👋 Thanks for playing TetrisSharp!"
fi

echo "✨ Script completed successfully!"
