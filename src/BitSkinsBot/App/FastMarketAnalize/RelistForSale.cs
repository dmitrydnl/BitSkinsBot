using System;
using System.Collections.Generic;
using BitSkinsApi.Market;
using BitSkinsBot.EventsLog;

namespace BitSkinsBot.FastMarketAnalize
{
    internal static class RelistForSale
    {
        internal static List<MarketItem> RelistItems(List<MarketItem> profitableMarketItems)
        {
            ConsoleLog.WriteInfo($"Start relist items. Count to relist - {profitableMarketItems.Count}");

            List<MarketItem> relistedItems = new List<MarketItem>();
            foreach (MarketItem marketItem in profitableMarketItems)
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

            ConsoleLog.WriteInfo($"End relist items. Successful relist - {relistedItems.Count}");

            return relistedItems;
        }
    }
}
