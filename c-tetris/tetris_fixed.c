/*
 * Tetris in C with SDL2 (Fixed Version with Chinese Support)
 * A complete Tetris implementation with proper text rendering and animations
 * Features: 7 piece types, rotation, line clearing, scoring, levels, ghost piece
 * Fixed issues: text rendering, animation clearing, Chinese/English language support
 */

#include <SDL2/SDL.h>
#include <SDL2/SDL_ttf.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>
#include <math.h>
#include <stdarg.h>

// Game constants
#define SCREEN_WIDTH 800
#define SCREEN_HEIGHT 700
#define GRID_SIZE 30
#define GRID_WIDTH 10
#define GRID_HEIGHT 20
#define SIDEBAR_WIDTH 200

// Language support
#define MAX_LANGUAGES 2
#define MAX_STRINGS 20
#define MAX_STRING_LEN 64

typedef enum {
    LANG_ENGLISH = 0,
    LANG_CHINESE = 1
} Language;

// Text strings for different languages
char GAME_STRINGS[MAX_LANGUAGES][MAX_STRINGS][MAX_STRING_LEN] = {
    // English
    {
        "Score: %d",           // 0
        "Level: %d",           // 1  
        "Lines: %d",           // 2
        "Next:",               // 3
        "CONTROLS",            // 4
        "L/R: Move",           // 5
        "Up: Rotate",          // 6
        "Down: Drop",          // 7
        "Space: Hard Drop",    // 8
        "P: Pause",            // 9
        "R: Reset",            // 10
        "+/-: Level",          // 11
        "Ghost shows drop",    // 12
        "PRESS P TO CONTINUE", // 13
        "GAME OVER",           // 14
        "Final Score: %d",     // 15
        "PRESS R TO RESTART",  // 16
        "READY",               // 17
        "PRESS P TO START",    // 18
        "L: Switch Language"   // 19
    },
    // Chinese
    {
        "得分: %d",
        "关卡: %d", 
        "消除: %d",
        "下一个:",
        "操作",
        "左右: 移动",
        "上: 旋转", 
        "下: 软降",
        "空格: 硬降",
        "P: 暂停",
        "R: 重置",
        "+/-: 关卡",
        "虚线显示落点",
        "按 P 继续游戏",
        "游戏结束",
        "最终得分: %d",
        "按 R 重新开始", 
        "准备",
        "按 P 开始游戏",
        "L: 切换语言"
    }
};

// Colors
typedef struct {
    Uint8 r, g, b, a;
} Color;

// Color definitions
Color COLORS[] = {
    {15, 15, 25, 255},      // BACKGROUND
    {25, 30, 40, 255},      // GRID_BG
    {45, 55, 70, 255},      // GRID_LINES
    {255, 255, 255, 255},   // TEXT
    {100, 200, 255, 255},   // HIGHLIGHT
    {80, 120, 160, 255},    // BORDER
    {200, 220, 255, 255},   // GLOW
    
    // Tetromino colors
    {0, 255, 255, 255},     // I - Cyan
    {255, 215, 0, 255},     // O - Gold
    {138, 43, 226, 255},    // T - Blue Violet
    {50, 205, 50, 255},     // S - Lime Green
    {255, 69, 0, 255},      // Z - Orange Red
    {30, 144, 255, 255},    // J - Dodger Blue
    {255, 165, 0, 255},     // L - Orange
    
    // Border colors
    {0, 200, 200, 255},     // I_BORDER
    {200, 170, 0, 255},     // O_BORDER
    {100, 30, 180, 255},    // T_BORDER
    {40, 160, 40, 255},     // S_BORDER
    {200, 50, 0, 255},      // Z_BORDER
    {20, 100, 200, 255},    // J_BORDER
    {200, 130, 0, 255},     // L_BORDER
};

enum ColorIndex {
    COL_BACKGROUND, COL_GRID_BG, COL_GRID_LINES, COL_TEXT, COL_HIGHLIGHT, COL_BORDER, COL_GLOW,
    COL_I, COL_O, COL_T, COL_S, COL_Z, COL_J, COL_L,
    COL_I_BORDER, COL_O_BORDER, COL_T_BORDER, COL_S_BORDER, COL_Z_BORDER, COL_J_BORDER, COL_L_BORDER
};

// Tetromino shapes (4x4 grid, 0=empty, 1=filled)
int SHAPES[7][4][4][4] = {
    // I piece - 4 rotations
    {{{0,0,0,0},{1,1,1,1},{0,0,0,0},{0,0,0,0}},
     {{0,0,1,0},{0,0,1,0},{0,0,1,0},{0,0,1,0}},
     {{0,0,0,0},{0,0,0,0},{1,1,1,1},{0,0,0,0}},
     {{0,1,0,0},{0,1,0,0},{0,1,0,0},{0,1,0,0}}},
    
    // O piece - all rotations same
    {{{0,0,0,0},{0,1,1,0},{0,1,1,0},{0,0,0,0}},
     {{0,0,0,0},{0,1,1,0},{0,1,1,0},{0,0,0,0}},
     {{0,0,0,0},{0,1,1,0},{0,1,1,0},{0,0,0,0}},
     {{0,0,0,0},{0,1,1,0},{0,1,1,0},{0,0,0,0}}},
    
    // T piece
    {{{0,0,0,0},{0,1,0,0},{1,1,1,0},{0,0,0,0}},
     {{0,0,0,0},{0,1,0,0},{0,1,1,0},{0,1,0,0}},
     {{0,0,0,0},{0,0,0,0},{1,1,1,0},{0,1,0,0}},
     {{0,0,0,0},{0,1,0,0},{1,1,0,0},{0,1,0,0}}},
    
    // S piece
    {{{0,0,0,0},{0,1,1,0},{1,1,0,0},{0,0,0,0}},
     {{0,0,0,0},{0,1,0,0},{0,1,1,0},{0,0,1,0}},
     {{0,0,0,0},{0,0,0,0},{0,1,1,0},{1,1,0,0}},
     {{0,0,0,0},{1,0,0,0},{1,1,0,0},{0,1,0,0}}},
    
    // Z piece
    {{{0,0,0,0},{1,1,0,0},{0,1,1,0},{0,0,0,0}},
     {{0,0,0,0},{0,0,1,0},{0,1,1,0},{0,1,0,0}},
     {{0,0,0,0},{0,0,0,0},{1,1,0,0},{0,1,1,0}},
     {{0,0,0,0},{0,1,0,0},{1,1,0,0},{1,0,0,0}}},
    
    // J piece
    {{{0,0,0,0},{1,0,0,0},{1,1,1,0},{0,0,0,0}},
     {{0,0,0,0},{0,1,1,0},{0,1,0,0},{0,1,0,0}},
     {{0,0,0,0},{0,0,0,0},{1,1,1,0},{0,0,1,0}},
     {{0,0,0,0},{0,1,0,0},{0,1,0,0},{1,1,0,0}}},
    
    // L piece
    {{{0,0,0,0},{0,0,1,0},{1,1,1,0},{0,0,0,0}},
     {{0,0,0,0},{0,1,0,0},{0,1,0,0},{0,1,1,0}},
     {{0,0,0,0},{0,0,0,0},{1,1,1,0},{1,0,0,0}},
     {{0,0,0,0},{1,1,0,0},{0,1,0,0},{0,1,0,0}}}
};

// Tetromino structure
typedef struct {
    int x, y;
    int shape;
    int rotation;
} Tetromino;

// Particle effect structure
typedef struct {
    float x, y;
    float vx, vy;
    int life;
    Color color;
} Particle;

// Game state structure
typedef struct {
    SDL_Window *window;
    SDL_Renderer *renderer;
    TTF_Font *font;
    TTF_Font *small_font;
    
    int grid[GRID_HEIGHT][GRID_WIDTH];
    Tetromino current_piece;
    Tetromino next_piece;
    
    int game_over;
    int paused;
    int game_started;
    int score;
    int level;
    int lines_cleared;
    Language current_language;
    
    float fall_speed;
    float fall_time;
    
    // Input handling
    int down_pressed;
    int left_pressed;
    int right_pressed;
    float down_hold_time;
    float left_hold_time;
    float right_hold_time;
    
    // Animation
    int clearing_lines[4];
    int num_clearing;
    float animation_time;
    Particle particles[500];
    int num_particles;
    
    Uint32 start_time;
} GameState;

// Function prototypes
void init_game(GameState *game);
void cleanup_game(GameState *game);
void spawn_piece(GameState *game);
int is_valid_position(GameState *game, Tetromino *piece, int x, int y);
void rotate_piece(GameState *game);
int move_piece(GameState *game, int dx, int dy);
void drop_piece(GameState *game);
void lock_piece(GameState *game);
void clear_lines(GameState *game);
int get_drop_position(GameState *game);
void update_game(GameState *game, float dt);
void handle_input(GameState *game, SDL_Event *event);
void draw_game(GameState *game);
void draw_grid(GameState *game);
void draw_sidebar(GameState *game);
void draw_game_state(GameState *game);
void set_color(SDL_Renderer *renderer, Color color);
void draw_block(GameState *game, int x, int y, int shape, int ghost);
void draw_text(GameState *game, const char* text, int x, int y, Color color, TTF_Font* font);
char* get_text(GameState *game, int string_id, ...);
void switch_language(GameState *game);

int main(int argc, char *argv[]) {
    (void)argc; (void)argv; // Suppress unused parameter warnings
    
    if (SDL_Init(SDL_INIT_VIDEO) < 0) {
        printf("SDL init failed: %s\n", SDL_GetError());
        return 1;
    }
    
    if (TTF_Init() < 0) {
        printf("TTF init failed: %s\n", TTF_GetError());
        SDL_Quit();
        return 1;
    }
    
    srand((unsigned int)time(NULL));
    
    GameState game;
    init_game(&game);
    
    Uint32 last_time = SDL_GetTicks();
    int running = 1;
    
    while (running) {
        Uint32 current_time = SDL_GetTicks();
        float dt = (current_time - last_time) / 1000.0f;
        last_time = current_time;
        
        SDL_Event event;
        while (SDL_PollEvent(&event)) {
            if (event.type == SDL_QUIT) {
                running = 0;
            } else {
                handle_input(&game, &event);
            }
        }
        
        update_game(&game, dt);
        draw_game(&game);
        
        SDL_Delay(16); // ~60 FPS
    }
    
    cleanup_game(&game);
    return 0;
}

void init_game(GameState *game) {
    memset(game, 0, sizeof(GameState));
    
    // Create window and renderer
    game->window = SDL_CreateWindow("俄罗斯方块 - Tetris",
        SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED,
        SCREEN_WIDTH, SCREEN_HEIGHT, SDL_WINDOW_SHOWN);
    
    game->renderer = SDL_CreateRenderer(game->window, -1, SDL_RENDERER_ACCELERATED);
    
    // Initialize fonts with improved quality and better font choices
    const char* font_paths[] = {
        "/System/Library/Fonts/PingFang.ttc",           // macOS Modern Chinese
        "/System/Library/Fonts/Hiragino Sans GB.ttc",   // macOS Professional Chinese
        "/Library/Fonts/Microsoft/YaHei.ttc",           // Microsoft YaHei if installed
        "/System/Library/Fonts/STHeiti Light.ttc",      // macOS Light Chinese
        "/Library/Fonts/Arial Unicode.ttf",             // macOS Unicode
        "/usr/share/fonts/truetype/noto/NotoSansCJK-Regular.ttc", // Linux Chinese
        "/Windows/Fonts/msyh.ttc",                      // Windows Chinese
        "C:/Windows/Fonts/msyh.ttc",                    // Windows Chinese alt path
        NULL
    };
    
    game->font = NULL;
    game->small_font = NULL;
    
    // Try to load fonts with optimized sharp rendering settings
    for (int i = 0; font_paths[i] != NULL && (!game->font || !game->small_font); i++) {
        TTF_Font* test_font = TTF_OpenFont(font_paths[i], 20);
        if (test_font) {
            // Configure font for sharp rendering
            TTF_SetFontHinting(test_font, TTF_HINTING_MONO);
            TTF_SetFontKerning(test_font, 0);
            
            // Set style for crisp edges
            TTF_SetFontStyle(test_font, TTF_STYLE_NORMAL);
            TTF_SetFontOutline(test_font, 0);
            
            // Test rendering
            SDL_Color test_color = {255, 255, 255, 255};
            SDL_Surface* test_surface = TTF_RenderUTF8_Blended(test_font, "测试", test_color);
            
            if (test_surface) {
                if (!game->font) {
                    game->font = test_font;
                    game->small_font = TTF_OpenFont(font_paths[i], 16);
                    if (game->small_font) {
                        // Apply same sharp rendering settings to small font
                        TTF_SetFontHinting(game->small_font, TTF_HINTING_MONO);
                        TTF_SetFontKerning(game->small_font, 0);
                        TTF_SetFontStyle(game->small_font, TTF_STYLE_NORMAL);
                        TTF_SetFontOutline(game->small_font, 0);
                    }
                }
                SDL_FreeSurface(test_surface);
                if (game->font && game->small_font) break;
            } else {
                TTF_CloseFont(test_font);
            }
        }
    }
    
    if (!game->font) {
        printf("Warning: Could not load any fonts. Text may not display properly.\n");
    }
    
    // Initialize game state
    memset(game->grid, 0, sizeof(game->grid));
    game->paused = 1;
    game->level = 1;
    game->fall_speed = 0.5f;
    game->start_time = SDL_GetTicks();
    game->current_language = LANG_CHINESE; // Start with Chinese
    
    spawn_piece(game);
}

void cleanup_game(GameState *game) {
    if (game->font) TTF_CloseFont(game->font);
    if (game->small_font) TTF_CloseFont(game->small_font);
    if (game->renderer) SDL_DestroyRenderer(game->renderer);
    if (game->window) SDL_DestroyWindow(game->window);
    TTF_Quit();
    SDL_Quit();
}

void spawn_piece(GameState *game) {
    if (game->next_piece.shape == 0) {
        game->next_piece.shape = rand() % 7 + 1;
        game->next_piece.x = GRID_WIDTH / 2;
        game->next_piece.y = 0;
        game->next_piece.rotation = 0;
    }
    
    game->current_piece = game->next_piece;
    game->next_piece.shape = rand() % 7 + 1;
    game->next_piece.x = GRID_WIDTH / 2;
    game->next_piece.y = 0;
    game->next_piece.rotation = 0;
    
    if (!is_valid_position(game, &game->current_piece, game->current_piece.x, game->current_piece.y)) {
        game->game_over = 1;
    }
}

int is_valid_position(GameState *game, Tetromino *piece, int x, int y) {
    int shape = piece->shape - 1;
    int rotation = piece->rotation % 4;
    
    for (int row = 0; row < 4; row++) {
        for (int col = 0; col < 4; col++) {
            if (SHAPES[shape][rotation][row][col]) {
                int pos_x = x + col;
                int pos_y = y + row;
                
                if (pos_x < 0 || pos_x >= GRID_WIDTH || 
                    pos_y >= GRID_HEIGHT ||
                    (pos_y >= 0 && game->grid[pos_y][pos_x])) {
                    return 0;
                }
            }
        }
    }
    return 1;
}

void rotate_piece(GameState *game) {
    if (!game->current_piece.shape || game->paused || game->game_over) return;
    
    int original_rotation = game->current_piece.rotation;
    game->current_piece.rotation = (game->current_piece.rotation + 1) % 4;
    
    if (!is_valid_position(game, &game->current_piece, game->current_piece.x, game->current_piece.y)) {
        // Try wall kicks
        int offsets[] = {-1, 1, -2, 2};
        int kicked = 0;
        
        for (int i = 0; i < 4 && !kicked; i++) {
            if (is_valid_position(game, &game->current_piece, 
                                game->current_piece.x + offsets[i], 
                                game->current_piece.y)) {
                game->current_piece.x += offsets[i];
                kicked = 1;
            }
        }
        
        if (!kicked) {
            game->current_piece.rotation = original_rotation;
        }
    }
}

int move_piece(GameState *game, int dx, int dy) {
    if (!game->current_piece.shape || game->paused || game->game_over) return 0;
    
    if (is_valid_position(game, &game->current_piece, 
                         game->current_piece.x + dx, 
                         game->current_piece.y + dy)) {
        game->current_piece.x += dx;
        game->current_piece.y += dy;
        return 1;
    }
    return 0;
}

void drop_piece(GameState *game) {
    if (!game->current_piece.shape || game->paused || game->game_over) return;
    
    while (move_piece(game, 0, 1)) {
        game->score += 1;
    }
    lock_piece(game);
}

void lock_piece(GameState *game) {
    if (!game->current_piece.shape) return;
    
    int shape = game->current_piece.shape - 1;
    int rotation = game->current_piece.rotation % 4;
    
    for (int row = 0; row < 4; row++) {
        for (int col = 0; col < 4; col++) {
            if (SHAPES[shape][rotation][row][col]) {
                int pos_x = game->current_piece.x + col;
                int pos_y = game->current_piece.y + row;
                
                if (pos_y >= 0 && pos_y < GRID_HEIGHT && 
                    pos_x >= 0 && pos_x < GRID_WIDTH) {
                    game->grid[pos_y][pos_x] = game->current_piece.shape;
                }
            }
        }
    }
    
    clear_lines(game);
    spawn_piece(game);
}

void clear_lines(GameState *game) {
    game->num_clearing = 0;
    
    // Find full lines
    for (int row = 0; row < GRID_HEIGHT; row++) {
        int full = 1;
        for (int col = 0; col < GRID_WIDTH; col++) {
            if (!game->grid[row][col]) {
                full = 0;
                break;
            }
        }
        if (full) {
            game->clearing_lines[game->num_clearing++] = row;
        }
    }
    
    if (game->num_clearing > 0) {
        game->animation_time = 0;
        
        // Update score
        int points[] = {100, 300, 500, 800};
        game->score += points[game->num_clearing - 1] * game->level;
        game->lines_cleared += game->num_clearing;
        
        // Update level
        game->level = game->lines_cleared / 10 + 1;
        game->fall_speed = fmaxf(0.05f, 0.5f - (game->level - 1) * 0.05f);
        
        // Create particle effects
        for (int i = 0; i < game->num_clearing * 20 && game->num_particles < 500; i++) {
            Particle *p = &game->particles[game->num_particles++];
            p->x = (float)(rand() % (GRID_WIDTH * GRID_SIZE));
            p->y = (float)(rand() % SCREEN_HEIGHT);
            p->vx = (rand() % 400 - 200) / 100.0f;
            p->vy = (rand() % 200 + 100) / -100.0f;
            p->life = 30;
            p->color = COLORS[COL_I + rand() % 7];
        }
    }
}

int get_drop_position(GameState *game) {
    if (!game->current_piece.shape) return -1;
    
    int test_y = game->current_piece.y;
    while (is_valid_position(game, &game->current_piece, game->current_piece.x, test_y + 1)) {
        test_y++;
    }
    return test_y;
}

void update_game(GameState *game, float dt) {
    // Always update animation time and particles
    game->animation_time += dt;
    
    // Update particles
    for (int i = 0; i < game->num_particles; i++) {
        Particle *p = &game->particles[i];
        p->x += p->vx;
        p->y += p->vy;
        p->life--;
        
        if (p->life <= 0) {
            game->particles[i] = game->particles[--game->num_particles];
            i--;
        }
    }
    
    // Handle line clearing animation (FIXED: This should work even when paused)
    if (game->num_clearing > 0 && game->animation_time > 0.3f) {
        // Remove the lines
        for (int i = 0; i < game->num_clearing; i++) {
            int row = game->clearing_lines[i];
            for (int r = row; r > 0; r--) {
                for (int c = 0; c < GRID_WIDTH; c++) {
                    game->grid[r][c] = game->grid[r-1][c];
                }
            }
            for (int c = 0; c < GRID_WIDTH; c++) {
                game->grid[0][c] = 0;
            }
            
            // Adjust remaining clearing lines
            for (int j = i + 1; j < game->num_clearing; j++) {
                if (game->clearing_lines[j] < row) {
                    game->clearing_lines[j]++;
                }
            }
        }
        game->num_clearing = 0;
    }
    
    // Return early if paused or game over (but animations still update above)
    if (game->paused || game->game_over) {
        return;
    }
    
    // Handle continuous key presses
    float current_speed = game->down_pressed ? 0.05f : game->fall_speed;
    
    if (game->left_pressed) {
        game->left_hold_time += dt;
        if (game->left_hold_time >= 0.15f) {
            if (fmodf(game->left_hold_time - 0.15f, 0.05f) < dt) {
                move_piece(game, -1, 0);
            }
        }
    }
    
    if (game->right_pressed) {
        game->right_hold_time += dt;
        if (game->right_hold_time >= 0.15f) {
            if (fmodf(game->right_hold_time - 0.15f, 0.05f) < dt) {
                move_piece(game, 1, 0);
            }
        }
    }
    
    // Update fall time
    game->fall_time += dt;
    if (game->fall_time >= current_speed) {
        if (!move_piece(game, 0, 1)) {
            lock_piece(game);
        } else if (game->down_pressed) {
            game->score += 1;
        }
        game->fall_time = 0;
    }
}

void handle_input(GameState *game, SDL_Event *event) {
    if (event->type == SDL_KEYDOWN) {
        switch (event->key.keysym.sym) {
            case SDLK_p:
                if (!game->game_over) {
                    game->paused = !game->paused;
                    if (!game->paused) {
                        game->game_started = 1;
                    }
                }
                break;
                
            case SDLK_r:
                // Reset game
                memset(game->grid, 0, sizeof(game->grid));
                game->game_over = 0;
                game->paused = 1;
                game->game_started = 0;
                game->score = 0;
                game->level = 1;
                game->lines_cleared = 0;
                game->fall_speed = 0.5f;
                game->fall_time = 0;
                game->num_clearing = 0;
                game->num_particles = 0;
                game->start_time = SDL_GetTicks();
                spawn_piece(game);
                break;
                
            case SDLK_l:
                switch_language(game);
                break;
                
            case SDLK_PLUS:
            case SDLK_EQUALS:
                game->level = (game->level < 20) ? game->level + 1 : 20;
                game->fall_speed = fmaxf(0.05f, 0.5f - (game->level - 1) * 0.05f);
                break;
                
            case SDLK_MINUS:
                game->level = (game->level > 1) ? game->level - 1 : 1;
                game->fall_speed = fmaxf(0.05f, 0.5f - (game->level - 1) * 0.05f);
                break;
        }
        
        if (game->game_started && !game->game_over && !game->paused) {
            switch (event->key.keysym.sym) {
                case SDLK_LEFT:
                    move_piece(game, -1, 0);
                    game->left_pressed = 1;
                    game->left_hold_time = 0;
                    break;
                    
                case SDLK_RIGHT:
                    move_piece(game, 1, 0);
                    game->right_pressed = 1;
                    game->right_hold_time = 0;
                    break;
                    
                case SDLK_DOWN:
                    game->down_pressed = 1;
                    game->down_hold_time = 0;
                    break;
                    
                case SDLK_UP:
                    rotate_piece(game);
                    break;
                    
                case SDLK_SPACE:
                    drop_piece(game);
                    break;
            }
        }
    } else if (event->type == SDL_KEYUP) {
        switch (event->key.keysym.sym) {
            case SDLK_DOWN:
                game->down_pressed = 0;
                game->down_hold_time = 0;
                break;
                
            case SDLK_LEFT:
                game->left_pressed = 0;
                game->left_hold_time = 0;
                break;
                
            case SDLK_RIGHT:
                game->right_pressed = 0;
                game->right_hold_time = 0;
                break;
        }
    }
}

void set_color(SDL_Renderer *renderer, Color color) {
    SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);
}

void draw_block(GameState *game, int x, int y, int shape, int ghost) {
    int grid_start_x = (SCREEN_WIDTH - SIDEBAR_WIDTH) / 2 - GRID_WIDTH * GRID_SIZE / 2;
    int grid_start_y = 50;
    
    SDL_Rect rect = {
        grid_start_x + x * GRID_SIZE + 1,
        grid_start_y + y * GRID_SIZE + 1,
        GRID_SIZE - 2,
        GRID_SIZE - 2
    };
    
    if (ghost) {
        // Draw ghost piece with transparency effect
        Color color = COLORS[COL_I + shape - 1];
        color.r = color.r / 4 + 150;
        color.g = color.g / 4 + 150;
        color.b = color.b / 4 + 150;
        color.a = 35;
        
        SDL_SetRenderDrawBlendMode(game->renderer, SDL_BLENDMODE_BLEND);
        set_color(game->renderer, color);
        SDL_RenderFillRect(game->renderer, &rect);
        SDL_SetRenderDrawBlendMode(game->renderer, SDL_BLENDMODE_NONE);
    } else {
        // Draw solid block with 3D effect
        Color color = COLORS[COL_I + shape - 1];
        set_color(game->renderer, color);
        SDL_RenderFillRect(game->renderer, &rect);
        
        // Highlight
        SDL_Rect highlight = {rect.x + 2, rect.y + 2, rect.w - 4, rect.h - 4};
        Color highlight_color = {
            (Uint8)fminf(255, color.r + 60),
            (Uint8)fminf(255, color.g + 60),
            (Uint8)fminf(255, color.b + 60),
            255
        };
        set_color(game->renderer, highlight_color);
        SDL_RenderDrawRect(game->renderer, &highlight);
        SDL_RenderDrawRect(game->renderer, &highlight);
        
        // Border
        Color border_color = COLORS[COL_I_BORDER + shape - 1];
        set_color(game->renderer, border_color);
        SDL_RenderDrawRect(game->renderer, &rect);
        SDL_RenderDrawRect(game->renderer, &rect);
    }
}

void draw_text(GameState *game, const char* text, int x, int y, Color color, TTF_Font* font) {
    if (!font || !text) return;
    
    // Apply optimal font settings for sharp rendering
    TTF_SetFontHinting(font, TTF_HINTING_MONO);  // Use monospace hinting for sharper edges
    TTF_SetFontKerning(font, 0);                 // Disable kerning for more precise positioning
    
    SDL_Color sdl_color = {color.r, color.g, color.b, color.a};
    SDL_Surface* surface = NULL;
    
    #ifdef TTF_RENDER_UTF8_LCD  // Check if LCD rendering is available
        surface = TTF_RenderUTF8_LCD(font, text, sdl_color, COLORS[COL_BACKGROUND]);
    #endif
    
    if (!surface) {  // Fallback to high-quality Blended mode if LCD not available
        surface = TTF_RenderUTF8_Blended(font, text, sdl_color);
    }
    
    if (surface) {
        SDL_Texture* texture = SDL_CreateTextureFromSurface(game->renderer, surface);
        
        // Ensure texture uses nearest-neighbor scaling for sharp edges
        SDL_SetTextureScaleMode(texture, SDL_ScaleModeNearest);
        
        // Ensure precise pixel alignment
        SDL_Rect rect = {
            x,
            y,
            surface->w,
            surface->h
        };
        
        // Draw background first to ensure clean edges
        SDL_SetRenderDrawColor(game->renderer, 
                             COLORS[COL_BACKGROUND].r,
                             COLORS[COL_BACKGROUND].g,
                             COLORS[COL_BACKGROUND].b,
                             COLORS[COL_BACKGROUND].a);
        SDL_RenderFillRect(game->renderer, &rect);
        
        // Draw the text
        SDL_RenderCopy(game->renderer, texture, NULL, &rect);
        
        SDL_DestroyTexture(texture);
        SDL_FreeSurface(surface);
    }
}

char* get_text(GameState *game, int string_id, ...) {
    static char buffer[128];
    va_list args;
    va_start(args, string_id);
    vsnprintf(buffer, sizeof(buffer), GAME_STRINGS[game->current_language][string_id], args);
    va_end(args);
    return buffer;
}

void switch_language(GameState *game) {
    game->current_language = (game->current_language + 1) % MAX_LANGUAGES;
    
    // Update window title
    if (game->current_language == LANG_CHINESE) {
        SDL_SetWindowTitle(game->window, "俄罗斯方块");
    } else {
        SDL_SetWindowTitle(game->window, "Tetris");
    }
}

void draw_grid(GameState *game) {
    int grid_start_x = (SCREEN_WIDTH - SIDEBAR_WIDTH) / 2 - GRID_WIDTH * GRID_SIZE / 2;
    int grid_start_y = 50;
    
    // Grid background
    SDL_Rect grid_rect = {grid_start_x, grid_start_y, GRID_WIDTH * GRID_SIZE, GRID_HEIGHT * GRID_SIZE};
    set_color(game->renderer, COLORS[COL_GRID_BG]);
    SDL_RenderFillRect(game->renderer, &grid_rect);
    
    // Grid border
    set_color(game->renderer, COLORS[COL_BORDER]);
    for (int i = 0; i < 3; i++) {
        SDL_Rect border = {grid_rect.x - i, grid_rect.y - i, grid_rect.w + 2*i, grid_rect.h + 2*i};
        SDL_RenderDrawRect(game->renderer, &border);
    }
    
    // Grid lines
    set_color(game->renderer, COLORS[COL_GRID_LINES]);
    for (int x = 0; x <= GRID_WIDTH; x++) {
        SDL_RenderDrawLine(game->renderer,
                          grid_start_x + x * GRID_SIZE, grid_start_y,
                          grid_start_x + x * GRID_SIZE, grid_start_y + GRID_HEIGHT * GRID_SIZE);
    }
    for (int y = 0; y <= GRID_HEIGHT; y++) {
        SDL_RenderDrawLine(game->renderer,
                          grid_start_x, grid_start_y + y * GRID_SIZE,
                          grid_start_x + GRID_WIDTH * GRID_SIZE, grid_start_y + y * GRID_SIZE);
    }
    
    // Draw locked pieces
    for (int row = 0; row < GRID_HEIGHT; row++) {
        for (int col = 0; col < GRID_WIDTH; col++) {
            if (game->grid[row][col]) {
                draw_block(game, col, row, game->grid[row][col], 0);
            }
        }
    }
    
    // Draw current piece
    if (game->current_piece.shape && !game->game_over) {
        int shape = game->current_piece.shape - 1;
        int rotation = game->current_piece.rotation % 4;
        
        for (int row = 0; row < 4; row++) {
            for (int col = 0; col < 4; col++) {
                if (SHAPES[shape][rotation][row][col]) {
                    int pos_x = game->current_piece.x + col;
                    int pos_y = game->current_piece.y + row;
                    
                    if (pos_y >= 0 && pos_y < GRID_HEIGHT && pos_x >= 0 && pos_x < GRID_WIDTH) {
                        draw_block(game, pos_x, pos_y, game->current_piece.shape, 0);
                    }
                }
            }
        }
    }
    
    // Draw ghost piece
    if (game->current_piece.shape && !game->game_over && !game->paused) {
        int drop_y = get_drop_position(game);
        if (drop_y > game->current_piece.y) {
            int shape = game->current_piece.shape - 1;
            int rotation = game->current_piece.rotation % 4;
            
            for (int row = 0; row < 4; row++) {
                for (int col = 0; col < 4; col++) {
                    if (SHAPES[shape][rotation][row][col]) {
                        int pos_x = game->current_piece.x + col;
                        int pos_y = drop_y + row;
                        
                        if (pos_y >= 0 && pos_y < GRID_HEIGHT && pos_x >= 0 && pos_x < GRID_WIDTH) {
                            draw_block(game, pos_x, pos_y, game->current_piece.shape, 1);
                        }
                    }
                }
            }
        }
    }
    
    // Draw line clearing animation
    for (int i = 0; i < game->num_clearing; i++) {
        int alpha = (int)(255 * fabs(sin(game->animation_time * 10)));
        SDL_Rect flash_rect = {
            grid_start_x,
            grid_start_y + game->clearing_lines[i] * GRID_SIZE,
            GRID_WIDTH * GRID_SIZE,
            GRID_SIZE
        };
        
        SDL_SetRenderDrawBlendMode(game->renderer, SDL_BLENDMODE_BLEND);
        SDL_SetRenderDrawColor(game->renderer, 255, 255, 255, alpha);
        SDL_RenderFillRect(game->renderer, &flash_rect);
        SDL_SetRenderDrawBlendMode(game->renderer, SDL_BLENDMODE_NONE);
    }
    
    // Draw particle effects
    SDL_SetRenderDrawBlendMode(game->renderer, SDL_BLENDMODE_BLEND);
    for (int i = 0; i < game->num_particles; i++) {
        Particle *p = &game->particles[i];
        int radius = (p->life / 10) > 0 ? (p->life / 10) : 1;
        
        set_color(game->renderer, p->color);
        for (int dx = -radius; dx <= radius; dx++) {
            for (int dy = -radius; dy <= radius; dy++) {
                if (dx*dx + dy*dy <= radius*radius) {
                    SDL_RenderDrawPoint(game->renderer, (int)p->x + dx, (int)p->y + dy);
                }
            }
        }
    }
    SDL_SetRenderDrawBlendMode(game->renderer, SDL_BLENDMODE_NONE);
}

void draw_sidebar(GameState *game) {
    // Solid background matching the main background color
    SDL_Rect sidebar_bg = {
        SCREEN_WIDTH - SIDEBAR_WIDTH,
        0,
        SIDEBAR_WIDTH,
        SCREEN_HEIGHT
    };
    Color bg_color = COLORS[COL_BACKGROUND];
    SDL_SetRenderDrawColor(game->renderer, bg_color.r, bg_color.g, bg_color.b, bg_color.a);
    SDL_RenderFillRect(game->renderer, &sidebar_bg);
    
    // Glow border effect
    for (int width = 5; width > 0; width--) {
        Uint8 alpha = (6 - width) * 40;
        Color border_color = COLORS[COL_BORDER];
        SDL_SetRenderDrawBlendMode(game->renderer, SDL_BLENDMODE_BLEND);
        SDL_SetRenderDrawColor(game->renderer, border_color.r, border_color.g, border_color.b, alpha);
        SDL_RenderDrawLine(game->renderer,
                          SCREEN_WIDTH - SIDEBAR_WIDTH - width, 0,
                          SCREEN_WIDTH - SIDEBAR_WIDTH - width, SCREEN_HEIGHT);
        SDL_SetRenderDrawBlendMode(game->renderer, SDL_BLENDMODE_NONE);
    }
    
    int y_offset = 50;
    
    // Score
    draw_text(game, get_text(game, 0, game->score), SCREEN_WIDTH - SIDEBAR_WIDTH + 20, y_offset, COLORS[COL_TEXT], game->font);
    y_offset += 50;
    
    // Level
    draw_text(game, get_text(game, 1, game->level), SCREEN_WIDTH - SIDEBAR_WIDTH + 20, y_offset, COLORS[COL_TEXT], game->font);
    y_offset += 50;
    
    // Lines cleared
    draw_text(game, get_text(game, 2, game->lines_cleared), SCREEN_WIDTH - SIDEBAR_WIDTH + 20, y_offset, COLORS[COL_TEXT], game->font);
    y_offset += 70;
    
    // Next piece
    draw_text(game, get_text(game, 3), SCREEN_WIDTH - SIDEBAR_WIDTH + 20, y_offset, COLORS[COL_TEXT], game->font);
    y_offset += 40;
    
    // Draw next piece preview
    if (game->next_piece.shape) {
        int shape = game->next_piece.shape - 1;
        int preview_size = 20;
        int preview_offset_x = SCREEN_WIDTH - SIDEBAR_WIDTH + 50;
        int preview_offset_y = y_offset;
        
        for (int row = 0; row < 4; row++) {
            for (int col = 0; col < 4; col++) {
                if (SHAPES[shape][0][row][col]) {
                    SDL_Rect preview_rect = {
                        preview_offset_x + col * preview_size,
                        preview_offset_y + row * preview_size,
                        preview_size - 2,
                        preview_size - 2
                    };
                    
                    Color color = COLORS[COL_I + shape];
                    set_color(game->renderer, color);
                    SDL_RenderFillRect(game->renderer, &preview_rect);
                }
            }
        }
    }
    
    // Controls
    int controls_y = SCREEN_HEIGHT - 250;
    const int control_strings[] = {4, 5, 6, 7, 8, 9, 10, 11, 12, 19};
    
    for (int i = 0; i < 10; i++) {
        draw_text(game, get_text(game, control_strings[i]), 
                 SCREEN_WIDTH - SIDEBAR_WIDTH + 20, controls_y + i * 25, COLORS[COL_TEXT], game->small_font);
    }
}

void draw_game_state(GameState *game) {
    if (game->paused && !game->game_over) {
        // Pause overlay
        SDL_Rect overlay = {0, 0, SCREEN_WIDTH - SIDEBAR_WIDTH, SCREEN_HEIGHT};
        SDL_SetRenderDrawBlendMode(game->renderer, SDL_BLENDMODE_BLEND);
        SDL_SetRenderDrawColor(game->renderer, 0, 0, 0, 128);
        SDL_RenderFillRect(game->renderer, &overlay);
        SDL_SetRenderDrawBlendMode(game->renderer, SDL_BLENDMODE_NONE);
        
        draw_text(game, get_text(game, 13), (SCREEN_WIDTH - SIDEBAR_WIDTH - 200) / 2, SCREEN_HEIGHT / 2, COLORS[COL_HIGHLIGHT], game->font);
        
    } else if (game->game_over) {
        // Game over overlay
        SDL_Rect overlay = {0, 0, SCREEN_WIDTH - SIDEBAR_WIDTH, SCREEN_HEIGHT};
        SDL_SetRenderDrawBlendMode(game->renderer, SDL_BLENDMODE_BLEND);
        SDL_SetRenderDrawColor(game->renderer, 0, 0, 0, 180);
        SDL_RenderFillRect(game->renderer, &overlay);
        SDL_SetRenderDrawBlendMode(game->renderer, SDL_BLENDMODE_NONE);
        
        // Game over message
        draw_text(game, get_text(game, 14), (SCREEN_WIDTH - SIDEBAR_WIDTH - 160) / 2, SCREEN_HEIGHT / 2 - 40, (Color){240, 50, 50, 255}, game->font);
        
        // Final score
        draw_text(game, get_text(game, 15, game->score), (SCREEN_WIDTH - SIDEBAR_WIDTH - 150) / 2, SCREEN_HEIGHT / 2, COLORS[COL_TEXT], game->small_font);
        
        // Restart instruction
        draw_text(game, get_text(game, 16), (SCREEN_WIDTH - SIDEBAR_WIDTH - 180) / 2, SCREEN_HEIGHT / 2 + 40, COLORS[COL_HIGHLIGHT], game->small_font);
        
    } else if (!game->paused) {
        // Ready animation at start
        Uint32 current_time = SDL_GetTicks();
        if (current_time - game->start_time < 2000) {
            int alpha = 255 - (current_time - game->start_time) / 8;
            if (alpha > 0) {
                SDL_Rect overlay = {0, 0, SCREEN_WIDTH - SIDEBAR_WIDTH, SCREEN_HEIGHT};
                SDL_SetRenderDrawBlendMode(game->renderer, SDL_BLENDMODE_BLEND);
                SDL_SetRenderDrawColor(game->renderer, 0, 0, 0, alpha);
                SDL_RenderFillRect(game->renderer, &overlay);
                SDL_SetRenderDrawBlendMode(game->renderer, SDL_BLENDMODE_NONE);
                
                draw_text(game, get_text(game, 17), (SCREEN_WIDTH - SIDEBAR_WIDTH - 100) / 2, SCREEN_HEIGHT / 2, COLORS[COL_HIGHLIGHT], game->font);
            }
        } else if (!game->game_started) {
            // Start instruction
            draw_text(game, get_text(game, 18), (SCREEN_WIDTH - SIDEBAR_WIDTH - 180) / 2, SCREEN_HEIGHT / 2 + 50, COLORS[COL_HIGHLIGHT], game->small_font);
        }
    }
}

void draw_game(GameState *game) {
    set_color(game->renderer, COLORS[COL_BACKGROUND]);
    SDL_RenderClear(game->renderer);
    
    draw_grid(game);
    draw_sidebar(game);
    draw_game_state(game);
    
    SDL_RenderPresent(game->renderer);
}
