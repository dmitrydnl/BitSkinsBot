using System;
using System.Collections.Generic;
using BitSkinsApi.Market;
using BitSkinsBot.Log;

namespace BitSkinsBot.FastMarketAnalize
{
    internal static class RelistForSale
    {
        internal static List<ProfitableMarketItem> RelistItems(List<ProfitableMarketItem> profitableMarketItems)
        {
            ConsoleLog.WriteInfo("Start relist items");
            List<ProfitableMarketItem> relistedItems = new List<ProfitableMarketItem>();
            foreach (ProfitableMarketItem marketItem in profitableMarketItems)
            {
                AppId.AppName app = marketItem.App;
                List<string> itemId = new List<string> { marketItem.Id };
                List<double> itemPrice = new List<double> { marketItem.SellPrice };

                List<RelistedItem> successfullyRelistedItems = null;
                try
                {
                    successfullyRelistedItems = BitSkinsApi.Market.RelistForSale.RelistItem(app, itemId, itemPrice);
                }
                catch (Exception exception)
                {
                    ConsoleLog.WriteError(exception.Message);
                }

                if (successfullyRelistedItems != null)
                {
                    ConsoleLog.WriteItemOnSale(app, marketItem.Name, marketItem.SellPrice);
                    marketItem.Id = successfullyRelistedItems[0].ItemId;
                    marketItem.OfferedForSaleDate = DateTime.Now;
                    relistedItems.Add(marketItem);
                }
            }

            return relistedItems;
        }
    }
}
