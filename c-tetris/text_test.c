/*
 * Text Rendering Test for Tetris Font Issues
 * Minimal program to test if text renders as squares or correctly
 */

#include <SDL2/SDL.h>
#include <SDL2/SDL_ttf.h>
#include <stdio.h>
#include <stdlib.h>

int main(int argc, char* argv[]) {
    (void)argc;
    (void)argv;
    
    printf("Text Rendering Test\n");
    printf("===================\n");
    
    if (SDL_Init(SDL_INIT_VIDEO) < 0) {
        printf("SDL init failed: %s\n", SDL_GetError());
        return 1;
    }
    
    if (TTF_Init() < 0) {
        printf("TTF init failed: %s\n", TTF_GetError());
        SDL_Quit();
        return 1;
    }
    
    // Create window
    SDL_Window* window = SDL_CreateWindow("Text Rendering Test - Look for squares vs proper text",
        SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED,
        800, 600, SDL_WINDOW_SHOWN);
    
    if (!window) {
        printf("Window creation failed: %s\n", SDL_GetError());
        TTF_Quit();
        SDL_Quit();
        return 1;
    }
    
    SDL_Renderer* renderer = SDL_CreateRenderer(window, -1, SDL_RENDERER_ACCELERATED);
    if (!renderer) {
        printf("Renderer creation failed: %s\n", SDL_GetError());
        SDL_DestroyWindow(window);
        TTF_Quit();
        SDL_Quit();
        return 1;
    }
    
    // Load font
    TTF_Font* font = TTF_OpenFont("/System/Library/Fonts/Helvetica.ttc", 24);
    if (!font) {
        printf("Font loading failed: %s\n", TTF_GetError());
        SDL_DestroyRenderer(renderer);
        SDL_DestroyWindow(window);
        TTF_Quit();
        SDL_Quit();
        return 1;
    }
    
    printf("Font loaded successfully. Creating text surfaces...\n");
    
    // Test different rendering methods
    const char* test_text = "Score: 1234";
    SDL_Color white = {255, 255, 255, 255};
    
    printf("Testing TTF_RenderText_Solid...\n");
    SDL_Surface* surface1 = TTF_RenderText_Solid(font, test_text, white);
    
    printf("Testing TTF_RenderUTF8_Solid...\n");
    SDL_Surface* surface2 = TTF_RenderUTF8_Solid(font, test_text, white);
    
    printf("Testing TTF_RenderText_Blended...\n");
    SDL_Surface* surface3 = TTF_RenderText_Blended(font, test_text, white);
    
    // Event loop
    int running = 1;
    while (running) {
        SDL_Event e;
        while (SDL_PollEvent(&e)) {
            if (e.type == SDL_QUIT) {
                running = 0;
            } else if (e.type == SDL_KEYDOWN) {
                if (e.key.keysym.sym == SDLK_ESCAPE) {
                    running = 0;
                }
            }
        }
        
        // Clear screen
        SDL_SetRenderDrawColor(renderer, 40, 40, 60, 255);
        SDL_RenderClear(renderer);
        
        // Draw instruction text
        SDL_SetRenderDrawColor(renderer, 255, 255, 0, 255);
        SDL_Rect instruction_rect = {10, 10, 600, 30};
        SDL_RenderDrawRect(renderer, &instruction_rect);
        
        int y_pos = 50;
        
        // Render different methods
        if (surface1) {
            SDL_Texture* texture1 = SDL_CreateTextureFromSurface(renderer, surface1);
            if (texture1) {
                SDL_Rect rect1 = {50, y_pos, surface1->w, surface1->h};
                SDL_RenderCopy(renderer, texture1, NULL, &rect1);
                SDL_DestroyTexture(texture1);
                
                // Draw method label
                SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);
                for (int i = 0; i < 200; i += 4) {
                    SDL_RenderDrawPoint(renderer, 10 + i, y_pos + 10);
                }
            }
            y_pos += 60;
        }
        
        if (surface2) {
            SDL_Texture* texture2 = SDL_CreateTextureFromSurface(renderer, surface2);
            if (texture2) {
                SDL_Rect rect2 = {50, y_pos, surface2->w, surface2->h};
                SDL_RenderCopy(renderer, texture2, NULL, &rect2);
                SDL_DestroyTexture(texture2);
                
                // Draw method label
                SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);
                for (int i = 0; i < 200; i += 8) {
                    SDL_RenderDrawPoint(renderer, 10 + i, y_pos + 10);
                }
            }
            y_pos += 60;
        }
        
        if (surface3) {
            SDL_Texture* texture3 = SDL_CreateTextureFromSurface(renderer, surface3);
            if (texture3) {
                SDL_Rect rect3 = {50, y_pos, surface3->w, surface3->h};
                SDL_RenderCopy(renderer, texture3, NULL, &rect3);
                SDL_DestroyTexture(texture3);
                
                // Draw method label
                SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);
                for (int i = 0; i < 200; i += 12) {
                    SDL_RenderDrawPoint(renderer, 10 + i, y_pos + 10);
                }
            }
            y_pos += 60;
        }
        
        // Draw some reference squares to compare
        SDL_SetRenderDrawColor(renderer, 100, 100, 100, 255);
        for (int i = 0; i < 10; i++) {
            SDL_Rect square = {50 + i * 15, y_pos + 40, 12, 12};
            SDL_RenderFillRect(renderer, &square);
        }
        
        // Instructions
        SDL_SetRenderDrawColor(renderer, 200, 200, 0, 255);
        SDL_Rect info_rect = {10, 500, 780, 80};
        SDL_RenderDrawRect(renderer, &info_rect);
        
        SDL_RenderPresent(renderer);
        SDL_Delay(16);
    }
    
    // Cleanup
    if (surface1) SDL_FreeSurface(surface1);
    if (surface2) SDL_FreeSurface(surface2);
    if (surface3) SDL_FreeSurface(surface3);
    
    TTF_CloseFont(font);
    SDL_DestroyRenderer(renderer);
    SDL_DestroyWindow(window);
    TTF_Quit();
    SDL_Quit();
    
    printf("\nTest completed.\n");
    printf("Expected: You should see 'Score: 1234' rendered 3 times with different methods\n");
    printf("If you see squares instead of text, there's a font/encoding issue\n");
    printf("Gray squares at bottom are reference - text should NOT look like those\n");
    
    return 0;
}