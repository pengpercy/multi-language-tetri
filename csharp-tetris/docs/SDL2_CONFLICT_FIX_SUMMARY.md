# SDL2 Conflict Fix Summary

## Problem
The TetrisSharp application was experiencing fatal crashes due to SDL2 library conflicts on macOS. The error manifested as:

```
objc[52040]: Class SDLApplication is implemented in both /Users/percy/Documents/workspace/vibecode/TetrisSharp/bin/Debug/net9.0/runtimes/osx-x64/native/libSDL2.dylib and /usr/local/Cellar/sdl2/2.32.10/lib/libSDL2-2.0.0.dylib. This may cause spurious casting failures and mysterious crashes.
```

This resulted in segmentation faults and immediate application termination.

## Root Cause
The conflict occurred because:
1. **NuGet Package SDL2**: The `ppy.SDL2-CS` package bundles its own SDL2 libraries in `runtimes/osx-x64/native/`
2. **System SDL2**: Homebrew had installed SDL2 system-wide at `/usr/local/Cellar/sdl2/2.32.10/lib/`
3. **Dual Loading**: Both libraries were being loaded simultaneously, causing symbol conflicts
4. **Runtime Dependencies**: SDL2_ttf libraries had hardcoded dependencies on system SDL2 paths

## Solution Implementation

### 1. Library Reorganization
- **Removed** the problematic `runtimes` directory after build
- **Copied** SDL2 libraries directly to the main output directory
- **Created** symbolic links for compatibility (`libSDL2-2.0.0.dylib` → `libSDL2.dylib`)

### 2. Build Process Automation
Created `build_and_run.sh` script that:
- Builds the project normally
- Automatically fixes library conflicts post-build
- Sets appropriate environment variables
- Provides comprehensive error handling

### 3. Environment Isolation
```bash
export DYLD_LIBRARY_PATH="$BUILD_DIR"
export DYLD_FALLBACK_LIBRARY_PATH="$BUILD_DIR"
```

## Files Modified/Created

### New Files
- `build_and_run.sh` - Main build and run script with conflict resolution
- `fix_libs.sh` - Library reorganization utility
- `launch.sh` - Alternative launcher with environment isolation

### Modified Files
- `TetrisSharp.csproj` - Simplified to avoid self-contained deployment issues
- `Program.cs` - Kept clean without runtime library path manipulation

## Results

### ✅ Fixed Issues
- **No more crashes**: Application starts and runs without segmentation faults
- **No more library conflicts**: Eliminated duplicate SDL2 library warnings
- **Stable execution**: Game runs reliably with proper input handling
- **Fallback fonts**: When TTF fails, bitmap fonts work as backup

### ⚠️ Known Limitations
- SDL2_ttf may not load due to path dependencies (fallback to bitmap fonts)
- Requires running fix script after each clean build
- macOS specific solution (other platforms may need different approaches)

## Usage Instructions

### Quick Start
```bash
# Build and run in one command
./build_and_run.sh

# Build only
./build_and_run.sh --build

# Run only (after building)
./build_and_run.sh --run

# Clean build
./build_and_run.sh --clean
```

### Manual Process
```bash
# 1. Build normally
dotnet build

# 2. Fix library conflicts
./fix_libs.sh

# 3. Run with proper environment
cd bin/Debug/net9.0
dotnet TetrisSharp.dll
```

## Technical Details

### Library Conflict Resolution Strategy
1. **Elimination**: Remove duplicate runtime libraries
2. **Consolidation**: Use single SDL2 library source
3. **Symlinking**: Create compatibility links for different naming conventions
4. **Environment Isolation**: Use DYLD variables to control library loading

### Environment Variables Used
- `DYLD_LIBRARY_PATH`: Primary library search path
- `DYLD_FALLBACK_LIBRARY_PATH`: Fallback library location
- `DYLD_FORCE_FLAT_NAMESPACE`: Attempted namespace flattening (limited effectiveness)

## Alternative Solutions Attempted

### ❌ Runtime Configuration
- Tried `runtimeconfig.template.json` modifications
- Limited effectiveness due to .NET Core library loading behavior

### ❌ Self-Contained Deployment
- Attempted with `<SelfContained>true</SelfContained>`
- Created additional conflicts between bundled and system libraries

### ❌ Environment Variables Only
- DYLD variables have limited effectiveness in modern macOS
- Security restrictions prevent some dynamic library manipulation

### ❌ System Library Removal
- Temporary system library renaming required sudo access
- Not sustainable for development workflow

## Best Practices for Future

1. **Avoid System SDL2**: Don't install SDL2 via Homebrew if using .NET SDL2 packages
2. **Post-Build Scripts**: Always include library conflict resolution in build process
3. **Environment Isolation**: Use DYLD variables for additional safety
4. **Testing**: Verify on clean systems without system SDL2 installation
5. **Documentation**: Maintain clear build instructions for team members

## Verification

The fix can be verified by:
1. No crash messages in console output
2. Game window opens successfully
3. Input controls work properly
4. No SDL2 conflict warnings in terminal

## Impact

This fix enables reliable development and execution of SDL2-based .NET applications on macOS systems that have system-wide SDL2 installations, which is common in development environments with multiple SDL2-dependent tools.