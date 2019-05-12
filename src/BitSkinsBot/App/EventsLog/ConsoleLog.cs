using System;
using BitSkinsApi.Market;

namespace BitSkinsBot.EventsLog
{
    internal static class ConsoleLog
    {
        private const ConsoleColor DEFAULT_FOREGROUND_COLOR = ConsoleColor.White;

        internal static void StartProgeress(string text)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            ConsoleWriteLineWithDate($"{text}. Progress - (0%)");
            ClearConsoleForegroundColor();
        }

        internal static void WriteProgress(string text, int done, int total)
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            ClearCurrentConsoleLine();

            Console.ForegroundColor = ConsoleColor.Gray;
            if (done >= total)
            {
                ConsoleWriteLineWithDate($"{text}. Complete");
            }
            else
            {
                double complete = Math.Round((double)done / (double)total * 100, 2);
                ConsoleWriteLineWithDate($"{text}. Progress - ({complete}%)");
            }
            ClearConsoleForegroundColor();
        }


        internal static void WriteInfo(string textInfo)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            ConsoleWriteLineWithDate(textInfo);
            ClearConsoleForegroundColor();
        }

        internal static void WriteError(string textError)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleWriteLineWithDate(textError);
            ClearConsoleForegroundColor();
        }


        internal static void WriteItemOnSale(AppId.AppName app, string name, double sellPrice)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            ConsoleWriteLineWithDate($"{name} ({app}) on sale for {sellPrice}$");
            ClearConsoleForegroundColor();
        }

        internal static void WriteBuyItem(AppId.AppName app, string name, double buyPrice)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            ConsoleWriteLineWithDate($"{name} ({app}) bought for {buyPrice}$");
            ClearConsoleForegroundColor();
        }

        private static void ConsoleWriteLineWithDate(string message)
        {
            Console.WriteLine(DateTime.Now + " : " + message);
        }

        private static void ClearConsoleForegroundColor()
        {
            Console.ForegroundColor = DEFAULT_FOREGROUND_COLOR;
        }

        private static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}
