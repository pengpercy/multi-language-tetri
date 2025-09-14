using SDL2;
using static SDL2.SDL;

using System.Runtime.InteropServices;

namespace TetrisSharp;

/// <summary>
/// 游戏主类，实现IDisposable以确保正确释放SDL资源
/// </summary>
public sealed class Game : IDisposable
{
    private IntPtr _window;
    private IntPtr _renderer;
    private FontManager? _fontManager;
    private bool _isDisposed;
    private bool _isRunning;
    private readonly GameState _state;

    public Game()
    {
        _state = new GameState();
        Initialize();
    }

    private void Initialize()
    {
        // 创建窗口和渲染器
        _window = SDL_CreateWindow(
            "Tetris - 俄罗斯方块",
            SDL_WINDOWPOS_CENTERED,
            SDL_WINDOWPOS_CENTERED,
            GameState.SCREEN_WIDTH,
            GameState.SCREEN_HEIGHT,
            SDL_WindowFlags.SDL_WINDOW_SHOWN
        );

        if (_window == IntPtr.Zero)
        {
            throw new Exception($"Window创建失败: {SDL_GetError()}");
        }

        _renderer = SDL_CreateRenderer(
            _window,
            -1,
            SDL_RendererFlags.SDL_RENDERER_ACCELERATED
        );

        if (_renderer == IntPtr.Zero)
        {
            throw new Exception($"Renderer创建失败: {SDL_GetError()}");
        }

        // 初始化字体管理器
        _fontManager = new FontManager(_renderer);

        // 初始化游戏
        SpawnPiece();
    }

    public unsafe void Run()
    {
        _isRunning = true;
        var lastTime = SDL_GetTicks();

        while (_isRunning)
        {
            // 计算时间增量
            var currentTime = SDL_GetTicks();
            var deltaTime = (currentTime - lastTime) / 1000.0f;
            lastTime = currentTime;

            // 处理事件
            while (SDL_PollEvent(out SDL_Event evt) != 0)
            {
                switch (evt.type)
                {
                    case SDL_EventType.SDL_QUIT:
                        _isRunning = false;
                        break;
                    default:
                        HandleInput(ref evt);
                        break;
                }
            }

            // 更新游戏状态
            Update(deltaTime);

            // 渲染
            Render();

            // 限制帧率
            SDL_Delay(16); // ~60 FPS
        }
    }

    /// <summary>
    /// 初始化一个新的方块
    /// </summary>
    private void SpawnPiece()
    {
        if (_state.NextPiece.Shape == 0)
        {
            _state.NextPiece.Shape = Random.Shared.Next(7) + 1;
            _state.NextPiece.X = GameState.GRID_WIDTH / 2 - 2;
            _state.NextPiece.Y = 0;
            _state.NextPiece.Rotation = 0;
        }

        _state.CurrentPiece = _state.NextPiece;
        _state.NextPiece.Shape = Random.Shared.Next(7) + 1;
        _state.NextPiece.X = GameState.GRID_WIDTH / 2 - 2;
        _state.NextPiece.Y = 0;
        _state.NextPiece.Rotation = 0;

        if (!IsValidPosition(_state.CurrentPiece, _state.CurrentPiece.X, _state.CurrentPiece.Y))
        {
            _state.IsGameOver = true;
        }
    }

    /// <summary>
    /// 检查指定位置是否有效（无碰撞）
    /// </summary>
    private bool IsValidPosition(Tetromino piece, int x, int y)
    {
        int shape = piece.Shape - 1;
        int rotation = piece.Rotation % 4;

        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                if (Tetromino.Shapes[shape, rotation, row, col] != 0)
                {
                    int posX = x + col;
                    int posY = y + row;

                    if (posX < 0 || posX >= GameState.GRID_WIDTH ||
                        posY >= GameState.GRID_HEIGHT ||
                        (posY >= 0 && _state.Grid[posY, posX] != 0))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 旋转当前方块
    /// </summary>
    private void RotatePiece()
    {
        if (_state.CurrentPiece.Shape == 0 || _state.IsPaused || _state.IsGameOver)
            return;

        int originalRotation = _state.CurrentPiece.Rotation;
        _state.CurrentPiece.Rotation = (_state.CurrentPiece.Rotation + 1) % 4;

        if (!IsValidPosition(_state.CurrentPiece, _state.CurrentPiece.X, _state.CurrentPiece.Y))
        {
            // 尝试墙踢
            int[] offsets = { -1, 1, -2, 2 };
            bool kicked = false;

            foreach (int offset in offsets)
            {
                if (IsValidPosition(_state.CurrentPiece, _state.CurrentPiece.X + offset, _state.CurrentPiece.Y))
                {
                    _state.CurrentPiece.X += offset;
                    kicked = true;
                    break;
                }
            }

            if (!kicked)
            {
                _state.CurrentPiece.Rotation = originalRotation;
            }
        }
    }

    /// <summary>
    /// 移动当前方块
    /// </summary>
    private bool MovePiece(int dx, int dy)
    {
        if (_state.CurrentPiece.Shape == 0 || _state.IsPaused || _state.IsGameOver)
            return false;

        if (IsValidPosition(_state.CurrentPiece, _state.CurrentPiece.X + dx, _state.CurrentPiece.Y + dy))
        {
            _state.CurrentPiece.X += dx;
            _state.CurrentPiece.Y += dy;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 快速下落当前方块
    /// </summary>
    private void HardDrop()
    {
        if (_state.CurrentPiece.Shape == 0 || _state.IsPaused || _state.IsGameOver)
            return;

        while (MovePiece(0, 1))
        {
            _state.Score += 1;
        }
        LockPiece();
    }

    /// <summary>
    /// 锁定当前方块到游戏网格
    /// </summary>
    private void LockPiece()
    {
        if (_state.CurrentPiece.Shape == 0)
            return;

        int shape = _state.CurrentPiece.Shape - 1;
        int rotation = _state.CurrentPiece.Rotation % 4;

        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                if (Tetromino.Shapes[shape, rotation, row, col] != 0)
                {
                    int posX = _state.CurrentPiece.X + col;
                    int posY = _state.CurrentPiece.Y + row;

                    if (posY >= 0 && posY < GameState.GRID_HEIGHT &&
                        posX >= 0 && posX < GameState.GRID_WIDTH)
                    {
                        _state.Grid[posY, posX] = _state.CurrentPiece.Shape;
                    }
                }
            }
        }

        ClearLines();
        SpawnPiece();
    }

    /// <summary>
    /// 检测并清除填满的行
    /// </summary>
    private void ClearLines()
    {
        _state.NumClearing = 0;

        // 寻找满行
        for (int row = 0; row < GameState.GRID_HEIGHT; row++)
        {
            bool full = true;
            for (int col = 0; col < GameState.GRID_WIDTH; col++)
            {
                if (_state.Grid[row, col] == 0)
                {
                    full = false;
                    break;
                }
            }
            if (full)
            {
                _state.ClearingLines[_state.NumClearing++] = row;
            }
        }

        if (_state.NumClearing > 0)
        {
            _state.AnimationTime = 0;

            // 更新分数
            int[] points = { 100, 300, 500, 800 };
            _state.Score += points[_state.NumClearing - 1] * _state.Level;
            _state.LinesCleared += _state.NumClearing;

            // 更新等级
            _state.Level = _state.LinesCleared / 10 + 1;
            _state.FallSpeed = Math.Max(0.05f, 0.5f - (_state.Level - 1) * 0.05f);

            // 添加粒子效果
            CreateClearAnimation();
        }
    }

    /// <summary>
    /// 执行消行动画
    /// </summary>
    private void CreateClearAnimation()
    {
        if (_state.NumClearing == 0)
            return;

        var random = Random.Shared;
        for (int i = 0; i < _state.NumClearing; i++)
        {
            int row = _state.ClearingLines[i];
            for (int col = 0; col < GameState.GRID_WIDTH; col++)
            {
                int pieceType = _state.Grid[row, col];
                if (pieceType == 0)
                    continue;

                // 为每个方块创建多个粒子
                for (int p = 0; p < 5; p++)
                {
                    var particle = new Particle
                    {
                        X = col * GameState.GRID_SIZE + GameState.GRID_SIZE / 2,
                        Y = row * GameState.GRID_SIZE + GameState.GRID_SIZE / 2,
                        VelocityX = (random.NextSingle() - 0.5f) * 300,
                        VelocityY = (random.NextSingle() - 0.5f) * 300,
                        Life = random.Next(30, 60),
                        Color = GetPieceColor(pieceType)
                    };
                    _state.Particles.Add(particle);
                }
            }
        }

        // 等动画完成后再移除行
        RemoveLines();
    }

    /// <summary>
    /// 获取指定类型方块的颜色
    /// </summary>
    private static Color GetPieceColor(int pieceType) => pieceType switch
    {
        1 => Color.Cyan,
        2 => Color.Gold,
        3 => Color.BlueViolet,
        4 => Color.LimeGreen,
        5 => Color.OrangeRed,
        6 => Color.DodgerBlue,
        7 => Color.Orange,
        _ => Color.Text
    };

    /// <summary>
    /// 移除已清除的行
    /// </summary>
    private void RemoveLines()
    {
        // 从最顶端开始，逐行下移
        for (int i = _state.NumClearing - 1; i >= 0; i--)
        {
            int row = _state.ClearingLines[i];
            for (int y = row; y > 0; y--)
            {
                for (int x = 0; x < GameState.GRID_WIDTH; x++)
                {
                    _state.Grid[y, x] = _state.Grid[y - 1, x];
                }
            }
            // 清空最顶行
            for (int x = 0; x < GameState.GRID_WIDTH; x++)
            {
                _state.Grid[0, x] = 0;
            }
        }

        _state.NumClearing = 0;
    }

    /// <summary>
    /// 计算当前方块的投影位置
    /// </summary>
    private int GetDropPosition()
    {
        int dropY = _state.CurrentPiece.Y;
        while (IsValidPosition(_state.CurrentPiece, _state.CurrentPiece.X, dropY + 1))
        {
            dropY++;
        }
        return dropY;
    }

    private void HandleInput(ref SDL_Event evt)
    {
        switch (evt.type)
        {
            case SDL_EventType.SDL_KEYDOWN:
                HandleKeyDown(evt.key.keysym.sym);
                break;
            case SDL_EventType.SDL_KEYUP:
                HandleKeyUp(evt.key.keysym.sym);
                break;
        }
    }

    private void HandleKeyDown(SDL_Keycode keycode)
    {
        if (!_state.IsGameStarted && keycode != SDL_Keycode.SDLK_p)
            return;

        switch (keycode)
        {
            case SDL_Keycode.SDLK_LEFT:
                _state.IsLeftPressed = true;
                _state.LeftHoldTime = 0;
                MovePiece(-1, 0);
                break;

            case SDL_Keycode.SDLK_RIGHT:
                _state.IsRightPressed = true;
                _state.RightHoldTime = 0;
                MovePiece(1, 0);
                break;

            case SDL_Keycode.SDLK_DOWN:
                _state.IsDownPressed = true;
                _state.DownHoldTime = 0;
                if (!MovePiece(0, 1))
                {
                    LockPiece();
                }
                break;

            case SDL_Keycode.SDLK_UP:
                RotatePiece();
                break;

            case SDL_Keycode.SDLK_SPACE:
                HardDrop();
                break;

            case SDL_Keycode.SDLK_p:
                if (!_state.IsGameStarted)
                {
                    _state.IsGameStarted = true;
                    _state.IsPaused = false;
                }
                else
                {
                    _state.IsPaused = !_state.IsPaused;
                }
                break;

            case SDL_Keycode.SDLK_r:
                if (_state.IsGameOver)
                {
                    // 重置游戏
                    Array.Clear(_state.Grid);
                    _state.Score = 0;
                    _state.Level = 1;
                    _state.LinesCleared = 0;
                    _state.FallSpeed = 0.5f;
                    _state.IsGameOver = false;
                    _state.IsPaused = true;
                    _state.IsGameStarted = false;
                    _state.Particles.Clear();
                    SpawnPiece();
                }
                break;

            case SDL_Keycode.SDLK_l:
            case SDL_Keycode.SDLK_UNKNOWN: // 对于某些键盘布局，L键可能被识别为UNKNOWN
                _state.SwitchLanguage();
                break;

            case SDL_Keycode.SDLK_KP_PLUS:
            case SDL_Keycode.SDLK_PLUS:
                if (_state.Level < 10)
                {
                    _state.Level++;
                    _state.FallSpeed = Math.Max(0.05f, 0.5f - (_state.Level - 1) * 0.05f);
                }
                break;

            case SDL_Keycode.SDLK_KP_MINUS:
            case SDL_Keycode.SDLK_MINUS:
                if (_state.Level > 1)
                {
                    _state.Level--;
                    _state.FallSpeed = Math.Max(0.05f, 0.5f - (_state.Level - 1) * 0.05f);
                }
                break;
        }
    }

    private void HandleKeyUp(SDL_Keycode keycode)
    {
        switch (keycode)
        {
            case SDL_Keycode.SDLK_LEFT:
                _state.IsLeftPressed = false;
                break;

            case SDL_Keycode.SDLK_RIGHT:
                _state.IsRightPressed = false;
                break;

            case SDL_Keycode.SDLK_DOWN:
                _state.IsDownPressed = false;
                break;
        }
    }

    private void Update(float deltaTime)
    {
        if (_state.IsGameOver || _state.IsPaused || !_state.IsGameStarted)
            return;

        // 更新自动下落
        _state.FallTime += deltaTime;
        if (_state.FallTime >= _state.FallSpeed)
        {
            _state.FallTime = 0;
            if (!MovePiece(0, 1))
            {
                LockPiece();
            }
        }

        // 更新持续按键
        if (_state.IsDownPressed)
        {
            _state.DownHoldTime += deltaTime;
            if (_state.DownHoldTime >= 0.05f)
            {
                _state.DownHoldTime = 0;
                if (!MovePiece(0, 1))
                {
                    LockPiece();
                }
            }
        }

        if (_state.IsLeftPressed)
        {
            _state.LeftHoldTime += deltaTime;
            if (_state.LeftHoldTime >= 0.15f)
            {
                _state.LeftHoldTime = 0;
                MovePiece(-1, 0);
            }
        }

        if (_state.IsRightPressed)
        {
            _state.RightHoldTime += deltaTime;
            if (_state.RightHoldTime >= 0.15f)
            {
                _state.RightHoldTime = 0;
                MovePiece(1, 0);
            }
        }

        // 更新粒子效果
        UpdateParticles(deltaTime);
    }

    /// <summary>
    /// 更新粒子效果
    /// </summary>
    private void UpdateParticles(float deltaTime)
    {
        for (int i = _state.Particles.Count - 1; i >= 0; i--)
        {
            var particle = _state.Particles[i];

            // 更新位置
            particle.X += particle.VelocityX * deltaTime;
            particle.Y += particle.VelocityY * deltaTime;
            particle.Life--;

            // 应用重力
            particle.VelocityY += 600f * deltaTime;

            if (particle.Life <= 0)
            {
                _state.Particles.RemoveAt(i);
            }
            else
            {
                _state.Particles[i] = particle;
            }
        }
    }

    private void Render()
    {
        // 清屏
        SDL_SetRenderDrawColor(_renderer, Color.Background.R, Color.Background.G, Color.Background.B, Color.Background.A);
        SDL_RenderClear(_renderer);

        // 绘制游戏区域
        DrawGameArea();

        // 绘制侧边栏
        DrawSidebar();

        // 绘制粒子效果
        DrawParticles();

        // 绘制游戏状态
        DrawGameState();

        // 刷新屏幕
        SDL_RenderPresent(_renderer);
    }

    private void DrawGameArea()
    {
        // 绘制背景网格
        SDL_SetRenderDrawColor(_renderer, Color.GridBackground.R, Color.GridBackground.G, Color.GridBackground.B, Color.GridBackground.A);
        var rect = new SDL_Rect
        {
            x = GameState.MARGIN,
            y = GameState.MARGIN,
            w = GameState.GRID_WIDTH * GameState.GRID_SIZE,
            h = GameState.GRID_HEIGHT * GameState.GRID_SIZE
        };
        SDL_RenderFillRect(_renderer, ref rect);

        // 绘制网格线
        SDL_SetRenderDrawColor(_renderer, Color.GridLines.R, Color.GridLines.G, Color.GridLines.B, Color.GridLines.A);
        for (int x = 0; x <= GameState.GRID_WIDTH; x++)
        {
            SDL_RenderDrawLine(_renderer,
                GameState.MARGIN + x * GameState.GRID_SIZE, GameState.MARGIN,
                GameState.MARGIN + x * GameState.GRID_SIZE, GameState.MARGIN + GameState.GRID_HEIGHT * GameState.GRID_SIZE);
        }
        for (int y = 0; y <= GameState.GRID_HEIGHT; y++)
        {
            SDL_RenderDrawLine(_renderer,
                GameState.MARGIN, GameState.MARGIN + y * GameState.GRID_SIZE,
                GameState.MARGIN + GameState.GRID_WIDTH * GameState.GRID_SIZE, GameState.MARGIN + y * GameState.GRID_SIZE);
        }

        // 绘制已放置的方块
        for (int y = 0; y < GameState.GRID_HEIGHT; y++)
        {
            for (int x = 0; x < GameState.GRID_WIDTH; x++)
            {
                if (_state.Grid[y, x] != 0)
                {
                    DrawBlock(x, y, _state.Grid[y, x], false);
                }
            }
        }

        // 绘制当前方块的投影
        if (_state.CurrentPiece.Shape != 0 && !_state.IsPaused && !_state.IsGameOver)
        {
            int dropY = GetDropPosition();
            DrawTetromino(_state.CurrentPiece, _state.CurrentPiece.X, dropY, true);
        }

        // 绘制当前方块
        if (_state.CurrentPiece.Shape != 0)
        {
            DrawTetromino(_state.CurrentPiece, _state.CurrentPiece.X, _state.CurrentPiece.Y, false);
        }
    }

    private void DrawSidebar()
    {
        int x = GameState.MARGIN + GameState.GRID_WIDTH * GameState.GRID_SIZE + 50;  // 增加侧边栏左边距
        int y = GameState.MARGIN + 30;  // 增加侧边栏顶部间距

        // 绘制分数
        _fontManager?.DrawText(_state.GetText(0, _state.Score), x, y, Color.Text, FontSize.Medium);
        y += 30;  // 减少间距

        // 绘制等级
        _fontManager?.DrawText(_state.GetText(1, _state.Level), x, y, Color.Text, FontSize.Medium);
        y += 30;  // 减少间距

        // 绘制消除行数
        _fontManager?.DrawText(_state.GetText(2, _state.LinesCleared), x, y, Color.Text, FontSize.Medium);
        y += 35;  // 减少间距，让下一个方块预览更靠上

        // 绘制下一个方块预览
        _fontManager?.DrawText(_state.GetText(3), x, y, Color.Text, FontSize.Medium);
        y += 25;  // 减少标题后间距
        // 预览方块位置：向左一个方格，向上一个方格
        DrawTetromino(_state.NextPiece, x / GameState.GRID_SIZE, (y - GameState.GRID_SIZE) / GameState.GRID_SIZE, false);

        // 计算操作说明从底部开始的位置，往上移动一个方块的距离
        int operationStartY = GameState.SCREEN_HEIGHT - GameState.MARGIN - (9 * 25) - 30; // 9行操作说明，每行25px，再往上30px

        // 绘制操作说明（从底部对齐）
        _fontManager?.DrawText(_state.GetText(4), x, operationStartY, Color.Highlight, FontSize.Medium);
        operationStartY += 25;
        _fontManager?.DrawText(_state.GetText(5), x, operationStartY, Color.Text, FontSize.Small);
        operationStartY += 25;
        _fontManager?.DrawText(_state.GetText(6), x, operationStartY, Color.Text, FontSize.Small);
        operationStartY += 25;
        _fontManager?.DrawText(_state.GetText(7), x, operationStartY, Color.Text, FontSize.Small);
        operationStartY += 25;
        _fontManager?.DrawText(_state.GetText(8), x, operationStartY, Color.Text, FontSize.Small);
        operationStartY += 25;
        _fontManager?.DrawText(_state.GetText(9), x, operationStartY, Color.Text, FontSize.Small);
        operationStartY += 25;
        _fontManager?.DrawText(_state.GetText(10), x, operationStartY, Color.Text, FontSize.Small);
        operationStartY += 25;
        _fontManager?.DrawText(_state.GetText(11), x, operationStartY, Color.Text, FontSize.Small);
        operationStartY += 25;
        _fontManager?.DrawText(_state.GetText(12), x, operationStartY, Color.Text, FontSize.Small);
        operationStartY += 25;
        _fontManager?.DrawText(_state.GetText(19), x, operationStartY, Color.Highlight, FontSize.Small);
    }

    private void DrawParticles()
    {
        foreach (var particle in _state.Particles)
        {
            var alpha = (byte)(255 * (particle.Life / 60.0f));
            SDL_SetRenderDrawColor(_renderer,
                particle.Color.R,
                particle.Color.G,
                particle.Color.B,
                alpha);

            var rect = new SDL_Rect
            {
                x = (int)particle.X - 2,
                y = (int)particle.Y - 2,
                w = 4,
                h = 4
            };
            SDL_RenderFillRect(_renderer, ref rect);
        }
    }

    private void DrawGameState()
    {
        if (_state.IsGameOver)
        {
            // 绘制游戏结束画面 - 在左侧游戏区域居中
            int gameAreaCenterX = GameState.MARGIN + (GameState.GRID_WIDTH * GameState.GRID_SIZE) / 2;
            int gameAreaCenterY = GameState.MARGIN + (GameState.GRID_HEIGHT * GameState.GRID_SIZE) / 2 - 50;

            _fontManager?.DrawText(_state.GetText(14), gameAreaCenterX, gameAreaCenterY, Color.OrangeRed, FontSize.Medium, TextAlignment.Center);
            _fontManager?.DrawText(_state.GetText(15, _state.Score), gameAreaCenterX, gameAreaCenterY + 40, Color.Text, FontSize.Small, TextAlignment.Center);
            _fontManager?.DrawText(_state.GetText(16), gameAreaCenterX, gameAreaCenterY + 70, Color.Highlight, FontSize.Small, TextAlignment.Center);
        }
        else if (_state.IsPaused)
        {
            // 绘制暂停画面 - 在左侧游戏区域居中
            int gameAreaCenterX = GameState.MARGIN + (GameState.GRID_WIDTH * GameState.GRID_SIZE) / 2;
            int gameAreaCenterY = GameState.MARGIN + (GameState.GRID_HEIGHT * GameState.GRID_SIZE) / 2 - 50;

            if (!_state.IsGameStarted)
            {
                _fontManager?.DrawText(_state.GetText(17), gameAreaCenterX, gameAreaCenterY, Color.Highlight, FontSize.Medium, TextAlignment.Center);
                _fontManager?.DrawText(_state.GetText(18), gameAreaCenterX, gameAreaCenterY + 40, Color.Text, FontSize.Small, TextAlignment.Center);
            }
            else
            {
                _fontManager?.DrawText(_state.GetText(13), gameAreaCenterX, gameAreaCenterY, Color.Text, FontSize.Medium, TextAlignment.Center);
            }
        }
    }

    private void DrawTetromino(Tetromino piece, int x, int y, bool isGhost)
    {
        if (piece.Shape == 0) return;

        int shape = piece.Shape - 1;
        int rotation = piece.Rotation % 4;

        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                if (Tetromino.Shapes[shape, rotation, row, col] != 0)
                {
                    int posX = x + col;
                    int posY = y + row;
                    if (posY >= 0) // 只绘制可见的部分
                    {
                        DrawBlock(posX, posY, piece.Shape, isGhost);
                    }
                }
            }
        }
    }

    private void DrawBlock(int x, int y, int type, bool isGhost)
    {
        var color = GetPieceColor(type);
        var borderColor = GetBorderColor(type);

        var rect = new SDL_Rect
        {
            x = GameState.MARGIN + x * GameState.GRID_SIZE + 1,
            y = GameState.MARGIN + y * GameState.GRID_SIZE + 1,
            w = GameState.GRID_SIZE - 2,
            h = GameState.GRID_SIZE - 2
        };

        if (isGhost)
        {
            // 绘制更淡的幽灵方块，不显示边框
            SDL_SetRenderDrawBlendMode(_renderer, SDL_BlendMode.SDL_BLENDMODE_BLEND);

            // 主体填充 - 更淡的颜色，去掉边框
            SDL_SetRenderDrawColor(_renderer, color.R, color.G, color.B, 15);
            SDL_RenderFillRect(_renderer, ref rect);

            // 添加微弱的内部纹理点缀，提升视觉层次
            SDL_SetRenderDrawColor(_renderer, color.R, color.G, color.B, 25);
            for (int i = 3; i < rect.w - 3; i += 6)
            {
                for (int j = 3; j < rect.h - 3; j += 6)
                {
                    var dot = new SDL_Rect { x = rect.x + i, y = rect.y + j, w = 1, h = 1 };
                    SDL_RenderFillRect(_renderer, ref dot);
                }
            }
        }
        else
        {
            // 绘制普通方块
            SDL_SetRenderDrawBlendMode(_renderer, SDL_BlendMode.SDL_BLENDMODE_NONE);
            SDL_SetRenderDrawColor(_renderer, color.R, color.G, color.B, color.A);
            SDL_RenderFillRect(_renderer, ref rect);

            // 绘制边框
            SDL_SetRenderDrawColor(_renderer, borderColor.R, borderColor.G, borderColor.B, borderColor.A);
            SDL_RenderDrawRect(_renderer, ref rect);
        }

        SDL_SetRenderDrawBlendMode(_renderer, SDL_BlendMode.SDL_BLENDMODE_NONE);
    }

    private Color GetBorderColor(int type) => type switch
    {
        1 => Color.CyanBorder,
        2 => Color.GoldBorder,
        3 => Color.BlueVioletBorder,
        4 => Color.LimeGreenBorder,
        5 => Color.OrangeRedBorder,
        6 => Color.DodgerBlueBorder,
        7 => Color.OrangeBorder,
        _ => Color.Text
    };

    public void Dispose()
    {
        if (_isDisposed) return;

        _fontManager?.Dispose();

        if (_renderer != IntPtr.Zero)
        {
            SDL_DestroyRenderer(_renderer);
            _renderer = IntPtr.Zero;
        }

        if (_window != IntPtr.Zero)
        {
            SDL_DestroyWindow(_window);
            _window = IntPtr.Zero;
        }

        _isDisposed = true;
    }
}
