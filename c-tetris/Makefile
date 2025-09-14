# Makefile for Tetris in C
# Requires SDL2 and SDL2_ttf libraries

CC = gcc
CFLAGS = -Wall -Wextra -std=c99 -O2

# Use pkg-config for better dependency detection
PKG_CONFIG := $(shell which pkg-config)

# Base SDL2 settings
SDL2_CFLAGS := $(shell pkg-config --cflags sdl2 2>/dev/null || echo "-I/usr/local/include -I/opt/homebrew/include")
SDL2_LIBS := $(shell pkg-config --libs sdl2 2>/dev/null || echo "-L/usr/local/lib -L/opt/homebrew/lib -lSDL2")

# SDL2_ttf settings
SDL2_TTF_CFLAGS := $(shell pkg-config --cflags sdl2_ttf 2>/dev/null || echo "")
SDL2_TTF_LIBS := $(shell pkg-config --libs sdl2_ttf 2>/dev/null || echo "-lSDL2_ttf")

# Combine flags
CFLAGS += $(SDL2_CFLAGS)
LDFLAGS +=
LIBS = $(SDL2_LIBS) -lm

# Detect operating system for fallback
UNAME_S := $(shell uname -s)
ifeq ($(UNAME_S),Linux)
    CFLAGS += -D_GNU_SOURCE
endif

SIMPLE_TARGET = tetris_simple
SIMPLE_SOURCE = tetris_simple.c
FULL_TARGET = tetris_full
FULL_SOURCE = tetris_fixed.c
FIXED_TARGET = tetris_fixed
FIXED_SOURCE = tetris_fixed.c
IMPROVED_TARGET = tetris
IMPROVED_SOURCE = tetris_improved.c
FULL_FIXED_TARGET = tetris_full_fixed
FULL_FIXED_SOURCE = tetris_fixed.c
DEBUG_TARGET = tetris_fixed_debug
DEBUG_SOURCE = tetris_fixed.c

.PHONY: all clean install deps simple full fixed improved full_fixed debug test_font test_text

all: improved

$(SIMPLE_TARGET): $(SIMPLE_SOURCE)
	$(CC) $(CFLAGS) $(LDFLAGS) -o $@ $< $(LIBS)

# Build simplified version
simple: $(SIMPLE_TARGET)

# Build full version with TTF support
full: $(FULL_TARGET)

$(FULL_TARGET): $(FULL_SOURCE)
	$(CC) $(CFLAGS) $(SDL2_TTF_CFLAGS) $(LDFLAGS) -DHAVE_SDL_TTF -o $@ $< $(LIBS) $(SDL2_TTF_LIBS)

# Build fixed version with Chinese support
fixed: $(FIXED_TARGET)

$(FIXED_TARGET): $(FIXED_SOURCE)
	$(CC) $(CFLAGS) $(SDL2_TTF_CFLAGS) $(LDFLAGS) -o $@ $< $(LIBS) $(SDL2_TTF_LIBS)

# Build improved version (default - no TTF needed)
improved: $(IMPROVED_TARGET)

$(IMPROVED_TARGET): $(IMPROVED_SOURCE)
	$(CC) $(CFLAGS) $(LDFLAGS) -o $@ $< $(LIBS)

# Build full fixed version with corrected compilation errors
full_fixed: $(FULL_FIXED_TARGET)

$(FULL_FIXED_TARGET): $(FULL_FIXED_SOURCE)
	$(CC) $(CFLAGS) $(SDL2_TTF_CFLAGS) $(LDFLAGS) -DHAVE_SDL_TTF -o $@ $< $(LIBS) $(SDL2_TTF_LIBS)

# Build debug version with detailed font loading info
debug: $(DEBUG_TARGET)

$(DEBUG_TARGET): $(DEBUG_SOURCE)
	$(CC) $(CFLAGS) $(SDL2_TTF_CFLAGS) $(LDFLAGS) -o $@ $< $(LIBS) $(SDL2_TTF_LIBS)

# Test font availability
test_font: font_test
	./font_test

font_test: font_test.c
	$(CC) $(CFLAGS) $(SDL2_TTF_CFLAGS) -o $@ $< $(LIBS) $(SDL2_TTF_LIBS)

# Test text rendering
test_text: text_test
	./text_test

text_test: text_test.c
	$(CC) $(CFLAGS) $(SDL2_TTF_CFLAGS) -o $@ $< $(LIBS) $(SDL2_TTF_LIBS)

clean:
	rm -f $(SIMPLE_TARGET) $(FULL_TARGET) $(FIXED_TARGET) $(IMPROVED_TARGET) $(FULL_FIXED_TARGET) $(DEBUG_TARGET) font_test text_test

# Install dependencies
deps:
ifeq ($(UNAME_S),Linux)
	@echo "Installing dependencies for Linux..."
	@echo "For simplified version: sudo apt-get install libsdl2-dev"
	@echo "For full version: sudo apt-get install libsdl2-dev libsdl2-ttf-dev"
	@echo "Or: sudo yum install SDL2-devel [SDL2_ttf-devel]"
endif
ifeq ($(UNAME_S),Darwin)
	@echo "Installing dependencies for macOS..."
	@echo "Make sure you have Homebrew installed, then run:"
	@echo "For simplified version: brew install sdl2"
	@echo "For full version: brew install sdl2 sdl2_ttf"
endif

# Run the game
run: $(TARGET)
	./$(TARGET)

# Debug build
debug: CFLAGS += -g -DDEBUG
debug: $(TARGET)

# Help
help:
	@echo "Tetris in C - Build Instructions"
	@echo "================================"
	@echo ""
	@echo "Available targets:"
	@echo "  all      - Build the improved version (default)"
	@echo "  improved - Build improved version with bitmap fonts"
	@echo "  simple   - Build simplified version (no TTF)"
	@echo "  full     - Build full version (requires SDL2_ttf)"
	@echo "  fixed    - Build fixed version with Chinese support"
	@echo "  clean     - Remove built files"
	@echo "  deps      - Show dependency installation instructions"
	@echo "  run       - Build and run the game"
	@echo "  debug     - Build debug version with font loading info"
	@echo "  full_fixed- Build full version with compilation fixes"
	@echo "  test_font - Test font availability on system"
	@echo "  test_text - Test text rendering (visual)"
	@echo "  help      - Show this help message"
	@echo ""
	@echo "Prerequisites:"
	@echo "  - SDL2 development libraries (required)"
	@echo "  - SDL2_ttf development libraries (for full version only)"
	@echo "  - C compiler (gcc/clang)"
	@echo ""
	@echo "Quick start (improved version):"
	@echo "  make deps   # Check dependency installation"
	@echo "  make        # Build the improved game"
	@echo "  make run    # Run the game"
	@echo ""
	@echo "Other versions:"
	@echo "  make simple    # Build simplified version"
	@echo "  make full      # Build full version (needs SDL2_ttf)"
	@echo "  make full_fixed# Build full version with fixes (needs SDL2_ttf)"
	@echo "  make fixed     # Build fixed version (needs SDL2_ttf)"
	@echo "  make improved  # Build improved version (recommended)"
	@echo "  make debug     # Build debug version with font info"
	@echo ""
	@echo "Troubleshooting:"
	@echo "  make test_font # Check which fonts are available"
	@echo "  make test_text # Visual test of text rendering"
