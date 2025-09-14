using System;
using System.Runtime.InteropServices;
using SDL2;
using static SDL2.SDL;
using static TetrisSharp.SDL_ttf;

namespace TetrisSharp;

public class FontManager : IDisposable
{
    private IntPtr _font;
    private IntPtr _smallFont;
    private IntPtr _renderer;
    private bool _isDisposed;
    private bool _useTtf;

    public FontManager(IntPtr renderer)
    {
        _renderer = renderer;
        Initialize();
    }

    private void Initialize()
    {
        _useTtf = TryInitializeTTF();
        if (!_useTtf)
        {
            Console.WriteLine("Using improved bitmap fonts");
        }
    }

    private bool TryInitializeTTF()
    {
        try
        {
            if (TTF_Init() < 0)
            {
                Console.WriteLine($"TTF_Init failed: {SDL_GetError()}");
                return false;
            }

            return LoadFonts();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TTF initialization failed: {ex.Message}");
            return false;
        }
    }

    private bool LoadFonts()
    {
        string[] fontPaths = {
            "/System/Library/Fonts/PingFang.ttc",
            "/System/Library/Fonts/Hiragino Sans GB.ttc",
            "/System/Library/Fonts/STHeiti Light.ttc",
            "/System/Library/Fonts/Helvetica.ttc",
            "/System/Library/Fonts/Arial.ttf",
            "/usr/share/fonts/truetype/noto/NotoSansCJK-Regular.ttc",
            "C:/Windows/Fonts/msyh.ttc",
            "/Windows/Fonts/msyh.ttc"
        };

        foreach (string fontPath in fontPaths)
        {
            _font = TTF_OpenFont(fontPath, 18);  // Smaller size
            if (_font != IntPtr.Zero)
            {
                TTF_SetFontHinting(_font, (int)TTF_Hinting.TTF_HINTING_NORMAL);
                TTF_SetFontKerning(_font, 0);

                _smallFont = TTF_OpenFont(fontPath, 14);  // Even smaller
                if (_smallFont != IntPtr.Zero)
                {
                    TTF_SetFontHinting(_smallFont, (int)TTF_Hinting.TTF_HINTING_NORMAL);
                    TTF_SetFontKerning(_smallFont, 0);

                    Console.WriteLine($"✅ Loaded TTF fonts: {fontPath}");
                    return true;
                }
                else
                {
                    TTF_CloseFont(_font);
                    _font = IntPtr.Zero;
                }
            }
        }

        return false;
    }

    public void DrawText(string text, int x, int y, Color color, FontSize size = FontSize.Medium, TextAlignment alignment = TextAlignment.Left)
    {
        if (string.IsNullOrEmpty(text)) return;

        if (_useTtf)
        {
            DrawTTFText(text, x, y, color, size, alignment);
        }
        else
        {
            DrawBitmapText(text, x, y, color, size, alignment);
        }
    }

    private void DrawTTFText(string text, int x, int y, Color color, FontSize size, TextAlignment alignment)
    {
        IntPtr font = size == FontSize.Small ? _smallFont : _font;
        if (font == IntPtr.Zero) return;

        var sdlColor = new SDL_Color { r = color.R, g = color.G, b = color.B, a = color.A };
        IntPtr surface = TTF_RenderUTF8_Blended(font, text, sdlColor);
        if (surface == IntPtr.Zero) return;

        try
        {
            IntPtr texture = SDL_CreateTextureFromSurface(_renderer, surface);
            if (texture == IntPtr.Zero) return;

            try
            {
                SDL_QueryTexture(texture, out _, out _, out int textWidth, out int textHeight);

                int renderX = alignment switch
                {
                    TextAlignment.Center => x - textWidth / 2,
                    TextAlignment.Right => x - textWidth,
                    _ => x
                };

                var destRect = new SDL_Rect
                {
                    x = renderX,
                    y = y,
                    w = textWidth,
                    h = textHeight
                };

                SDL_RenderCopy(_renderer, texture, IntPtr.Zero, ref destRect);
            }
            finally
            {
                SDL_DestroyTexture(texture);
            }
        }
        finally
        {
            SDL_FreeSurface(surface);
        }
    }

    private void DrawBitmapText(string text, int x, int y, Color color, FontSize size, TextAlignment alignment)
    {
        float scale = size switch
        {
            FontSize.Small => 1.4f,   // Much smaller than before
            FontSize.Medium => 1.8f,  // Reasonable size
            FontSize.Large => 2.4f,   // Still readable but not huge
            _ => 1.8f
        };

        var textSize = MeasureBitmapText(text, scale);
        int renderX = alignment switch
        {
            TextAlignment.Center => x - textSize.width / 2,
            TextAlignment.Right => x - textSize.width,
            _ => x
        };

        SDL_SetRenderDrawBlendMode(_renderer, SDL_BlendMode.SDL_BLENDMODE_BLEND);

        int currentX = renderX;
        int currentY = y;
        int startX = renderX;

        foreach (char c in text)
        {
            if (c == '\n')
            {
                currentY += (int)(12 * scale);
                currentX = startX;
                continue;
            }

            if (c == ' ')
            {
                currentX += (int)(6 * scale);
                continue;
            }

            byte[] pattern = FontData.GetCharacterData(c);
            DrawCharacterImproved(pattern, currentX, currentY, color, scale);

            bool isChinese = c > 127;
            float charWidth = isChinese ? 8.5f : 7.0f;
            currentX += (int)(charWidth * scale);
        }

        SDL_SetRenderDrawBlendMode(_renderer, SDL_BlendMode.SDL_BLENDMODE_NONE);
    }

    private void DrawCharacterImproved(byte[] pattern, int x, int y, Color color, float scale)
    {
        float pixelSize = scale * 0.9f;  // Slightly smaller pixels

        // Single pass rendering with edge smoothing
        SDL_SetRenderDrawColor(_renderer, color.R, color.G, color.B, color.A);

        for (int row = 0; row < 7; row++)
        {
            byte rowData = pattern[row];
            for (int col = 0; col < 6; col++)
            {
                if ((rowData & (1 << (5 - col))) != 0)
                {
                    // Main pixel
                    var rect = new SDL_Rect
                    {
                        x = (int)(x + col * pixelSize),
                        y = (int)(y + row * pixelSize),
                        w = (int)Math.Ceiling(pixelSize),
                        h = (int)Math.Ceiling(pixelSize)
                    };
                    SDL_RenderFillRect(_renderer, ref rect);
                }
            }
        }

        // Light edge smoothing for larger scales
        if (scale >= 2.0f)
        {
            byte edgeAlpha = (byte)(color.A * 0.2f);
            SDL_SetRenderDrawColor(_renderer, color.R, color.G, color.B, edgeAlpha);

            for (int row = 0; row < 7; row++)
            {
                byte rowData = pattern[row];
                for (int col = 0; col < 6; col++)
                {
                    if ((rowData & (1 << (5 - col))) != 0)
                    {
                        // Right edge
                        if (col < 5 && (rowData & (1 << (4 - col))) == 0)
                        {
                            SDL_RenderDrawPoint(_renderer, (int)(x + (col + 1) * pixelSize), (int)(y + row * pixelSize));
                        }

                        // Bottom edge
                        if (row < 6 && (pattern[row + 1] & (1 << (5 - col))) == 0)
                        {
                            SDL_RenderDrawPoint(_renderer, (int)(x + col * pixelSize), (int)(y + (row + 1) * pixelSize));
                        }
                    }
                }
            }
        }
    }

    public (int width, int height) MeasureText(string text, FontSize size = FontSize.Medium)
    {
        if (string.IsNullOrEmpty(text)) return (0, 0);

        if (_useTtf)
        {
            IntPtr font = size == FontSize.Small ? _smallFont : _font;
            if (font != IntPtr.Zero && TTF_SizeUTF8(font, text, out int width, out int height) == 0)
            {
                return (width, height);
            }
        }

        float scale = size switch
        {
            FontSize.Small => 1.4f,
            FontSize.Medium => 1.8f,
            FontSize.Large => 2.4f,
            _ => 1.8f
        };

        return MeasureBitmapText(text, scale);
    }

    private (int width, int height) MeasureBitmapText(string text, float scale)
    {
        int width = 0;
        int height = (int)(7 * scale * 0.9f);
        int maxWidth = 0;

        foreach (char c in text)
        {
            if (c == '\n')
            {
                maxWidth = Math.Max(maxWidth, width);
                width = 0;
                height += (int)(12 * scale);
                continue;
            }

            if (c == ' ')
            {
                width += (int)(6 * scale);
                continue;
            }

            bool isChinese = c > 127;
            float charWidth = isChinese ? 8.5f : 7.0f;
            width += (int)(charWidth * scale);
        }

        maxWidth = Math.Max(maxWidth, width);
        return (maxWidth, height);
    }

    public void Dispose()
    {
        if (_isDisposed) return;

        if (_font != IntPtr.Zero)
        {
            TTF_CloseFont(_font);
            _font = IntPtr.Zero;
        }

        if (_smallFont != IntPtr.Zero)
        {
            TTF_CloseFont(_smallFont);
            _smallFont = IntPtr.Zero;
        }

        if (_useTtf)
        {
            TTF_Quit();
        }

        _isDisposed = true;
    }
}

public enum FontSize
{
    Small,
    Medium,
    Large
}

public enum TextAlignment
{
    Left,
    Center,
    Right
}
