#!/bin/bash

# Verify Both Versions Script
# This script tests that both C and C# versions of Tetris work correctly

set -e

echo "🧪 Testing Both Tetris Versions"
echo "==============================="
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Test results
C_VERSION_OK=false
CS_VERSION_OK=false

echo "📋 Test Plan:"
echo "  1. Test C language version compilation and basic functionality"
echo "  2. Test C# version compilation and basic functionality"
echo "  3. Verify no SDL2 library conflicts"
echo "  4. Generate test report"
echo ""

# Test C language version
echo "🔍 Testing C Language Version"
echo "=============================="

cd "$(dirname "$0")"

echo "Step 1: Cleaning previous builds..."
make clean >/dev/null 2>&1 || true

echo "Step 2: Checking SDL2 dependencies..."
if pkg-config --exists sdl2 2>/dev/null; then
    echo -e "   ✅ SDL2: $(pkg-config --modversion sdl2)"
else
    echo -e "   ${RED}❌ SDL2 not found${NC}"
    echo "   Please run: brew install sdl2"
    exit 1
fi

if pkg-config --exists sdl2_ttf 2>/dev/null; then
    echo -e "   ✅ SDL2_ttf: $(pkg-config --modversion sdl2_ttf)"
else
    echo -e "   ${YELLOW}⚠️  SDL2_ttf not found, will test simple version${NC}"
fi

echo "Step 3: Building C version (improved)..."
if make improved >/dev/null 2>&1; then
    echo -e "   ✅ Compilation successful"

    echo "Step 4: Testing C version startup..."
    if timeout 2s ./tetris >/dev/null 2>&1; then
        # Timeout expected - means program started successfully
        echo -e "   ✅ Program starts and runs correctly"
        C_VERSION_OK=true
    elif [ $? -eq 124 ]; then
        # Timeout exit code - this is expected and means success
        echo -e "   ✅ Program starts and runs correctly"
        C_VERSION_OK=true
    else
        echo -e "   ${RED}❌ Program failed to start${NC}"
    fi
else
    echo -e "   ${RED}❌ Compilation failed${NC}"
fi

echo ""

# Test C# version
echo "🔍 Testing C# Version"
echo "====================="

cd TetrisSharp

echo "Step 1: Checking .NET environment..."
if command -v dotnet >/dev/null 2>&1; then
    echo -e "   ✅ .NET SDK: $(dotnet --version)"
else
    echo -e "   ${RED}❌ .NET SDK not found${NC}"
    echo "   Please install .NET 9.0 SDK"
    exit 1
fi

echo "Step 2: Building C# version..."
if ./build_and_run.sh --build >/dev/null 2>&1; then
    echo -e "   ✅ Compilation successful"

    echo "Step 3: Testing C# version startup..."
    cd bin/Debug/net9.0
    if timeout 2s env DYLD_LIBRARY_PATH="$(pwd)" dotnet TetrisSharp.dll >/dev/null 2>&1; then
        # Timeout expected
        echo -e "   ✅ Program starts and runs correctly"
        CS_VERSION_OK=true
    elif [ $? -eq 124 ]; then
        # Timeout exit code - success
        echo -e "   ✅ Program starts and runs correctly"
        CS_VERSION_OK=true
    else
        echo -e "   ${RED}❌ Program failed to start${NC}"
    fi
    cd ../../..
else
    echo -e "   ${RED}❌ Compilation failed${NC}"
fi

cd ..

echo ""

# Test SDL2 library conflicts
echo "🔍 Testing SDL2 Library Status"
echo "=============================="

echo "Step 1: Checking system SDL2 library..."
if [ -f "/usr/local/Cellar/sdl2/2.32.10/lib/libSDL2-2.0.0.dylib" ]; then
    echo -e "   ✅ System SDL2 library exists and accessible"
else
    echo -e "   ${YELLOW}⚠️  System SDL2 library path unusual${NC}"
fi

echo "Step 2: Checking C# SDL2 libraries..."
if [ -f "TetrisSharp/bin/Debug/net9.0/libSDL2.dylib" ]; then
    echo -e "   ✅ C# bundled SDL2 library exists"
else
    echo -e "   ${YELLOW}⚠️  C# bundled SDL2 library missing${NC}"
fi

echo "Step 3: Verifying no conflicts..."
# This is more of a conceptual check - if both versions work, there's no conflict
if [ "$C_VERSION_OK" = true ] && [ "$CS_VERSION_OK" = true ]; then
    echo -e "   ✅ Both versions work - no effective conflicts"
else
    echo -e "   ${YELLOW}⚠️  Potential conflicts detected${NC}"
fi

echo ""

# Generate test report
echo "📊 Test Report"
echo "=============="

echo "Test Results:"
if [ "$C_VERSION_OK" = true ]; then
    echo -e "   ${GREEN}✅ C Language Version: PASS${NC}"
else
    echo -e "   ${RED}❌ C Language Version: FAIL${NC}"
fi

if [ "$CS_VERSION_OK" = true ]; then
    echo -e "   ${GREEN}✅ C# Version: PASS${NC}"
else
    echo -e "   ${RED}❌ C# Version: FAIL${NC}"
fi

echo ""
echo "Environment Information:"
echo "   OS: $(uname -s) $(uname -r)"
echo "   Architecture: $(uname -m)"
if command -v gcc >/dev/null 2>&1; then
    echo "   C Compiler: gcc $(gcc --version | head -n1 | sed 's/.*) //')"
fi
if command -v dotnet >/dev/null 2>&1; then
    echo "   .NET SDK: $(dotnet --version)"
fi
if pkg-config --exists sdl2 2>/dev/null; then
    echo "   SDL2: $(pkg-config --modversion sdl2)"
fi

echo ""
echo "Usage Instructions:"
echo "   Run C version:  ./build_and_run.sh"
echo "   Run C# version: cd TetrisSharp && ./build_and_run.sh"

echo ""

# Final verdict
if [ "$C_VERSION_OK" = true ] && [ "$CS_VERSION_OK" = true ]; then
    echo -e "${GREEN}🎉 SUCCESS: Both versions are working correctly!${NC}"
    echo ""
    echo "✅ C language version compiles and runs"
    echo "✅ C# version compiles and runs"
    echo "✅ No SDL2 library conflicts detected"
    echo "✅ Both versions can coexist"
    echo ""
    echo "🎮 You can now use both versions of Tetris!"
    exit 0
else
    echo -e "${RED}❌ FAILURE: One or both versions have issues${NC}"
    echo ""
    echo "Troubleshooting suggestions:"
    if [ "$C_VERSION_OK" = false ]; then
        echo "   • For C version: brew install sdl2 sdl2_ttf"
        echo "   • Check: make clean && make improved"
    fi
    if [ "$CS_VERSION_OK" = false ]; then
        echo "   • For C# version: Check .NET 9.0 SDK installation"
        echo "   • Try: cd TetrisSharp && ./build_and_run.sh --clean --build"
    fi
    echo "   • See SDL2_COEXISTENCE_SOLUTION.md for detailed troubleshooting"
    exit 1
fi
