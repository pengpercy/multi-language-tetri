/*
 * Font Test Program for Tetris
 * Tests which fonts are available on the system and their text rendering capabilities
 */

#include <SDL2/SDL.h>
#include <SDL2/SDL_ttf.h>
#include <stdio.h>
#include <stdlib.h>

int main(int argc, char* argv[]) {
    (void)argc;
    (void)argv;
    
    printf("Font Test Program\n");
    printf("=================\n\n");
    
    if (SDL_Init(SDL_INIT_VIDEO) < 0) {
        printf("SDL init failed: %s\n", SDL_GetError());
        return 1;
    }
    
    if (TTF_Init() < 0) {
        printf("TTF init failed: %s\n", TTF_GetError());
        SDL_Quit();
        return 1;
    }
    
    // Test font paths
    const char* font_paths[] = {
        // macOS fonts
        "/System/Library/Fonts/PingFang.ttc",
        "/System/Library/Fonts/Helvetica.ttc",
        "/System/Library/Fonts/Arial Unicode.ttf",
        "/Library/Fonts/Arial.ttf",
        
        // Linux fonts
        "/usr/share/fonts/truetype/noto/NotoSansCJK-Regular.ttc",
        "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
        "/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf",
        "/usr/share/fonts/TTF/DejaVuSans.ttf",
        
        // Windows fonts
        "/Windows/Fonts/msyh.ttc",
        "/Windows/Fonts/arial.ttf",
        "C:/Windows/Fonts/msyh.ttc",
        "C:/Windows/Fonts/arial.ttf",
        
        NULL
    };
    
    printf("Testing font availability:\n");
    printf("--------------------------\n");
    
    TTF_Font* working_font = NULL;
    const char* working_font_path = NULL;
    
    for (int i = 0; font_paths[i] != NULL; i++) {
        printf("Testing: %s\n", font_paths[i]);
        
        TTF_Font* font = TTF_OpenFont(font_paths[i], 24);
        if (font) {
            printf("  ✓ SUCCESS: Font loaded successfully\n");
            
            if (!working_font) {
                working_font = font;
                working_font_path = font_paths[i];
                printf("  → Using this font for rendering test\n");
            } else {
                TTF_CloseFont(font);
            }
        } else {
            printf("  ✗ FAILED: %s\n", TTF_GetError());
        }
        printf("\n");
    }
    
    if (working_font) {
        printf("Font Rendering Test:\n");
        printf("--------------------\n");
        printf("Using font: %s\n\n", working_font_path);
        
        // Create a simple window for testing
        SDL_Window* window = SDL_CreateWindow("Font Test",
            SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED,
            800, 600, SDL_WINDOW_SHOWN);
        
        if (window) {
            SDL_Renderer* renderer = SDL_CreateRenderer(window, -1, SDL_RENDERER_ACCELERATED);
            if (renderer) {
                // Test strings
                const char* test_strings[] = {
                    "Score: 1234",
                    "Level: 5",
                    "Lines: 42",
                    "Hello World",
                    "TETRIS",
                    "俄罗斯方块",  // Russian Tetris in Chinese
                    "分数: 1234",   // Score in Chinese
                    NULL
                };
                
                printf("Rendering test strings:\n");
                
                for (int i = 0; test_strings[i] != NULL; i++) {
                    printf("Testing: \"%s\"\n", test_strings[i]);
                    
                    SDL_Color white = {255, 255, 255, 255};
                    SDL_Surface* surface = TTF_RenderUTF8_Solid(working_font, test_strings[i], white);
                    
                    if (surface) {
                        printf("  ✓ Rendered successfully (%dx%d pixels)\n", surface->w, surface->h);
                        
                        SDL_Texture* texture = SDL_CreateTextureFromSurface(renderer, surface);
                        if (texture) {
                            printf("  ✓ Texture created successfully\n");
                            SDL_DestroyTexture(texture);
                        } else {
                            printf("  ✗ Failed to create texture: %s\n", SDL_GetError());
                        }
                        
                        SDL_FreeSurface(surface);
                    } else {
                        printf("  ✗ Rendering failed: %s\n", TTF_GetError());
                    }
                    printf("\n");
                }
                
                // Visual test - display window for 3 seconds
                printf("Displaying visual test for 3 seconds...\n");
                
                SDL_SetRenderDrawColor(renderer, 20, 20, 40, 255);
                SDL_RenderClear(renderer);
                
                // Draw test text
                int y_offset = 50;
                for (int i = 0; test_strings[i] != NULL && i < 7; i++) {
                    SDL_Color color = {255, 255, 255, 255};
                    SDL_Surface* surface = TTF_RenderUTF8_Solid(working_font, test_strings[i], color);
                    
                    if (surface) {
                        SDL_Texture* texture = SDL_CreateTextureFromSurface(renderer, surface);
                        if (texture) {
                            SDL_Rect rect = {50, y_offset, surface->w, surface->h};
                            SDL_RenderCopy(renderer, texture, NULL, &rect);
                            SDL_DestroyTexture(texture);
                        }
                        SDL_FreeSurface(surface);
                    }
                    
                    y_offset += 60;
                }
                
                SDL_RenderPresent(renderer);
                SDL_Delay(3000);
                
                SDL_DestroyRenderer(renderer);
            } else {
                printf("Failed to create renderer: %s\n", SDL_GetError());
            }
            
            SDL_DestroyWindow(window);
        } else {
            printf("Failed to create window: %s\n", SDL_GetError());
        }
        
        TTF_CloseFont(working_font);
    } else {
        printf("No working fonts found!\n");
        printf("\nSuggested fixes:\n");
        printf("- macOS: Install system fonts or check /System/Library/Fonts/\n");
        printf("- Linux: Install font packages (e.g., sudo apt-get install fonts-dejavu-core)\n");
        printf("- Windows: Check C:/Windows/Fonts/ directory\n");
    }
    
    TTF_Quit();
    SDL_Quit();
    
    printf("\nFont test completed.\n");
    return 0;
}