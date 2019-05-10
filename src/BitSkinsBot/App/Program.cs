using System;
using System.Collections.Generic;
using BitSkinsBot.FastMarketAnalize;

namespace BitSkinsBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Bot.Initilize.InitilizeAccount();

            Filters filters = new Filters();
            foreach (Filter filter in filters.filters)
            {
                ProfitableItems profitableItems = new ProfitableItems(filter);
                List<ProfitableMarketItem> profitableMarketItems = profitableItems.GetProfitableItems();

                Console.WriteLine("PROFITABLE ITEMS");
                foreach (ProfitableMarketItem profitableMarketItem in profitableMarketItems)
                {
                    Console.WriteLine("Name: " + profitableMarketItem.Name);
                    Console.WriteLine("ID: " + profitableMarketItem.Id);
                    Console.WriteLine("Buy price: " + profitableMarketItem.BuyPrice);
                    Console.WriteLine("Sell price: " + profitableMarketItem.SellPrice);
                    Console.WriteLine();
                }
            }

            Console.ReadKey();
        }
    }
}
