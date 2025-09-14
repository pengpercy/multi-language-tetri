using System;
using SDL2;
using static SDL2.SDL;

namespace TetrisSharp;

public class Program
{
    private const int SCREEN_WIDTH = 800;
    private const int SCREEN_HEIGHT = 700;

    public static int Main(string[] args)
    {

        if (SDL_Init(SDL_INIT_VIDEO) < 0)
        {
            Console.WriteLine($"SDL初始化失败: {SDL_GetError()}");
            return 1;
        }

        try
        {
            // 启动游戏
            using var game = new Game();
            game.Run();
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"游戏运行时发生错误: {ex}");
            return 1;
        }
        finally
        {
            SDL_Quit();
        }
    }
}
