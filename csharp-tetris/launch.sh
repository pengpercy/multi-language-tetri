#!/bin/bash

# TetrisSharp Launch Script
# This script prevents SDL2 library conflicts using environment isolation

# Get the directory where this script is located
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
APP_DIR="$SCRIPT_DIR/bin/Debug/net9.0"

# Check if the application exists
if [ ! -f "$APP_DIR/TetrisSharp.dll" ]; then
    echo "❌ TetrisSharp.dll not found. Please build the project first:"
    echo "   dotnet build"
    exit 1
fi

# Clear potentially conflicting environment variables
unset DYLD_LIBRARY_PATH
unset DYLD_FALLBACK_LIBRARY_PATH
unset DYLD_INSERT_LIBRARIES

# Set environment to use only our bundled libraries
export DYLD_LIBRARY_PATH="$APP_DIR"
export DYLD_FALLBACK_LIBRARY_PATH="$APP_DIR"
export DYLD_FORCE_FLAT_NAMESPACE=1

echo "🎮 Launching TetrisSharp..."
echo "📁 Application directory: $APP_DIR"
echo "🔧 Using isolated library environment"

# Change to the application directory and run using dotnet
cd "$APP_DIR"
exec dotnet TetrisSharp.dll "$@"
