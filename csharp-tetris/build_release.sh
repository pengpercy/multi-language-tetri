#!/bin/bash

# TetrisSharp Release Build Script
# 高性能、小体积的release构建

set -e

echo "🚀 TetrisSharp - Release Build Script"
echo "===================================="

# Get the directory where this script is located
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
RELEASE_DIR="$SCRIPT_DIR/bin/Release/net9.0/osx-x64"
PUBLISH_DIR="$SCRIPT_DIR/bin/Release/net9.0/osx-x64/publish"

# Function to display usage
usage() {
    echo "Usage: $0 [options]"
    echo "Options:"
    echo "  -c, --clean     Clean all build artifacts before building"
    echo "  -t, --test      Test the release build after compilation"
    echo "  -s, --size      Show detailed size information"
    echo "  -h, --help      Show this help message"
    exit 1
}

# Parse command line arguments
CLEAN_BUILD=false
TEST_BUILD=false
SHOW_SIZE=false

while [[ $# -gt 0 ]]; do
    case $1 in
        -c|--clean)
            CLEAN_BUILD=true
            shift
            ;;
        -t|--test)
            TEST_BUILD=true
            shift
            ;;
        -s|--size)
            SHOW_SIZE=true
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

echo "🎯 Build Configuration:"
echo "  - Target: Release (AOT + Trimmed + Single File)"
echo "  - Runtime: osx-x64"
echo "  - Optimizations: Maximum performance + Minimum size"
echo ""

cd "$SCRIPT_DIR"

if [ "$CLEAN_BUILD" = true ]; then
    echo "🧹 Cleaning all build artifacts..."
    dotnet clean --configuration Release --verbosity minimal
    rm -rf bin/Release obj/Release
fi

echo "📦 Restoring packages..."
if ! dotnet restore --runtime osx-x64; then
    echo "❌ Package restore failed!"
    exit 1
fi

echo "🔨 Building Release version..."
echo "⏱️  This may take a few minutes for AOT compilation..."

# Release build with maximum optimizations
if ! dotnet publish \
    --configuration Release \
    --runtime osx-x64 \
    --self-contained true \
    --output "$PUBLISH_DIR" \
    -p:PublishAot=true \
    -p:PublishTrimmed=true \
    -p:PublishSingleFile=true \
    -p:TrimMode=full \
    -p:OptimizationPreference=Speed \
    -p:IlcOptimizationPreference=Speed \
    -p:IlcFoldIdenticalMethodBodies=true \
    -p:IlcDisableReflection=true \
    -p:DebugType=none \
    -p:DebugSymbols=false \
    -p:EventSourceSupport=false \
    -p:UseSystemResourceKeys=true \
    -p:InvariantGlobalization=true \
    -p:DebuggerSupport=false \
    -p:EnableUnsafeUTF7Encoding=false \
    --verbosity minimal; then
    echo "❌ Release build failed!"
    exit 1
fi

# Fix SDL2 library conflicts for release build
echo "🔧 Fixing SDL2 library conflicts for release..."

# Remove conflicting runtimes directory
if [ -d "$PUBLISH_DIR/runtimes" ]; then
    echo "📁 Removing conflicting runtimes directory..."
    rm -rf "$PUBLISH_DIR/runtimes"
fi

# Copy and setup SDL2 libraries
NUGET_CACHE="$HOME/.nuget/packages/ppy.sdl2-cs/1.0.82/runtimes/osx-x64/native"

if [ -d "$NUGET_CACHE" ]; then
    echo "📦 Setting up SDL2 libraries..."
    cp "$NUGET_CACHE/libSDL2.dylib" "$PUBLISH_DIR/" 2>/dev/null || {
        find ~/.nuget/packages -name "libSDL2.dylib" -path "*/osx-x64/*" | head -1 | while read sdl_lib; do
            if [ -f "$sdl_lib" ]; then
                cp "$sdl_lib" "$PUBLISH_DIR/"
            fi
        done
    }

    # Create compatibility symlink
    cd "$PUBLISH_DIR"
    if [ -f "libSDL2.dylib" ]; then
        ln -sf "libSDL2.dylib" "libSDL2-2.0.0.dylib"
    fi
fi

# Copy SDL2_ttf libraries from debug build
DEBUG_BUILD_DIR="$SCRIPT_DIR/bin/Debug/net9.0"
if [ -f "$DEBUG_BUILD_DIR/SDL2_ttf.dylib" ]; then
    echo "📦 Setting up SDL2_ttf libraries..."
    cp "$DEBUG_BUILD_DIR/SDL2_ttf.dylib" "$PUBLISH_DIR/"
fi
if [ -f "$DEBUG_BUILD_DIR/libSDL2_ttf.dylib" ]; then
    cp "$DEBUG_BUILD_DIR/libSDL2_ttf.dylib" "$PUBLISH_DIR/"
fi

echo "✅ Release build completed successfully!"

# Show size information
if [ "$SHOW_SIZE" = true ]; then
    echo ""
    echo "📊 Build Size Information:"
    echo "========================="

    if [ -f "$PUBLISH_DIR/TetrisSharp" ]; then
        EXECUTABLE_SIZE=$(ls -lh "$PUBLISH_DIR/TetrisSharp" | awk '{print $5}')
        echo "🎮 Executable size: $EXECUTABLE_SIZE"
    fi

    if [ -d "$PUBLISH_DIR" ]; then
        TOTAL_SIZE=$(du -sh "$PUBLISH_DIR" | awk '{print $1}')
        echo "📁 Total release size: $TOTAL_SIZE"

        echo ""
        echo "📋 Release contents:"
        ls -lah "$PUBLISH_DIR"
    fi
fi

# Test the release build
if [ "$TEST_BUILD" = true ]; then
    echo ""
    echo "🧪 Testing release build..."
    echo "========================="

    if [ ! -f "$PUBLISH_DIR/TetrisSharp" ]; then
        echo "❌ Release executable not found!"
        exit 1
    fi

    # Set environment for testing
    export DYLD_LIBRARY_PATH="$PUBLISH_DIR"
    export DYLD_FALLBACK_LIBRARY_PATH="$PUBLISH_DIR"

    echo "🎮 Starting release version (will quit after 3 seconds)..."
    cd "$PUBLISH_DIR"

    # Test run for 3 seconds
    timeout 3s ./TetrisSharp 2>/dev/null || {
        if [ $? -eq 124 ]; then
            echo "✅ Release version started successfully!"
        else
            echo "❌ Release version failed to start!"
            exit 1
        fi
    }
fi

echo ""
echo "🎉 Release Build Summary:"
echo "========================"
echo "📁 Release location: $PUBLISH_DIR"
echo "🎯 Optimizations:"
echo "   ✅ AOT Compilation (Native performance)"
echo "   ✅ Full Trimming (Reduced size)"
echo "   ✅ Single File (Easy deployment)"
echo "   ✅ Speed Optimizations"
echo "   ✅ SDL2 Conflicts Resolved"
echo ""
echo "🚀 To run the release version:"
echo "   cd \"$PUBLISH_DIR\""
echo "   ./TetrisSharp"
echo ""
echo "📦 To distribute:"
echo "   Archive the contents of: $PUBLISH_DIR"
echo ""

if [ "$SHOW_SIZE" = false ]; then
    echo "💡 Use --size flag to see detailed size information"
fi

if [ "$TEST_BUILD" = false ]; then
    echo "💡 Use --test flag to test the release build"
fi

echo "✨ Release build completed successfully!"
