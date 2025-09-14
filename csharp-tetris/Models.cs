using System.Runtime.InteropServices;

namespace TetrisSharp;

/// <summary>
/// 语言选项
/// </summary>
public enum Language
{
    English = 0,
    Chinese = 1
}

/// <summary>
/// 颜色定义，使用值类型以避免堆分配
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct Color
{
    public readonly byte R;
    public readonly byte G;
    public readonly byte B;
    public readonly byte A;

    public Color(byte r, byte g, byte b, byte a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    // 预定义颜色
    public static readonly Color Background = new(15, 15, 25, 255);
    public static readonly Color GridBackground = new(25, 30, 40, 255);
    public static readonly Color GridLines = new(45, 55, 70, 255);
    public static readonly Color Text = new(255, 255, 255, 255);
    public static readonly Color Highlight = new(100, 200, 255, 255);
    public static readonly Color Border = new(80, 120, 160, 255);
    public static readonly Color Glow = new(200, 220, 255, 255);

    // 方块颜色
    public static readonly Color Cyan = new(0, 255, 255, 255);      // I
    public static readonly Color Gold = new(255, 215, 0, 255);      // O
    public static readonly Color BlueViolet = new(138, 43, 226, 255); // T
    public static readonly Color LimeGreen = new(50, 205, 50, 255); // S
    public static readonly Color OrangeRed = new(255, 69, 0, 255);  // Z
    public static readonly Color DodgerBlue = new(30, 144, 255, 255); // J
    public static readonly Color Orange = new(255, 165, 0, 255);    // L

    // 边框颜色
    public static readonly Color CyanBorder = new(0, 200, 200, 255);        // I
    public static readonly Color GoldBorder = new(200, 170, 0, 255);        // O
    public static readonly Color BlueVioletBorder = new(100, 30, 180, 255); // T
    public static readonly Color LimeGreenBorder = new(40, 160, 40, 255);   // S
    public static readonly Color OrangeRedBorder = new(200, 50, 0, 255);    // Z
    public static readonly Color DodgerBlueBorder = new(20, 100, 200, 255); // J
    public static readonly Color OrangeBorder = new(200, 130, 0, 255);      // L
}

/// <summary>
/// 俄罗斯方块结构，使用值类型避免堆分配
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Tetromino
{
    public int X;
    public int Y;
    public int Shape;
    public int Rotation;

    // 定义所有形状的旋转状态
    public static readonly int[,,,] Shapes = new int[7, 4, 4, 4]
    {
        // I piece
        {
            {{0,0,0,0}, {1,1,1,1}, {0,0,0,0}, {0,0,0,0}},
            {{0,0,1,0}, {0,0,1,0}, {0,0,1,0}, {0,0,1,0}},
            {{0,0,0,0}, {0,0,0,0}, {1,1,1,1}, {0,0,0,0}},
            {{0,1,0,0}, {0,1,0,0}, {0,1,0,0}, {0,1,0,0}}
        },
        // O piece
        {
            {{0,0,0,0}, {0,1,1,0}, {0,1,1,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,1,1,0}, {0,1,1,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,1,1,0}, {0,1,1,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,1,1,0}, {0,1,1,0}, {0,0,0,0}}
        },
        // T piece
        {
            {{0,0,0,0}, {0,1,0,0}, {1,1,1,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,1,0,0}, {0,1,1,0}, {0,1,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {1,1,1,0}, {0,1,0,0}},
            {{0,0,0,0}, {0,1,0,0}, {1,1,0,0}, {0,1,0,0}}
        },
        // S piece
        {
            {{0,0,0,0}, {0,1,1,0}, {1,1,0,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,1,0,0}, {0,1,1,0}, {0,0,1,0}},
            {{0,0,0,0}, {0,0,0,0}, {0,1,1,0}, {1,1,0,0}},
            {{0,0,0,0}, {1,0,0,0}, {1,1,0,0}, {0,1,0,0}}
        },
        // Z piece
        {
            {{0,0,0,0}, {1,1,0,0}, {0,1,1,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,0,1,0}, {0,1,1,0}, {0,1,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {1,1,0,0}, {0,1,1,0}},
            {{0,0,0,0}, {0,1,0,0}, {1,1,0,0}, {1,0,0,0}}
        },
        // J piece
        {
            {{0,0,0,0}, {1,0,0,0}, {1,1,1,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,1,1,0}, {0,1,0,0}, {0,1,0,0}},
            {{0,0,0,0}, {0,0,0,0}, {1,1,1,0}, {0,0,1,0}},
            {{0,0,0,0}, {0,1,0,0}, {0,1,0,0}, {1,1,0,0}}
        },
        // L piece
        {
            {{0,0,0,0}, {0,0,1,0}, {1,1,1,0}, {0,0,0,0}},
            {{0,0,0,0}, {0,1,0,0}, {0,1,0,0}, {0,1,1,0}},
            {{0,0,0,0}, {0,0,0,0}, {1,1,1,0}, {1,0,0,0}},
            {{0,0,0,0}, {1,1,0,0}, {0,1,0,0}, {0,1,0,0}}
        }
    };
}

/// <summary>
/// 粒子特效结构，使用值类型以避免堆分配
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Particle
{
    public float X;
    public float Y;
    public float VelocityX;
    public float VelocityY;
    public int Life;
    public Color Color;
}

/// <summary>
/// 文本字体位图数据，使用只读字典优化性能
/// </summary>
public static class FontData
{
    private static readonly Dictionary<char, byte[]> CharacterData = new()
    {
        [' '] = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
        ['0'] = new byte[] { 0x1C, 0x22, 0x26, 0x2A, 0x32, 0x22, 0x1C },
        ['1'] = new byte[] { 0x08, 0x18, 0x08, 0x08, 0x08, 0x08, 0x1C },
        ['2'] = new byte[] { 0x1C, 0x22, 0x02, 0x1C, 0x20, 0x20, 0x3E },
        ['3'] = new byte[] { 0x3E, 0x02, 0x04, 0x0C, 0x02, 0x22, 0x1C },
        ['4'] = new byte[] { 0x04, 0x0C, 0x14, 0x24, 0x3E, 0x04, 0x04 },
        ['5'] = new byte[] { 0x3E, 0x20, 0x3C, 0x02, 0x02, 0x22, 0x1C },
        ['6'] = new byte[] { 0x0C, 0x10, 0x20, 0x3C, 0x22, 0x22, 0x1C },
        ['7'] = new byte[] { 0x3E, 0x02, 0x04, 0x08, 0x10, 0x10, 0x10 },
        ['8'] = new byte[] { 0x1C, 0x22, 0x22, 0x1C, 0x22, 0x22, 0x1C },
        ['9'] = new byte[] { 0x1C, 0x22, 0x22, 0x1E, 0x02, 0x04, 0x18 },
        ['A'] = new byte[] { 0x1C, 0x22, 0x22, 0x3E, 0x22, 0x22, 0x22 },
        ['B'] = new byte[] { 0x3C, 0x22, 0x22, 0x3C, 0x22, 0x22, 0x3C },
        ['C'] = new byte[] { 0x1C, 0x22, 0x20, 0x20, 0x20, 0x22, 0x1C },
        ['D'] = new byte[] { 0x38, 0x24, 0x22, 0x22, 0x22, 0x24, 0x38 },
        ['E'] = new byte[] { 0x3E, 0x20, 0x20, 0x3C, 0x20, 0x20, 0x3E },
        ['F'] = new byte[] { 0x3E, 0x20, 0x20, 0x3C, 0x20, 0x20, 0x20 },
        ['G'] = new byte[] { 0x1C, 0x22, 0x20, 0x2E, 0x22, 0x22, 0x1C },
        ['H'] = new byte[] { 0x22, 0x22, 0x22, 0x3E, 0x22, 0x22, 0x22 },
        ['I'] = new byte[] { 0x1C, 0x08, 0x08, 0x08, 0x08, 0x08, 0x1C },
        ['J'] = new byte[] { 0x0E, 0x04, 0x04, 0x04, 0x04, 0x24, 0x18 },
        ['K'] = new byte[] { 0x22, 0x24, 0x28, 0x30, 0x28, 0x24, 0x22 },
        ['L'] = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x3E },
        ['M'] = new byte[] { 0x22, 0x36, 0x2A, 0x2A, 0x22, 0x22, 0x22 },
        ['N'] = new byte[] { 0x22, 0x32, 0x2A, 0x26, 0x22, 0x22, 0x22 },
        ['O'] = new byte[] { 0x1C, 0x22, 0x22, 0x22, 0x22, 0x22, 0x1C },
        ['P'] = new byte[] { 0x3C, 0x22, 0x22, 0x3C, 0x20, 0x20, 0x20 },
        ['Q'] = new byte[] { 0x1C, 0x22, 0x22, 0x22, 0x2A, 0x24, 0x1A },
        ['R'] = new byte[] { 0x3C, 0x22, 0x22, 0x3C, 0x28, 0x24, 0x22 },
        ['S'] = new byte[] { 0x1C, 0x22, 0x20, 0x1C, 0x02, 0x22, 0x1C },
        ['T'] = new byte[] { 0x3E, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08 },
        ['U'] = new byte[] { 0x22, 0x22, 0x22, 0x22, 0x22, 0x22, 0x1C },
        ['V'] = new byte[] { 0x22, 0x22, 0x22, 0x22, 0x14, 0x14, 0x08 },
        ['W'] = new byte[] { 0x22, 0x22, 0x22, 0x2A, 0x2A, 0x36, 0x22 },
        ['X'] = new byte[] { 0x22, 0x14, 0x08, 0x08, 0x14, 0x22, 0x22 },
        ['Y'] = new byte[] { 0x22, 0x22, 0x14, 0x08, 0x08, 0x08, 0x08 },
        ['Z'] = new byte[] { 0x3E, 0x02, 0x04, 0x08, 0x10, 0x20, 0x3E },
        [':'] = new byte[] { 0x00, 0x18, 0x18, 0x00, 0x18, 0x18, 0x00 },
        ['/'] = new byte[] { 0x02, 0x04, 0x08, 0x08, 0x10, 0x20, 0x20 },
        ['-'] = new byte[] { 0x00, 0x00, 0x00, 0x3E, 0x00, 0x00, 0x00 },
        ['+'] = new byte[] { 0x00, 0x08, 0x08, 0x3E, 0x08, 0x08, 0x00 }
    };

    // Chinese character patterns (simplified representations)
    private static readonly Dictionary<char, byte[]> ChineseCharacterData = new()
    {
        ['分'] = new byte[] { 0x08, 0x08, 0x3E, 0x08, 0x08, 0x14, 0x22 }, // Score
        ['数'] = new byte[] { 0x3E, 0x02, 0x1C, 0x20, 0x1E, 0x22, 0x1C },
        ['：'] = new byte[] { 0x00, 0x18, 0x18, 0x00, 0x18, 0x18, 0x00 },
        ['级'] = new byte[] { 0x22, 0x22, 0x1C, 0x08, 0x1C, 0x22, 0x22 }, // Level
        ['别'] = new byte[] { 0x08, 0x14, 0x22, 0x3E, 0x22, 0x14, 0x08 },
        ['消'] = new byte[] { 0x3E, 0x08, 0x08, 0x3E, 0x08, 0x08, 0x3E }, // Clear
        ['除'] = new byte[] { 0x3C, 0x20, 0x20, 0x3E, 0x22, 0x22, 0x3E },
        ['行'] = new byte[] { 0x08, 0x08, 0x3E, 0x08, 0x08, 0x08, 0x08 }, // Lines
        ['下'] = new byte[] { 0x1C, 0x08, 0x08, 0x08, 0x08, 0x08, 0x3E }, // Next
        ['一'] = new byte[] { 0x00, 0x00, 0x00, 0x3E, 0x00, 0x00, 0x00 },
        ['个'] = new byte[] { 0x08, 0x08, 0x1C, 0x08, 0x08, 0x14, 0x22 },
        ['操'] = new byte[] { 0x22, 0x22, 0x3E, 0x22, 0x22, 0x22, 0x3E }, // Controls
        ['作'] = new byte[] { 0x08, 0x14, 0x22, 0x08, 0x14, 0x22, 0x08 },
        ['说'] = new byte[] { 0x3E, 0x22, 0x22, 0x3E, 0x22, 0x22, 0x3E },
        ['明'] = new byte[] { 0x1C, 0x22, 0x22, 0x22, 0x22, 0x22, 0x1C },
        ['左'] = new byte[] { 0x08, 0x18, 0x08, 0x08, 0x08, 0x08, 0x3E }, // Left/Right
        ['右'] = new byte[] { 0x3E, 0x08, 0x08, 0x08, 0x08, 0x18, 0x08 },
        ['移'] = new byte[] { 0x22, 0x14, 0x08, 0x3E, 0x08, 0x14, 0x22 }, // Move
        ['动'] = new byte[] { 0x1C, 0x22, 0x20, 0x1C, 0x02, 0x22, 0x1C },
        ['上'] = new byte[] { 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x1C }, // Up
        ['旋'] = new byte[] { 0x1C, 0x22, 0x02, 0x0C, 0x02, 0x22, 0x1C }, // Rotate
        ['转'] = new byte[] { 0x22, 0x22, 0x1C, 0x08, 0x1C, 0x22, 0x22 },
        ['落'] = new byte[] { 0x22, 0x22, 0x22, 0x3E, 0x08, 0x08, 0x08 }, // Drop
        ['空'] = new byte[] { 0x3E, 0x20, 0x20, 0x20, 0x20, 0x20, 0x3E }, // Space
        ['格'] = new byte[] { 0x3E, 0x22, 0x22, 0x22, 0x22, 0x22, 0x3E },
        ['快'] = new byte[] { 0x08, 0x08, 0x3E, 0x08, 0x3E, 0x08, 0x08 }, // Fast
        ['速'] = new byte[] { 0x22, 0x14, 0x08, 0x3E, 0x08, 0x14, 0x22 },
        ['暂'] = new byte[] { 0x1C, 0x22, 0x20, 0x3C, 0x20, 0x22, 0x1C }, // Pause
        ['停'] = new byte[] { 0x08, 0x08, 0x3E, 0x08, 0x08, 0x08, 0x3E },
        ['重'] = new byte[] { 0x1C, 0x22, 0x22, 0x1C, 0x22, 0x22, 0x1C }, // Reset
        ['置'] = new byte[] { 0x3E, 0x08, 0x08, 0x3E, 0x08, 0x08, 0x3E },
        ['调'] = new byte[] { 0x22, 0x22, 0x3E, 0x22, 0x3E, 0x22, 0x22 }, // Adjust
        ['整'] = new byte[] { 0x3E, 0x20, 0x3E, 0x02, 0x3E, 0x02, 0x3E },
        ['虚'] = new byte[] { 0x22, 0x14, 0x08, 0x08, 0x08, 0x14, 0x22 }, // Ghost
        ['影'] = new byte[] { 0x1C, 0x22, 0x20, 0x1C, 0x20, 0x22, 0x1C },
        ['显'] = new byte[] { 0x1C, 0x22, 0x22, 0x3E, 0x22, 0x22, 0x22 }, // Show
        ['示'] = new byte[] { 0x08, 0x08, 0x3E, 0x08, 0x08, 0x08, 0x08 },
        ['点'] = new byte[] { 0x08, 0x08, 0x08, 0x3E, 0x08, 0x08, 0x08 }, // Drop point
        ['按'] = new byte[] { 0x22, 0x22, 0x14, 0x08, 0x14, 0x22, 0x22 }, // Press
        ['继'] = new byte[] { 0x22, 0x22, 0x3E, 0x22, 0x22, 0x22, 0x3E }, // Continue
        ['续'] = new byte[] { 0x1C, 0x20, 0x18, 0x06, 0x02, 0x22, 0x1C },
        ['游'] = new byte[] { 0x3E, 0x02, 0x0C, 0x10, 0x0C, 0x02, 0x3E }, // Game
        ['戏'] = new byte[] { 0x08, 0x14, 0x22, 0x3E, 0x22, 0x14, 0x08 },
        ['结'] = new byte[] { 0x22, 0x22, 0x1C, 0x08, 0x1C, 0x22, 0x22 }, // End
        ['束'] = new byte[] { 0x08, 0x08, 0x3E, 0x08, 0x08, 0x08, 0x3E },
        ['最'] = new byte[] { 0x1C, 0x22, 0x22, 0x1C, 0x08, 0x08, 0x08 }, // Final
        ['终'] = new byte[] { 0x22, 0x22, 0x22, 0x3E, 0x08, 0x08, 0x08 },
        ['得'] = new byte[] { 0x08, 0x14, 0x22, 0x3E, 0x22, 0x22, 0x22 }, // Score
        ['新'] = new byte[] { 0x08, 0x08, 0x3E, 0x08, 0x3E, 0x08, 0x08 }, // New/Restart
        ['开'] = new byte[] { 0x3E, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08 }, // Start
        ['始'] = new byte[] { 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x3E },
        ['准'] = new byte[] { 0x1C, 0x22, 0x20, 0x1C, 0x20, 0x22, 0x1C }, // Ready
        ['备'] = new byte[] { 0x22, 0x22, 0x22, 0x3E, 0x22, 0x22, 0x22 },
        ['切'] = new byte[] { 0x08, 0x08, 0x3E, 0x08, 0x08, 0x08, 0x08 }, // Switch
        ['换'] = new byte[] { 0x22, 0x14, 0x08, 0x08, 0x08, 0x14, 0x22 },
        ['语'] = new byte[] { 0x3E, 0x22, 0x22, 0x3E, 0x22, 0x22, 0x3E }, // Language
        ['言'] = new byte[] { 0x1C, 0x08, 0x08, 0x3E, 0x08, 0x08, 0x1C }
    };

    public static byte[] GetCharacterData(char c)
    {
        // First try ASCII characters
        if (CharacterData.TryGetValue(c, out var data))
        {
            return data;
        }

        // Then try Chinese characters
        if (ChineseCharacterData.TryGetValue(c, out var chineseData))
        {
            return chineseData;
        }

        // Default to empty pattern for unknown characters
        return new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
    }
}

/// <summary>
/// 游戏状态类，使用只读字段优化性能
/// </summary>
public sealed class GameState
{
    // 游戏常量
    public const int GRID_WIDTH = 10;
    public const int GRID_HEIGHT = 20;
    public const int GRID_SIZE = 30;
    public const int SIDEBAR_WIDTH = 200;
    public const int MARGIN = 60; // 增加边距，改善整体留白
    public const int SCREEN_WIDTH = MARGIN + GRID_WIDTH * GRID_SIZE + SIDEBAR_WIDTH + MARGIN;
    public const int SCREEN_HEIGHT = MARGIN + GRID_HEIGHT * GRID_SIZE + MARGIN;

    // 游戏网格
    public readonly int[,] Grid = new int[GRID_HEIGHT, GRID_WIDTH];

    // 当前和下一个方块
    public Tetromino CurrentPiece;
    public Tetromino NextPiece;

    // 游戏状态
    public bool IsGameOver;
    public bool IsPaused;
    public bool IsGameStarted;
    public int Score;
    public int Level;
    public int LinesCleared;
    public Language CurrentLanguage;

    // 游戏控制参数
    public float FallSpeed;
    public float FallTime;
    public readonly long StartTime;

    // 输入状态
    public bool IsDownPressed;
    public bool IsLeftPressed;
    public bool IsRightPressed;
    public float DownHoldTime;
    public float LeftHoldTime;
    public float RightHoldTime;

    // 动画状态
    public readonly int[] ClearingLines = new int[4];
    public int NumClearing;
    public float AnimationTime;
    public readonly List<Particle> Particles = new();

    // 游戏文本资源
    private static readonly string[,] GameStrings = new string[2, 20]
    {
        // English
        {
            "Score: {0}",           // 0
            "Level: {0}",           // 1
            "Lines: {0}",           // 2
            "Next:",                // 3
            "CONTROLS",             // 4
            "L/R: Move",            // 5
            "Up: Rotate",           // 6
            "Down: Drop",           // 7
            "Space: Hard Drop",     // 8
            "P: Pause",             // 9
            "R: Reset",             // 10
            "+/-: Level",           // 11
            "Ghost shows drop",     // 12
            "PRESS P TO CONTINUE",  // 13
            "GAME OVER",            // 14
            "Final Score: {0}",     // 15
            "PRESS R TO RESTART",   // 16
            "READY",                // 17
            "PRESS P TO START",     // 18
            "L: Switch Language"    // 19
        },
        // Chinese
        {
            "分数：{0}",            // 0
            "级别：{0}",            // 1
            "消除行数：{0}",        // 2
            "下一个：",             // 3
            "操作说明",             // 4
            "左/右：移动",          // 5
            "上：旋转",             // 6
            "下：下落",             // 7
            "空格：快速下落",       // 8
            "P：暂停",              // 9
            "R：重置",              // 10
            "+/-：调整级别",        // 11
            "虚影显示落点",         // 12
            "按P继续",              // 13
            "游戏结束",             // 14
            "最终得分：{0}",        // 15
            "按R重新开始",          // 16
            "准备",                 // 17
            "按P开始",              // 18
            "L：切换语言"           // 19
        }
    };

    public GameState()
    {
        Level = 1;
        FallSpeed = 0.5f;
        StartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        CurrentLanguage = Language.Chinese;
        IsPaused = true;
    }

    /// <summary>
    /// 获取指定ID的游戏文本
    /// </summary>
    /// <param name="stringId">文本ID</param>
    /// <param name="args">格式化参数</param>
    /// <returns>格式化后的文本</returns>
    public string GetText(int stringId, params object[] args)
    {
        if (stringId < 0 || stringId >= 20)
            return string.Empty;

        return string.Format(GameStrings[(int)CurrentLanguage, stringId], args);
    }

    /// <summary>
    /// 切换游戏语言
    /// </summary>
    public void SwitchLanguage()
    {
        CurrentLanguage = CurrentLanguage == Language.English ? Language.Chinese : Language.English;
    }
}
