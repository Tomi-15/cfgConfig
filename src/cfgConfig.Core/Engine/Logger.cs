using System;

namespace cfgConfig.Core.Engine
{
    internal static class Logger
    {
        public static void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"[{DateTime.Now.ToString("HH:mm:ss")}] [CONFIGURATION] ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"[INFO] {message}\n");
            Console.ResetColor();
        }

        public static void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"[{DateTime.Now.ToString("HH:mm:ss")}] [CONFIGURATION] ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"[ERROR] {message}\n");
            Console.ResetColor();
        }
    }
}
