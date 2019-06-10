using System.Threading;
using System.Collections.Generic;
using BitSkinsBot.FastMarketAnalize;
using BitSkinsBot.EventsLog;

namespace BitSkinsBot
{
    class Program
    {
        static void Main(string[] args)
        {
            TelegramBot.Bot.Initilize();
            Bot.Initilize.InitilizeAccount();

            MySearchFilters mySearchFilters = new MySearchFilters();
            List<ProfitableMarketItem> boughtItems = new List<ProfitableMarketItem>();
            List<ProfitableMarketItem> currentOnSaleItems = new List<ProfitableMarketItem>();
            List<ProfitableMarketItem> soldItems = new List<ProfitableMarketItem>();
            while (true)
            {
                foreach (SearchFilter filter in mySearchFilters.searchFilters)
                {
                    List<ProfitableMarketItem> profitableMarketItems = ProfitableItems.GetProfitableItems(filter);
                    if (profitableMarketItems == null || profitableMarketItems.Count == 0)
                    {
                        continue;
                    }

                    double availableBalance = BitSkinsApi.Balance.CurrentBalance.GetAccountBalance().AvailableBalance;
                    double balance = availableBalance / 5;
                    foreach (ProfitableMarketItem marketItem in profitableMarketItems)
                    {
                        if (marketItem.BuyPrice > balance)
                        {
                            continue;
                        }

                        if (balance <= 0)
                        {
                            break;
                        }

                        List<ProfitableMarketItem> items = Purchase.BuyItems(new List<ProfitableMarketItem> { marketItem });
                        foreach (ProfitableMarketItem item in items)
                        {
                            boughtItems.Add(item);
                            balance -= item.BuyPrice;
                        }
                    }
                }

                List<ProfitableMarketItem> relistedItems = RelistForSale.RelistItems(boughtItems);
                foreach (ProfitableMarketItem marketItem in relistedItems)
                {
                    boughtItems.Remove(marketItem);
                    currentOnSaleItems.Add(marketItem);
                }

                List<ProfitableMarketItem> recentlySoldItems = SoldItems.GetSoldItems(currentOnSaleItems);
                foreach (ProfitableMarketItem marketItem in recentlySoldItems)
                {
                    currentOnSaleItems.Remove(marketItem);
                    soldItems.Add(marketItem);
                }

                ConsoleLog.WriteInfo("Sleep 1 hour");
                Thread.Sleep(60 * 60 * 1000);
            }
        }
    }
}
