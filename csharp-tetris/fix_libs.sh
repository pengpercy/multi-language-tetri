#!/bin/bash

# Fix SDL2 library conflicts by reorganizing the libraries

echo "🔧 Fixing SDL2 library conflicts..."

# Get the build directory
BUILD_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)/bin/Debug/net9.0"

if [ ! -d "$BUILD_DIR" ]; then
    echo "❌ Build directory not found. Please run 'dotnet build' first."
    exit 1
fi

# Remove the runtimes directory to prevent conflicts
if [ -d "$BUILD_DIR/runtimes" ]; then
    echo "📁 Removing runtimes directory..."
    rm -rf "$BUILD_DIR/runtimes"
fi

# Copy SDL2 libraries from the NuGet package cache
NUGET_CACHE="$HOME/.nuget/packages/ppy.sdl2-cs/1.0.82/runtimes/osx-x64/native"

if [ -d "$NUGET_CACHE" ]; then
    echo "📦 Copying SDL2 libraries from NuGet cache..."
    cp "$NUGET_CACHE/libSDL2.dylib" "$BUILD_DIR/" 2>/dev/null || echo "⚠️  libSDL2.dylib not found in cache"
else
    echo "⚠️  NuGet cache not found, trying alternative approach..."

    # Restore the package to ensure libraries are available
    cd "$(dirname "${BASH_SOURCE[0]}")"
    dotnet restore

    # Try to find SDL2 in the restored packages
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

echo "✅ SDL2 library reorganization complete!"
echo "📁 Libraries in $BUILD_DIR:"
ls -la "$BUILD_DIR"/*.dylib 2>/dev/null || echo "No .dylib files found"
