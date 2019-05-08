using System;

namespace BitSkinsBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Bot.Initilize.InitilizeAccount();

            FastMarketAnalize.ProfitableItems profitableItems = new FastMarketAnalize.ProfitableItems();
            profitableItems.GetProfitableItems();

            Console.ReadKey();
        }
    }
}
