using System;
using System.Runtime.InteropServices;
using SDL2;

namespace TetrisSharp
{
    public static class SDL_ttf
    {
        private const string nativeLibName = "SDL2_ttf";

        #region SDL_ttf.h

        public const int TTF_MAJOR_VERSION = 2;
        public const int TTF_MINOR_VERSION = 20;
        public const int TTF_PATCHLEVEL = 1;

        [Flags]
        public enum TTF_Style : int
        {
            TTF_STYLE_NORMAL = 0x00,
            TTF_STYLE_BOLD = 0x01,
            TTF_STYLE_ITALIC = 0x02,
            TTF_STYLE_UNDERLINE = 0x04,
            TTF_STYLE_STRIKETHROUGH = 0x08
        }

        public enum TTF_Hinting : int
        {
            TTF_HINTING_NORMAL = 0,
            TTF_HINTING_LIGHT = 1,
            TTF_HINTING_MONO = 2,
            TTF_HINTING_NONE = 3,
            TTF_HINTING_LIGHT_SUBPIXEL = 4
        }

        /* IntPtr refers to a TTF_Font* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int TTF_Init();

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TTF_OpenFont(
            [In()] [MarshalAs(UnmanagedType.LPStr)]
                string file,
            int ptsize
        );

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TTF_OpenFontIndex(
            [In()] [MarshalAs(UnmanagedType.LPStr)]
                string file,
            int ptsize,
            long index
        );

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TTF_OpenFontRW(
            IntPtr src,
            int freesrc,
            int ptsize
        );

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TTF_OpenFontIndexRW(
            IntPtr src,
            int freesrc,
            int ptsize,
            long index
        );

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int TTF_GetFontStyle(IntPtr font);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void TTF_SetFontStyle(IntPtr font, int style);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int TTF_GetFontOutline(IntPtr font);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void TTF_SetFontOutline(IntPtr font, int outline);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int TTF_GetFontHinting(IntPtr font);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void TTF_SetFontHinting(IntPtr font, int hinting);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int TTF_FontHeight(IntPtr font);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int TTF_FontAscent(IntPtr font);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int TTF_FontDescent(IntPtr font);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int TTF_FontLineSkip(IntPtr font);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int TTF_GetFontKerning(IntPtr font);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void TTF_SetFontKerning(IntPtr font, int allowed);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long TTF_FontFaces(IntPtr font);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int TTF_FontFaceIsFixedWidth(IntPtr font);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string TTF_FontFaceFamilyName(IntPtr font);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public static extern string TTF_FontFaceStyleName(IntPtr font);

        /* IntPtr refers to a SDL_Surface* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TTF_RenderText_Solid(
            IntPtr font,
            [In()] [MarshalAs(UnmanagedType.LPStr)]
                string text,
            SDL.SDL_Color fg
        );

        /* IntPtr refers to a SDL_Surface* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TTF_RenderUTF8_Solid(
            IntPtr font,
            [In()] [MarshalAs(UnmanagedType.LPUTF8Str)]
                string text,
            SDL.SDL_Color fg
        );

        /* IntPtr refers to a SDL_Surface* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TTF_RenderUNICODE_Solid(
            IntPtr font,
            [In()] [MarshalAs(UnmanagedType.LPWStr)]
                string text,
            SDL.SDL_Color fg
        );

        /* IntPtr refers to a SDL_Surface* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TTF_RenderText_Shaded(
            IntPtr font,
            [In()] [MarshalAs(UnmanagedType.LPStr)]
                string text,
            SDL.SDL_Color fg,
            SDL.SDL_Color bg
        );

        /* IntPtr refers to a SDL_Surface* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TTF_RenderUTF8_Shaded(
            IntPtr font,
            [In()] [MarshalAs(UnmanagedType.LPUTF8Str)]
                string text,
            SDL.SDL_Color fg,
            SDL.SDL_Color bg
        );

        /* IntPtr refers to a SDL_Surface* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TTF_RenderUNICODE_Shaded(
            IntPtr font,
            [In()] [MarshalAs(UnmanagedType.LPWStr)]
                string text,
            SDL.SDL_Color fg,
            SDL.SDL_Color bg
        );

        /* IntPtr refers to a SDL_Surface* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TTF_RenderText_Blended(
            IntPtr font,
            [In()] [MarshalAs(UnmanagedType.LPStr)]
                string text,
            SDL.SDL_Color fg
        );

        /* IntPtr refers to a SDL_Surface* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TTF_RenderUTF8_Blended(
            IntPtr font,
            [In()] [MarshalAs(UnmanagedType.LPUTF8Str)]
                string text,
            SDL.SDL_Color fg
        );

        /* IntPtr refers to a SDL_Surface* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TTF_RenderUNICODE_Blended(
            IntPtr font,
            [In()] [MarshalAs(UnmanagedType.LPWStr)]
                string text,
            SDL.SDL_Color fg
        );

        /* IntPtr refers to a SDL_Surface* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TTF_RenderText_Blended_Wrapped(
            IntPtr font,
            [In()] [MarshalAs(UnmanagedType.LPStr)]
                string text,
            SDL.SDL_Color fg,
            uint wrapped
        );

        /* IntPtr refers to a SDL_Surface* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TTF_RenderUTF8_Blended_Wrapped(
            IntPtr font,
            [In()] [MarshalAs(UnmanagedType.LPUTF8Str)]
                string text,
            SDL.SDL_Color fg,
            uint wrapped
        );

        /* IntPtr refers to a SDL_Surface* */
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr TTF_RenderUNICODE_Blended_Wrapped(
            IntPtr font,
            [In()] [MarshalAs(UnmanagedType.LPWStr)]
                string text,
            SDL.SDL_Color fg,
            uint wrapped
        );

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int TTF_SizeText(
            IntPtr font,
            [In()] [MarshalAs(UnmanagedType.LPStr)]
                string text,
            out int w,
            out int h
        );

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int TTF_SizeUTF8(
            IntPtr font,
            [In()] [MarshalAs(UnmanagedType.LPUTF8Str)]
                string text,
            out int w,
            out int h
        );

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int TTF_SizeUNICODE(
            IntPtr font,
            [In()] [MarshalAs(UnmanagedType.LPWStr)]
                string text,
            out int w,
            out int h
        );

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void TTF_CloseFont(IntPtr font);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void TTF_Quit();

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int TTF_WasInit();

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int TTF_GetFontKerningSize(
            IntPtr font,
            int prev_index,
            int index
        );

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int TTF_GetFontKerningSizeGlyphs(
            IntPtr font,
            ushort previous_ch,
            ushort ch
        );

        #endregion

        #region Helper Functions

        public static void TTF_VERSION(out SDL.SDL_version X)
        {
            X.major = TTF_MAJOR_VERSION;
            X.minor = TTF_MINOR_VERSION;
            X.patch = TTF_PATCHLEVEL;
        }

        public static SDL.SDL_version TTF_VERSION()
        {
            SDL.SDL_version result;
            TTF_VERSION(out result);
            return result;
        }

        public static bool TTF_VERSION_ATLEAST(byte X, byte Y, byte Z)
        {
            return (TTF_MAJOR_VERSION >= X &&
                    TTF_MINOR_VERSION >= Y &&
                    TTF_PATCHLEVEL >= Z);
        }

        #endregion
    }
}
