#!/bin/bash

# Tetris in C - Quick Build and Run Script
# This script automatically detects dependencies and builds the appropriate version

set -e  # Exit on any error

echo "🎮 Tetris in C - Quick Start Script"
echo "=================================="

# Function to check if a command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Function to check if a library exists
check_library() {
    if command_exists pkg-config; then
        pkg-config --exists "$1" 2>/dev/null
    else
        # Fallback check for macOS/Homebrew
        if [[ "$OSTYPE" == "darwin"* ]]; then
            # Check for SDL2_ttf header file
            if [[ "$1" == "sdl2_ttf" ]]; then
                [ -f "/usr/local/include/SDL2/SDL_ttf.h" ] || [ -f "/opt/homebrew/include/SDL2/SDL_ttf.h" ]
            else
                brew list "$1" >/dev/null 2>&1
            fi
        else
            # Try to find library files directly
            find /usr/lib /usr/local/lib /opt/homebrew/lib -name "lib$1*" 2>/dev/null | head -1 | grep -q .
        fi
    fi
}

# Clean any previous builds
echo "🧹 Cleaning previous builds..."
make clean >/dev/null 2>&1 || true

# Check for required tools
if ! command_exists gcc && ! command_exists clang; then
    echo "❌ Error: No C compiler found (gcc or clang required)"
    echo "Please install build tools:"
    echo "  macOS: xcode-select --install"
    echo "  Ubuntu: sudo apt-get install build-essential"
    exit 1
fi

if ! command_exists make; then
    echo "❌ Error: make not found"
    echo "Please install make utility"
    exit 1
fi

# Check for SDL2
echo "🔍 Checking for SDL2..."
if check_library sdl2; then
    echo "✅ SDL2 found"
    SDL2_AVAILABLE=true
else
    echo "❌ SDL2 not found"
    echo "Please install SDL2:"
    echo "  macOS: brew install sdl2"
    echo "  Ubuntu: sudo apt-get install libsdl2-dev"
    echo "  CentOS/RHEL: sudo yum install SDL2-devel"
    exit 1
fi

# Check for SDL2_ttf
echo "🔍 Checking for SDL2_ttf..."
if check_library sdl2_ttf; then
    echo "✅ SDL2_ttf found"
    TTF_AVAILABLE=true
else
    echo "⚠️  SDL2_ttf not found - will build simplified version"
    TTF_AVAILABLE=false
fi

echo ""
echo "🎯 Available versions:"
echo "   - improved: Bitmap fonts, no TTF needed (recommended)"
if [ "$TTF_AVAILABLE" = true ]; then
    echo "   - fixed: TrueType fonts with Chinese support"
    echo "   - full: Complete TrueType font version"
fi
echo "   - simple: Basic version with minimal text"
echo ""

# Ask user which version to build (with timeout for non-interactive)
if [ "$TTF_AVAILABLE" = true ]; then
    echo "🤔 Which version would you like to build?"
    echo "   1) improved - Bitmap fonts (recommended, no TTF needed)"
    echo "   2) fixed    - TrueType fonts with Chinese support"
    echo "   3) full     - Complete TrueType version"
    echo "   4) simple   - Basic version"

    # Check if running interactively
    if [ -t 0 ] && [ -t 1 ]; then
        read -p "Enter choice (1-4) [1]: " choice
        choice=${choice:-1}
    else
        echo "Non-interactive mode detected - using default: improved version"
        choice=1
    fi
else
    echo "🤔 Which version would you like to build?"
    echo "   1) improved - Bitmap fonts (recommended)"
    echo "   2) simple   - Basic version"

    # Check if running interactively
    if [ -t 0 ] && [ -t 1 ]; then
        read -p "Enter choice (1-2) [1]: " choice
        choice=${choice:-1}
    else
        echo "Non-interactive mode detected - using default: improved version"
        choice=1
    fi
fi

echo ""
case $choice in
    1)
        echo "🔨 Building improved version (with bitmap fonts)..."
        if make improved; then
            echo "✅ Improved version built successfully!"
            EXECUTABLE="./tetris"
            VERSION="improved"
        else
            echo "❌ Build failed!"
            exit 1
        fi
        ;;
    2)
        if [ "$TTF_AVAILABLE" = true ]; then
            echo "🔨 Building fixed version (with TrueType fonts)..."
            if make fixed; then
                echo "✅ Fixed version built successfully!"
                EXECUTABLE="./tetris_fixed"
                VERSION="fixed"
            else
                echo "❌ Build failed!"
                exit 1
            fi
        else
            echo "🔨 Building simple version..."
            if make simple; then
                echo "✅ Simple version built successfully!"
                EXECUTABLE="./tetris_simple"
                VERSION="simple"
            else
                echo "❌ Build failed!"
                exit 1
            fi
        fi
        ;;
    3)
        if [ "$TTF_AVAILABLE" = true ]; then
            echo "🔨 Building full version..."
            if make full; then
                echo "✅ Full version built successfully!"
                EXECUTABLE="./tetris_full"
                VERSION="full"
            else
                echo "❌ Build failed!"
                exit 1
            fi
        else
            echo "❌ Invalid choice - TTF not available"
            exit 1
        fi
        ;;
    4)
        if [ "$TTF_AVAILABLE" = true ]; then
            echo "🔨 Building simple version..."
            if make simple; then
                echo "✅ Simple version built successfully!"
                EXECUTABLE="./tetris_simple"
                VERSION="simple"
            else
                echo "❌ Build failed!"
                exit 1
            fi
        else
            echo "❌ Invalid choice"
            exit 1
        fi
        ;;
    *)
        echo "❌ Invalid choice, building improved version by default..."
        if make improved; then
            echo "✅ Improved version built successfully!"
            EXECUTABLE="./tetris"
            VERSION="improved"
        else
            echo "❌ Build failed!"
            exit 1
        fi
        ;;
esac

# Show version info
echo ""
echo "📋 Build Information:"
echo "   Version: $VERSION"
echo "   Executable: $EXECUTABLE"
case $VERSION in
    "improved")
        echo "   Note: Uses bitmap-style fonts for clear text display"
        echo "   Features: Fixed animations, language switching (L key)"
        ;;
    "fixed")
        echo "   Note: Uses TrueType fonts with perfect Chinese support"
        echo "   Features: Beautiful font rendering, all languages supported"
        ;;
    "full")
        echo "   Note: Complete version with TrueType fonts"
        echo "   Features: All functionality with professional text rendering"
        ;;
    "simple")
        echo "   Note: Text will appear as colored rectangles (basic display)"
        echo "   Upgrade: Install SDL2_ttf and choose option 2 for better fonts"
        ;;
esac
echo ""

# Check if running in interactive mode
if [ -t 0 ] && [ -t 1 ]; then
    # Interactive mode - ask user if they want to run the game
    read -p "🚀 Run the game now? (y/N): " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        echo "🎮 Starting Tetris..."
        echo ""
        echo "Controls:"
        echo "  ← → : Move piece"
        echo "  ↑   : Rotate piece"
        echo "  ↓   : Soft drop"
        echo "  Space: Hard drop"
        echo "  P   : Pause/Start"
        echo "  R   : Reset game"
        echo "  +/- : Adjust level"
        echo "  L   : Switch language"
        echo ""
        echo "Press any key to start the game, then press P to begin playing..."
        $EXECUTABLE
    else
        echo "Game built successfully! Run with: $EXECUTABLE"
    fi
else
    # Non-interactive mode - just show completion message
    echo "🎉 Build completed successfully!"
    echo "   Executable: $EXECUTABLE"
    echo "   To run: cd $(pwd) && $EXECUTABLE"
    echo ""
    echo "Controls:"
    echo "  ← → : Move piece"
    echo "  ↑   : Rotate piece"
    echo "  ↓   : Soft drop"
    echo "  Space: Hard drop"
    echo "  P   : Pause/Start"
    echo "  R   : Reset game"
    echo "  +/- : Adjust level"
    echo "  L   : Switch language"
fi

echo ""
echo "🎉 Thanks for playing Tetris in C!"
