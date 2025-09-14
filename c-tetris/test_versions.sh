#!/bin/bash

# Test script for all Tetris versions
# This script compiles and tests all available versions

echo "🧪 Testing All Tetris Versions"
echo "=============================="

# Clean previous builds
echo "🧹 Cleaning previous builds..."
make clean

# Test results
RESULTS=()

# Test improved version (no TTF needed)
echo ""
echo "📝 Test 1: Improved Version (tetris_improved.c)"
echo "------------------------------------------------"
if make improved; then
    if [ -f "./tetris" ]; then
        echo "✅ Improved version: PASS (executable created)"
        RESULTS+=("improved: PASS")
    else
        echo "❌ Improved version: FAIL (no executable)"
        RESULTS+=("improved: FAIL")
    fi
else
    echo "❌ Improved version: FAIL (compilation error)"
    RESULTS+=("improved: FAIL")
fi

# Test simple version
echo ""
echo "📝 Test 2: Simple Version (tetris_simple.c)"
echo "--------------------------------------------"
if make simple; then
    if [ -f "./tetris_simple" ]; then
        echo "✅ Simple version: PASS (executable created)"
        RESULTS+=("simple: PASS")
    else
        echo "❌ Simple version: FAIL (no executable)"
        RESULTS+=("simple: FAIL")
    fi
else
    echo "❌ Simple version: FAIL (compilation error)"
    RESULTS+=("simple: FAIL")
fi

# Test TTF versions if available
TTF_AVAILABLE=false
if [ -f "/usr/local/include/SDL2/SDL_ttf.h" ] || [ -f "/opt/homebrew/include/SDL2/SDL_ttf.h" ]; then
    TTF_AVAILABLE=true
    echo "✅ SDL2_ttf detected - testing TTF versions"
else
    echo "⚠️  SDL2_ttf not found - skipping TTF versions"
fi

if [ "$TTF_AVAILABLE" = true ]; then
    # Test fixed version
    echo ""
    echo "📝 Test 3: Fixed Version (tetris_fixed.c)"
    echo "------------------------------------------"
    if make fixed; then
        if [ -f "./tetris_fixed" ]; then
            echo "✅ Fixed version: PASS (executable created)"
            RESULTS+=("fixed: PASS")
        else
            echo "❌ Fixed version: FAIL (no executable)"
            RESULTS+=("fixed: FAIL")
        fi
    else
        echo "❌ Fixed version: FAIL (compilation error)"
        RESULTS+=("fixed: FAIL")
    fi

    # Test full version
    echo ""
    echo "📝 Test 4: Full Version (tetris.c)"
    echo "-----------------------------------"
    if make full; then
        if [ -f "./tetris_full" ]; then
            echo "✅ Full version: PASS (executable created)"
            RESULTS+=("full: PASS")
        else
            echo "❌ Full version: FAIL (no executable)"
            RESULTS+=("full: FAIL")
        fi
    else
        echo "❌ Full version: FAIL (compilation error)"
        RESULTS+=("full: FAIL")
    fi
else
    RESULTS+=("fixed: SKIP (no TTF)")
    RESULTS+=("full: SKIP (no TTF)")
fi

# Display results summary
echo ""
echo "📊 Test Results Summary"
echo "======================"
for result in "${RESULTS[@]}"; do
    if [[ $result == *"PASS"* ]]; then
        echo "✅ $result"
    elif [[ $result == *"FAIL"* ]]; then
        echo "❌ $result"
    else
        echo "⚠️  $result"
    fi
done

# Count results
PASS_COUNT=$(printf '%s\n' "${RESULTS[@]}" | grep -c "PASS")
FAIL_COUNT=$(printf '%s\n' "${RESULTS[@]}" | grep -c "FAIL")
SKIP_COUNT=$(printf '%s\n' "${RESULTS[@]}" | grep -c "SKIP")

echo ""
echo "📈 Summary: $PASS_COUNT passed, $FAIL_COUNT failed, $SKIP_COUNT skipped"

# Recommendations
echo ""
echo "🎯 Recommendations:"
if [[ $PASS_COUNT -gt 0 ]]; then
    echo "   - Use 'improved' version for best compatibility (no TTF needed)"
    if [[ "$TTF_AVAILABLE" = true ]]; then
        echo "   - Use 'fixed' version for perfect Chinese font support"
    else
        echo "   - Install SDL2_ttf with: brew install sdl2_ttf (macOS) or apt-get install libsdl2-ttf-dev (Linux)"
    fi
else
    echo "   - Check SDL2 installation: brew install sdl2 (macOS) or apt-get install libsdl2-dev (Linux)"
fi

# Quick start instructions
echo ""
echo "🚀 Quick Start:"
echo "   ./build_and_run.sh    # Interactive build and run"
echo "   make && ./tetris       # Build and run improved version"
if [[ "$TTF_AVAILABLE" = true ]]; then
    echo "   make fixed && ./tetris_fixed  # Build and run Chinese version"
fi

echo ""
echo "🎮 Controls:"
echo "   ← →  : Move piece"
echo "   ↑    : Rotate piece"
echo "   ↓    : Soft drop"
echo "   Space: Hard drop"
echo "   P    : Pause/Start"
echo "   R    : Reset"
echo "   L    : Switch language (improved/fixed versions)"
echo "   +/-  : Adjust level"

if [[ $FAIL_COUNT -eq 0 ]]; then
    echo ""
    echo "🎉 All tests passed! Ready to play Tetris!"
    exit 0
else
    echo ""
    echo "⚠️  Some tests failed. Check compilation errors above."
    exit 1
fi
