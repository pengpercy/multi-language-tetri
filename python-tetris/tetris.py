#!/usr/bin/env python3
"""
Terios - A stylish Tetris game with掌机-like interface
Features:
- Start, pause, reset, and level adjustment
- Beautiful color scheme and animations
- Score system with cool effects
"""

import pygame
import random
import sys
import math

# Initialize pygame
pygame.init()

# Game constants
SCREEN_WIDTH = 800
SCREEN_HEIGHT = 700
GRID_SIZE = 30
GRID_WIDTH = 10
GRID_HEIGHT = 20
SIDEBAR_WIDTH = 200

# Colors (现代风格调色板)
COLORS = {
    'BACKGROUND': (15, 15, 25),      # 深蓝黑色背景
    'GRID_BG': (25, 30, 40),         # 深蓝灰网格背景
    'GRID_LINES': (45, 55, 70),      # 蓝灰网格线
    'TEXT': (255, 255, 255),         # 纯白文字
    'HIGHLIGHT': (100, 200, 255),    # 亮蓝高亮
    'BORDER': (80, 120, 160),        # 蓝色边框
    'GLOW': (200, 220, 255),         # 发光效果

    # Tetromino colors - 更鲜艳现代的配色
    'I': (0, 255, 255),      # 亮青色
    'O': (255, 215, 0),      # 金黄色
    'T': (138, 43, 226),     # 蓝紫色
    'S': (50, 205, 50),      # 青绿色
    'Z': (255, 69, 0),       # 橙红色
    'J': (30, 144, 255),     # 道奇蓝
    'L': (255, 165, 0),      # 橙色

    # 方块边框颜色 - 稍微深一点的同色系
    'I_BORDER': (0, 200, 200),
    'O_BORDER': (200, 170, 0),
    'T_BORDER': (100, 30, 180),
    'S_BORDER': (40, 160, 40),
    'Z_BORDER': (200, 50, 0),
    'J_BORDER': (20, 100, 200),
    'L_BORDER': (200, 130, 0),
}

# Tetromino shapes
SHAPES = {
    'I': [[1, 1, 1, 1]],
    'O': [[1, 1],
          [1, 1]],
    'T': [[0, 1, 0],
          [1, 1, 1]],
    'S': [[0, 1, 1],
          [1, 1, 0]],
    'Z': [[1, 1, 0],
          [0, 1, 1]],
    'J': [[1, 0, 0],
          [1, 1, 1]],
    'L': [[0, 0, 1],
          [1, 1, 1]]
}

class Tetromino:
    def __init__(self, x, y, shape):
        self.x = x
        self.y = y
        self.shape = shape
        self.color = COLORS[shape]
        self.rotation = 0

class TeriosGame:
    def __init__(self):
        self.screen = pygame.display.set_mode((SCREEN_WIDTH, SCREEN_HEIGHT))
        pygame.display.set_caption("Terios - 掌机俄罗斯方块")
        self.clock = pygame.time.Clock()

        # Initialize fonts with Chinese support
        # Try to use a specific Chinese font, fallback to default if not available
        try:
            self.font = pygame.font.Font("/System/Library/AssetsV2/com_apple_MobileAsset_Font7/3419f2a427639ad8c8e139149a287865a90fa17e.asset/AssetData/PingFang.ttc", 24)
            self.small_font = pygame.font.Font("/System/Library/AssetsV2/com_apple_MobileAsset_Font7/3419f2a427639ad8c8e139149a287865a90fa17e.asset/AssetData/PingFang.ttc", 18)
        except:
            try:
                self.font = pygame.font.Font("/System/Library/Fonts/STHeiti Medium.ttc", 24)
                self.small_font = pygame.font.Font("/System/Library/Fonts/STHeiti Medium.ttc", 18)
            except:
                self.font = pygame.font.Font(None, 24)
                self.small_font = pygame.font.Font(None, 18)

        # Game state
        self.grid = [[0 for _ in range(GRID_WIDTH)] for _ in range(GRID_HEIGHT)]
        self.current_piece = None
        self.next_piece = None
        self.game_over = False
        self.paused = True
        self.score = 0
        self.level = 1
        self.lines_cleared = 0
        self.fall_speed = 0.5  # seconds per grid cell
        self.fall_time = 0
        self.game_started = False  # Track if game has started

        # Input handling
        self.down_pressed = False
        self.down_hold_time = 0
        self.soft_drop_speed = 0.05  # Fast drop when holding down

        # Left/Right key handling for improved responsiveness
        self.left_pressed = False
        self.right_pressed = False
        self.horizontal_delay = 0.15  # Initial delay before repeat
        self.horizontal_repeat = 0.05  # Repeat interval
        self.left_hold_time = 0
        self.right_hold_time = 0

        # Animations
        self.clearing_lines = []
        self.animation_time = 0
        self.particle_effects = []

        # Initialize game
        self.spawn_piece()

    def spawn_piece(self):
        """Generate a new tetromino"""
        if not self.next_piece:
            shape = random.choice(list(SHAPES.keys()))
            self.next_piece = Tetromino(GRID_WIDTH // 2, 0, shape)

        self.current_piece = self.next_piece
        shape = random.choice(list(SHAPES.keys()))
        self.next_piece = Tetromino(GRID_WIDTH // 2, 0, shape)

        # Check if game is over
        if not self.is_valid_position(self.current_piece):
            self.game_over = True

    def get_rotated_shape(self, piece):
        """Get the rotated shape data based on current rotation"""
        shape_data = SHAPES[piece.shape]

        # Rotate the shape data based on rotation value
        for _ in range(piece.rotation % 4):
            # Rotate 90 degrees clockwise
            shape_data = list(zip(*shape_data[::-1]))
            shape_data = [list(row) for row in shape_data]

        return shape_data

    def is_valid_position(self, piece, x=None, y=None):
        """Check if the piece position is valid"""
        if x is None:
            x = piece.x
        if y is None:
            y = piece.y

        shape_data = self.get_rotated_shape(piece)
        for row_idx, row in enumerate(shape_data):
            for col_idx, cell in enumerate(row):
                if cell:
                    pos_x, pos_y = x + col_idx, y + row_idx
                    if (pos_x < 0 or pos_x >= GRID_WIDTH or
                        pos_y >= GRID_HEIGHT or
                        (pos_y >= 0 and self.grid[pos_y][pos_x])):
                        return False
        return True

    def rotate_piece(self):
        """Rotate the current piece"""
        if not self.current_piece or self.paused or self.game_over:
            return

        original_rotation = self.current_piece.rotation

        # Try to rotate
        self.current_piece.rotation = (self.current_piece.rotation + 1) % 4

        # If rotation is invalid, try wall kicks
        if not self.is_valid_position(self.current_piece):
            # Try moving left or right to make rotation valid (wall kick)
            for offset in [-1, 1, -2, 2]:
                if self.is_valid_position(self.current_piece, self.current_piece.x + offset, self.current_piece.y):
                    self.current_piece.x += offset
                    return

            # If wall kicks don't work, revert rotation
            self.current_piece.rotation = original_rotation

    def move_piece(self, dx, dy):
        """Move the current piece"""
        if not self.current_piece or self.paused or self.game_over:
            return

        if self.is_valid_position(self.current_piece, self.current_piece.x + dx, self.current_piece.y + dy):
            self.current_piece.x += dx
            self.current_piece.y += dy
            return True
        return False

    def get_drop_position(self):
        """Get the position where the current piece would land if dropped"""
        if not self.current_piece:
            return None

        # Start from current position and move down until collision
        test_y = self.current_piece.y
        while self.is_valid_position(self.current_piece, self.current_piece.x, test_y + 1):
            test_y += 1

        return test_y

    def drop_piece(self):
        """Drop the piece to the bottom"""
        if not self.current_piece or self.paused or self.game_over:
            return

        while self.move_piece(0, 1):
            self.score += 1
        self.lock_piece()

    def lock_piece(self):
        """Lock the piece in place and check for completed lines"""
        if not self.current_piece:
            return

        shape_data = self.get_rotated_shape(self.current_piece)
        for row_idx, row in enumerate(shape_data):
            for col_idx, cell in enumerate(row):
                if cell:
                    pos_x, pos_y = self.current_piece.x + col_idx, self.current_piece.y + row_idx
                    if 0 <= pos_y < GRID_HEIGHT and 0 <= pos_x < GRID_WIDTH:
                        self.grid[pos_y][pos_x] = self.current_piece.shape

        self.clear_lines()
        self.spawn_piece()

    def clear_lines(self):
        """Clear completed lines and update score"""
        lines_to_clear = []
        for row_idx in range(GRID_HEIGHT):
            if all(self.grid[row_idx]):
                lines_to_clear.append(row_idx)

        if not lines_to_clear:
            return

        # Add to clearing animation
        self.clearing_lines = lines_to_clear[:]
        self.animation_time = 0

        # Update score
        cleared = len(lines_to_clear)
        self.lines_cleared += cleared
        points = [100, 300, 500, 800]  # Points for 1, 2, 3, 4 lines
        self.score += points[min(cleared-1, 3)] * self.level

        # Update level (every 10 lines)
        self.level = self.lines_cleared // 10 + 1
        self.fall_speed = max(0.05, 0.5 - (self.level - 1) * 0.05)

        # Create particle effects for cool animation
        for _ in range(cleared * 20):
            self.particle_effects.append({
                'x': random.randint(0, GRID_WIDTH * GRID_SIZE),
                'y': random.randint(0, SCREEN_HEIGHT),
                'vx': random.uniform(-2, 2),
                'vy': random.uniform(-3, -1),
                'life': 30,
                'color': random.choice(list(COLORS.values())[3:])  # Use tetromino colors
            })

    def update_animations(self, dt):
        """Update all animation effects"""
        self.animation_time += dt

        # Update particle effects
        for particle in self.particle_effects[:]:
            particle['x'] += particle['vx']
            particle['y'] += particle['vy']
            particle['life'] -= 1
            if particle['life'] <= 0:
                self.particle_effects.remove(particle)

        # Handle line clearing animation
        if self.clearing_lines and self.animation_time > 0.3:  # 0.3 seconds for animation
            # Remove the lines
            for row_idx in sorted(self.clearing_lines, reverse=True):
                del self.grid[row_idx]
                self.grid.insert(0, [0 for _ in range(GRID_WIDTH)])
            self.clearing_lines = []

    def draw_grid(self):
        """Draw the game grid"""
        # Grid background
        grid_rect = pygame.Rect(
            (SCREEN_WIDTH - SIDEBAR_WIDTH) // 2 - GRID_WIDTH * GRID_SIZE // 2,
            50,
            GRID_WIDTH * GRID_SIZE,
            GRID_HEIGHT * GRID_SIZE
        )
        pygame.draw.rect(self.screen, COLORS['GRID_BG'], grid_rect)
        pygame.draw.rect(self.screen, COLORS['BORDER'], grid_rect, 3)

        # Grid lines
        for x in range(GRID_WIDTH + 1):
            pygame.draw.line(
                self.screen,
                COLORS['GRID_LINES'],
                (grid_rect.left + x * GRID_SIZE, grid_rect.top),
                (grid_rect.left + x * GRID_SIZE, grid_rect.bottom),
                1
            )
        for y in range(GRID_HEIGHT + 1):
            pygame.draw.line(
                self.screen,
                COLORS['GRID_LINES'],
                (grid_rect.left, grid_rect.top + y * GRID_SIZE),
                (grid_rect.right, grid_rect.top + y * GRID_SIZE),
                1
            )

        # Draw locked pieces
        for row_idx in range(GRID_HEIGHT):
            for col_idx in range(GRID_WIDTH):
                if self.grid[row_idx][col_idx]:
                    shape = self.grid[row_idx][col_idx]
                    color = COLORS[shape]
                    rect = pygame.Rect(
                        grid_rect.left + col_idx * GRID_SIZE + 1,
                        grid_rect.top + row_idx * GRID_SIZE + 1,
                        GRID_SIZE - 2,
                        GRID_SIZE - 2
                    )

                    # Draw with enhanced 3D effect and glow
                    pygame.draw.rect(self.screen, color, rect)
                    # 内部高光
                    highlight_rect = pygame.Rect(rect.x + 2, rect.y + 2, rect.width - 4, rect.height - 4)
                    highlight_color = (min(255, color[0] + 60), min(255, color[1] + 60), min(255, color[2] + 60))
                    pygame.draw.rect(self.screen, highlight_color, highlight_rect, 2)
                    # 外部边框
                    border_color = COLORS.get(f'{shape}_BORDER', (max(0, color[0] - 40), max(0, color[1] - 40), max(0, color[2] - 40)))
                    pygame.draw.rect(self.screen, border_color, rect, 2)

        # Draw current piece
        if self.current_piece and not self.game_over:
            shape_data = self.get_rotated_shape(self.current_piece)
            for row_idx, row in enumerate(shape_data):
                for col_idx, cell in enumerate(row):
                    if cell:
                        pos_x = self.current_piece.x + col_idx
                        pos_y = self.current_piece.y + row_idx
                        if 0 <= pos_y < GRID_HEIGHT and 0 <= pos_x < GRID_WIDTH:
                            color = COLORS[self.current_piece.shape]
                            rect = pygame.Rect(
                                grid_rect.left + pos_x * GRID_SIZE + 1,
                                grid_rect.top + pos_y * GRID_SIZE + 1,
                                GRID_SIZE - 2,
                                GRID_SIZE - 2
                            )
                            # Draw current piece with enhanced effects
                            pygame.draw.rect(self.screen, color, rect)
                            # 内部高光
                            highlight_rect = pygame.Rect(rect.x + 2, rect.y + 2, rect.width - 4, rect.height - 4)
                            highlight_color = (min(255, color[0] + 80), min(255, color[1] + 80), min(255, color[2] + 80))
                            pygame.draw.rect(self.screen, highlight_color, highlight_rect, 2)
                            # 外部边框
                            border_color = COLORS.get(f'{self.current_piece.shape}_BORDER', (max(0, color[0] - 40), max(0, color[1] - 40), max(0, color[2] - 40)))
                            pygame.draw.rect(self.screen, border_color, rect, 2)

        # Draw drop preview (ghost piece)
        if self.current_piece and not self.game_over and not self.paused:
            drop_y = self.get_drop_position()
            if drop_y is not None and drop_y > self.current_piece.y:
                shape_data = self.get_rotated_shape(self.current_piece)
                for row_idx, row in enumerate(shape_data):
                    for col_idx, cell in enumerate(row):
                        if cell:
                            pos_x = self.current_piece.x + col_idx
                            pos_y = drop_y + row_idx
                            if 0 <= pos_y < GRID_HEIGHT and 0 <= pos_x < GRID_WIDTH:
                                color = COLORS[self.current_piece.shape]
                                rect = pygame.Rect(
                                    grid_rect.left + pos_x * GRID_SIZE + 1,
                                    grid_rect.top + pos_y * GRID_SIZE + 1,
                                    GRID_SIZE - 2,
                                    GRID_SIZE - 2
                                )

                                # Draw frosted glass ghost piece - very subtle and transparent
                                ghost_surface = pygame.Surface((GRID_SIZE - 2, GRID_SIZE - 2))
                                ghost_surface.set_alpha(35)  # 轻微提高不透明度以确保可见性

                                # 使用原方块颜色的非常淡的版本，营造磨砂效果
                                ghost_color = (
                                    min(255, color[0] // 4 + 150),  # 淡化原色并加入白色
                                    min(255, color[1] // 4 + 150),
                                    min(255, color[2] // 4 + 150)
                                )
                                ghost_surface.fill(ghost_color)

                                # 绘制磨砂透明方块
                                self.screen.blit(ghost_surface, (rect.x, rect.y))

        # Draw line clearing animation
        for row_idx in self.clearing_lines:
            alpha = int(255 * abs(math.sin(self.animation_time * 10)))
            flash_surface = pygame.Surface((GRID_WIDTH * GRID_SIZE, GRID_SIZE))
            flash_surface.set_alpha(alpha)
            flash_surface.fill((255, 255, 255))
            self.screen.blit(
                flash_surface,
                (grid_rect.left, grid_rect.top + row_idx * GRID_SIZE)
            )

        # Draw particle effects
        for particle in self.particle_effects:
            pygame.draw.circle(
                self.screen,
                particle['color'],
                (int(particle['x']), int(particle['y'])),
                max(1, particle['life'] // 10)
            )

    def draw_sidebar(self):
        """Draw the game sidebar with information"""
        sidebar_rect = pygame.Rect(
            SCREEN_WIDTH - SIDEBAR_WIDTH,
            0,
            SIDEBAR_WIDTH,
            SCREEN_HEIGHT
        )
        # 渐变背景效果
        for i in range(SIDEBAR_WIDTH):
            alpha = i / SIDEBAR_WIDTH
            color = (
                int(COLORS['BACKGROUND'][0] + (COLORS['GRID_BG'][0] - COLORS['BACKGROUND'][0]) * alpha),
                int(COLORS['BACKGROUND'][1] + (COLORS['GRID_BG'][1] - COLORS['BACKGROUND'][1]) * alpha),
                int(COLORS['BACKGROUND'][2] + (COLORS['GRID_BG'][2] - COLORS['BACKGROUND'][2]) * alpha)
            )
            pygame.draw.line(self.screen, color,
                           (SCREEN_WIDTH - SIDEBAR_WIDTH + i, 0),
                           (SCREEN_WIDTH - SIDEBAR_WIDTH + i, SCREEN_HEIGHT))

        # 发光边框效果
        for width in range(5, 0, -1):
            alpha = (6 - width) * 40
            glow_color = (*COLORS['BORDER'], alpha) if len(COLORS['BORDER']) == 3 else COLORS['BORDER']
            pygame.draw.line(self.screen, glow_color,
                           (SCREEN_WIDTH - SIDEBAR_WIDTH - width, 0),
                           (SCREEN_WIDTH - SIDEBAR_WIDTH - width, SCREEN_HEIGHT), 1)

        # Draw game information
        y_offset = 50

        # Score
        score_text = self.font.render(f"得分: {self.score}", True, COLORS['TEXT'])
        self.screen.blit(score_text, (SCREEN_WIDTH - SIDEBAR_WIDTH + 20, y_offset))
        y_offset += 50

        # Level
        level_text = self.font.render(f"关卡: {self.level}", True, COLORS['TEXT'])
        self.screen.blit(level_text, (SCREEN_WIDTH - SIDEBAR_WIDTH + 20, y_offset))
        y_offset += 50

        # Lines cleared
        lines_text = self.font.render(f"消除: {self.lines_cleared}", True, COLORS['TEXT'])
        self.screen.blit(lines_text, (SCREEN_WIDTH - SIDEBAR_WIDTH + 20, y_offset))
        y_offset += 70

        # Next piece preview
        next_text = self.font.render("下一个:", True, COLORS['TEXT'])
        self.screen.blit(next_text, (SCREEN_WIDTH - SIDEBAR_WIDTH + 20, y_offset))
        y_offset += 40

        if self.next_piece:
            shape_data = SHAPES[self.next_piece.shape]
            preview_size = 20
            preview_offset_x = SCREEN_WIDTH - SIDEBAR_WIDTH + 50
            preview_offset_y = y_offset

            for row_idx, row in enumerate(shape_data):
                for col_idx, cell in enumerate(row):
                    if cell:
                        color = COLORS[self.next_piece.shape]
                        rect = pygame.Rect(
                            preview_offset_x + col_idx * preview_size,
                            preview_offset_y + row_idx * preview_size,
                            preview_size - 2,
                            preview_size - 2
                        )
                        pygame.draw.rect(self.screen, color, rect)

        y_offset += len(shape_data) * preview_size + 50

        # Controls
        controls_y = SCREEN_HEIGHT - 200
        controls = [
            "控制:",
            "← → : 移动",
            "↑ : 旋转",
            "↓ : 软降",
            "空格: 硬降",
            "P : 暂停",
            "R : 重置",
            "+/- : 关卡",
            "",
            "虚线显示落点位置"
        ]

        for i, text in enumerate(controls):
            ctrl_text = self.small_font.render(text, True, COLORS['TEXT'])
            self.screen.blit(ctrl_text, (SCREEN_WIDTH - SIDEBAR_WIDTH + 20, controls_y + i * 25))

    def draw_game_state(self):
        """Draw game state information (paused, game over, etc.)"""
        if self.paused and not self.game_over:
            # Draw pause overlay with animation
            overlay = pygame.Surface((SCREEN_WIDTH - SIDEBAR_WIDTH, SCREEN_HEIGHT))
            overlay.set_alpha(128)
            overlay.fill((0, 0, 0))
            self.screen.blit(overlay, (0, 0))

            pause_text = self.font.render("按 P 继续游戏", True, COLORS['HIGHLIGHT'])
            text_rect = pause_text.get_rect(center=((SCREEN_WIDTH - SIDEBAR_WIDTH) // 2, SCREEN_HEIGHT // 2))
            self.screen.blit(pause_text, text_rect)

        elif self.game_over:
            # Draw game over overlay with animation
            overlay = pygame.Surface((SCREEN_WIDTH - SIDEBAR_WIDTH, SCREEN_HEIGHT))
            overlay.set_alpha(180)
            overlay.fill((0, 0, 0))
            self.screen.blit(overlay, (0, 0))

            game_over_text = self.font.render("游戏结束", True, (240, 50, 50))
            text_rect = game_over_text.get_rect(center=((SCREEN_WIDTH - SIDEBAR_WIDTH) // 2, SCREEN_HEIGHT // 2 - 30))
            self.screen.blit(game_over_text, text_rect)

            score_text = self.small_font.render(f"最终得分: {self.score}", True, COLORS['TEXT'])
            score_rect = score_text.get_rect(center=((SCREEN_WIDTH - SIDEBAR_WIDTH) // 2, SCREEN_HEIGHT // 2 + 10))
            self.screen.blit(score_text, score_rect)

            restart_text = self.small_font.render("按 R 重新开始", True, COLORS['HIGHLIGHT'])
            restart_rect = restart_text.get_rect(center=((SCREEN_WIDTH - SIDEBAR_WIDTH) // 2, SCREEN_HEIGHT // 2 + 50))
            self.screen.blit(restart_text, restart_rect)

        elif not self.paused:
            # Draw "READY" animation at start
            if pygame.time.get_ticks() < 2000:  # First 2 seconds
                alpha = max(0, 255 - pygame.time.get_ticks() // 8)
                ready_surface = pygame.Surface((SCREEN_WIDTH - SIDEBAR_WIDTH, SCREEN_HEIGHT))
                ready_surface.set_alpha(alpha)
                ready_surface.fill((0, 0, 0))
                self.screen.blit(ready_surface, (0, 0))

                ready_text = self.font.render("READY", True, COLORS['HIGHLIGHT'])
                text_rect = ready_text.get_rect(center=((SCREEN_WIDTH - SIDEBAR_WIDTH) // 2, SCREEN_HEIGHT // 2))
                self.screen.blit(ready_text, text_rect)
            # Draw "Press P to start" if game is not started
            elif not self.game_started:
                start_text = self.small_font.render("按 P 开始游戏", True, COLORS['HIGHLIGHT'])
                text_rect = start_text.get_rect(center=((SCREEN_WIDTH - SIDEBAR_WIDTH) // 2, SCREEN_HEIGHT // 2 + 50))
                self.screen.blit(start_text, text_rect)

    def update(self, dt):
        """Update game state"""
        if self.paused or self.game_over:
            self.update_animations(dt)
            return

        # Handle continuous key presses
        if self.down_pressed:
            self.down_hold_time += dt
            current_speed = self.soft_drop_speed
        else:
            current_speed = self.fall_speed

        # Handle left/right continuous movement
        if self.left_pressed:
            self.left_hold_time += dt
            if self.left_hold_time >= self.horizontal_delay:
                # Start repeating movement
                if int((self.left_hold_time - self.horizontal_delay) / self.horizontal_repeat) > \
                   int((self.left_hold_time - self.horizontal_delay - dt) / self.horizontal_repeat):
                    self.move_piece(-1, 0)

        if self.right_pressed:
            self.right_hold_time += dt
            if self.right_hold_time >= self.horizontal_delay:
                # Start repeating movement
                if int((self.right_hold_time - self.horizontal_delay) / self.horizontal_repeat) > \
                   int((self.right_hold_time - self.horizontal_delay - dt) / self.horizontal_repeat):
                    self.move_piece(1, 0)

        # Update fall time
        self.fall_time += dt
        if self.fall_time >= current_speed:
            if not self.move_piece(0, 1):
                self.lock_piece()
            else:
                # Add score for soft drop only when down key is pressed
                if self.down_pressed:
                    self.score += 1
            self.fall_time = 0

        # Update animations
        self.update_animations(dt)

    def handle_input(self, event):
        """Handle user input"""
        if event.type == pygame.KEYDOWN:
            if event.key == pygame.K_p:  # Pause
                if not self.game_over:
                    self.paused = not self.paused
                    if not self.paused:
                        self.game_started = True  # Mark game as started when unpausing
            elif event.key == pygame.K_r:  # Reset
                self.__init__()  # Reinitialize the game
            elif event.key == pygame.K_PLUS or event.key == pygame.K_EQUALS:  # Increase level
                self.level = min(20, self.level + 1)
                self.fall_speed = max(0.05, 0.5 - (self.level - 1) * 0.05)
            elif event.key == pygame.K_MINUS:  # Decrease level
                self.level = max(1, self.level - 1)
                self.fall_speed = max(0.05, 0.5 - (self.level - 1) * 0.05)

            # Allow controls only when game has started, not game over, and not paused
            if self.game_started and not self.game_over and not self.paused:
                if event.key == pygame.K_LEFT:
                    self.move_piece(-1, 0)  # Immediate move
                    self.left_pressed = True
                    self.left_hold_time = 0
                elif event.key == pygame.K_RIGHT:
                    self.move_piece(1, 0)  # Immediate move
                    self.right_pressed = True
                    self.right_hold_time = 0
                elif event.key == pygame.K_DOWN:
                    self.down_pressed = True
                    self.down_hold_time = 0
                elif event.key == pygame.K_UP:
                    self.rotate_piece()
                elif event.key == pygame.K_SPACE:
                    self.drop_piece()

        elif event.type == pygame.KEYUP:
            # Handle key release events
            if event.key == pygame.K_DOWN:
                self.down_pressed = False
                self.down_hold_time = 0
            elif event.key == pygame.K_LEFT:
                self.left_pressed = False
                self.left_hold_time = 0
            elif event.key == pygame.K_RIGHT:
                self.right_pressed = False
                self.right_hold_time = 0

    def draw(self):
        """Draw everything"""
        self.screen.fill(COLORS['BACKGROUND'])
        self.draw_grid()
        self.draw_sidebar()
        self.draw_game_state()
        pygame.display.flip()

    def run(self):
        """Main game loop"""
        running = True
        while running:
            dt = self.clock.tick(60) / 1000.0  # Delta time in seconds

            # Handle events
            for event in pygame.event.get():
                if event.type == pygame.QUIT:
                    running = False
                else:
                    self.handle_input(event)

            self.update(dt)
            self.draw()

        pygame.quit()
        sys.exit()

if __name__ == "__main__":
    game = TeriosGame()
    game.run()
