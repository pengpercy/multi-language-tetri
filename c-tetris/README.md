# Tetris in C

A complete Tetris implementation in C using SDL2, based on the Python pygame version. This C version maintains all the features and visual effects of the original Python game while providing better performance.

**Two versions available:**
- **Simplified version** (`tetris_simple.c`) - Only requires SDL2, uses basic rectangle-based text rendering
- **Full version** (`tetris.c`) - Requires SDL2 + SDL2_ttf, includes proper font rendering

## Features

- **Complete Tetris Gameplay**: All 7 tetromino pieces (I, O, T, S, Z, J, L) with proper rotation
- **Modern Visual Effects**: 
  - 3D-styled blocks with highlights and borders
  - Ghost piece showing drop position
  - Particle effects when clearing lines
  - Smooth animations and visual feedback
- **Game Mechanics**:
  - Line clearing with scoring system
  - Progressive level system with increasing speed
  - Soft drop and hard drop controls
  - Wall kick rotation system
- **User Interface**:
  - Real-time score, level, and lines display
  - Next piece preview
  - Pause and restart functionality
  - Control instructions in sidebar
- **Responsive Controls**:
  - Continuous movement when holding keys
  - Separate timing for different key types
  - Immediate response for rotation and drops

## Controls

| Key | Action |
|-----|--------|
| ← → | Move piece left/right |
| ↑ | Rotate piece clockwise |
| ↓ | Soft drop (faster fall) |
| Space | Hard drop (instant drop) |
| P | Pause/Resume game |
| R | Reset game |
| +/- | Adjust level |

## Prerequisites

Before building, you need to install SDL2 development libraries. For the full version, you'll also need SDL2_ttf:

### macOS (with Homebrew)
```bash
# For simplified version
brew install sdl2

# For full version (with proper fonts)
brew install sdl2 sdl2_ttf
```

### Ubuntu/Debian
```bash
# For simplified version
sudo apt-get install libsdl2-dev

# For full version (with proper fonts)
sudo apt-get install libsdl2-dev libsdl2-ttf-dev
```

### CentOS/RHEL/Fedora
```bash
# For simplified version
sudo yum install SDL2-devel
# or
sudo dnf install SDL2-devel

# For full version (with proper fonts)
sudo yum install SDL2-devel SDL2_ttf-devel
# or
sudo dnf install SDL2-devel SDL2_ttf-devel
```

### Windows
Download SDL2 and SDL2_ttf development libraries from:
- https://www.libsdl.org/download-2.0.php
- https://www.libsdl.org/projects/SDL_ttf/

## Building

1. **Check dependencies**:
   ```bash
   make deps
   ```

2. **Build the game**:
   ```bash
   # Build simplified version (default, easier setup)
   make
   # or explicitly
   make simple
   
   # Build full version with proper fonts (requires SDL2_ttf)
   make full
   ```

3. **Run the game**:
   ```bash
   make run
   # or
   ./tetris        # simplified version
   # or
   ./tetris_full   # full version
   ```

4. **Clean build files**:
   ```bash
   make clean
   ```

5. **Debug build**:
   ```bash
   make debug
   ```

## Game Rules

- **Scoring**: Points are awarded for:
  - Soft dropping pieces (1 point per cell)
  - Hard dropping pieces (1 point per cell)
  - Clearing lines: 100 × level (1 line), 300 × level (2 lines), 500 × level (3 lines), 800 × level (4 lines)

- **Levels**: Level increases every 10 lines cleared, making pieces fall faster

- **Game Over**: When a new piece cannot be placed at the top of the grid

## Code Structure

The C implementation is organized into several key components:

- **Game State**: `GameState` structure holds all game data
- **Tetromino System**: Shape definitions and rotation logic
- **Physics**: Collision detection and piece movement
- **Rendering**: SDL2-based graphics with visual effects
- **Input**: Keyboard handling with proper timing
- **Animation**: Particle effects and line clearing animations

## Differences from Python Version

While maintaining the same gameplay and visual style, the C version:

- Uses SDL2 instead of pygame for better performance
- Implements manual memory management
- Uses structures and functions instead of classes
- Provides more efficient rendering and game loop
- Maintains all visual effects including transparency and particles

**Simplified vs Full Version:**
- **Simplified**: Uses basic rectangle-based text rendering, easier to compile (only SDL2 needed)
- **Full**: Uses proper TrueType font rendering, requires SDL2_ttf library
- Both versions have identical gameplay and visual effects for the game grid and pieces

## Troubleshooting

### Common Issues

1. **"SDL2 not found" error**:
   - Make sure SDL2 development libraries are installed
   - Check the Makefile for correct library paths

2. **"SDL2_ttf not found" error (full version only)**:
   - Install SDL2_ttf development libraries
   - Or use the simplified version: `make simple`

3. **Font rendering issues (full version)**:
   - The game tries to use system fonts, but falls back to basic rendering
   - You can modify the font paths in the `init_game()` function

4. **Text appears as rectangles (simplified version)**:
   - This is normal - the simplified version uses basic rectangle-based text
   - For proper fonts, build the full version with SDL2_ttf

5. **Performance issues**:
   - The game targets 60 FPS; if your system can't maintain this, you can adjust the delay in the main loop

### Building on Different Systems

The Makefile automatically detects your operating system and adjusts compiler flags accordingly. If you encounter issues:

- On Linux: Ensure you have gcc and the development headers
- On macOS: Make sure Xcode command line tools are installed
- On Windows: Consider using MinGW or MSYS2 for compilation

## License

This implementation is based on the Python Tetris game and maintains the same open structure for educational purposes.

## Quick Start Summary

**Easiest setup (simplified version):**
```bash
# Install SDL2 only
brew install sdl2                    # macOS
sudo apt-get install libsdl2-dev     # Ubuntu

# Build and run
make
./tetris
```

**Full experience (with proper fonts):**
```bash
# Install SDL2 + SDL2_ttf
brew install sdl2 sdl2_ttf           # macOS
sudo apt-get install libsdl2-dev libsdl2-ttf-dev  # Ubuntu

# Build and run
make full
./tetris_full
```

## Contributing

Feel free to improve the code! Some areas for enhancement:
- Additional visual effects
- Sound system integration
- High score saving
- Different game modes
- Network multiplayer support
- Better bitmap font system for simplified version