using System;
using BitSkinsApi.Market;

namespace BitSkinsBot.EventsLog
{
    internal static class ConsoleLog
    {
        private const ConsoleColor DEFAULT_TEXT_COLOR = ConsoleColor.White;
        private const ConsoleColor INFO_TEXT_COLOR = ConsoleColor.Cyan;
        private const ConsoleColor ERROR_TEXT_COLOR = ConsoleColor.Red;
        private const ConsoleColor PROGRESS_TEXT_COLOR = ConsoleColor.Gray;
        private const ConsoleColor BUY_ITEM_TEXT_COLOR = ConsoleColor.Magenta;
        private const ConsoleColor SELL_ITEM_TEXT_COLOR = ConsoleColor.Green;
        private const ConsoleColor ITEM_ON_SALE_TEXT_COLOR = ConsoleColor.Yellow;

        internal static void StartProgress(string text)
        {
            Console.ForegroundColor = PROGRESS_TEXT_COLOR;
            ConsoleWriteLineWithDate($"{text}. Progress - (0%)");
            ClearConsoleForegroundColor();
        }

        internal static void WriteProgress(string text, int done, int total)
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            ClearCurrentConsoleLine();

            Console.ForegroundColor = PROGRESS_TEXT_COLOR;
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
            Console.ForegroundColor = INFO_TEXT_COLOR;
            ConsoleWriteLineWithDate(textInfo);
            ClearConsoleForegroundColor();
        }

        internal static void WriteError(string textError)
        {
            Console.ForegroundColor = ERROR_TEXT_COLOR;
            ConsoleWriteLineWithDate(textError);
            ClearConsoleForegroundColor();
        }


        internal static void WriteItemOnSale(AppId.AppName app, string name, double sellPrice)
        {
            Console.ForegroundColor = ITEM_ON_SALE_TEXT_COLOR;
            ConsoleWriteLineWithDate($"{name} ({app}) on sale for {sellPrice}$");
            ClearConsoleForegroundColor();
        }

        internal static void WriteBuyItem(AppId.AppName app, string name, double buyPrice)
        {
            Console.ForegroundColor = BUY_ITEM_TEXT_COLOR;
            ConsoleWriteLineWithDate($"{name} ({app}) bought for {buyPrice}$");
            ClearConsoleForegroundColor();
        }

        internal static void WriteSellItem(AppId.AppName app, string name, double sellPrice)
        {
            Console.ForegroundColor = SELL_ITEM_TEXT_COLOR;
            ConsoleWriteLineWithDate($"{name} ({app}) sold for {sellPrice}$");
            ClearConsoleForegroundColor();
        }


        private static void ConsoleWriteLineWithDate(string message)
        {
            Console.WriteLine(DateTime.Now + " : " + message);
        }

        private static void ClearConsoleForegroundColor()
        {
            Console.ForegroundColor = DEFAULT_TEXT_COLOR;
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
