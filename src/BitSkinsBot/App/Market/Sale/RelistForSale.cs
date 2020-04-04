using System;
using System.Collections.Generic;
using BitSkinsBot.EventsLog;
using BitSkinsApi.Market;

namespace BitSkinsBot.Market.Sale
{
    internal class RelistForSale : IRelistForSale
    {
        public List<MarketItem> RelistItemsForSale(List<MarketItem> marketItems)
        {
            ConsoleLog.WriteInfo($"Start relist items. Count to relist - {marketItems.Count}");

            List<MarketItem> relistedItems = new List<MarketItem>();
            foreach (MarketItem item in marketItems)
            {
                AppId.AppName app = item.App;
                List<string> itemId = new List<string> { item.Id };
                List<double> itemPrice = new List<double> { item.SellPrice };

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
                    ConsoleLog.WriteItemOnSale(app, item.Name, item.SellPrice);

                    item.Id = successfullyRelistedItems[0].ItemId;
                    item.OfferedForSaleDate = DateTime.Now;
                    relistedItems.Add(item);
                }
            }

            ConsoleLog.WriteInfo($"End relist items. Successful relist - {relistedItems.Count}");

            return relistedItems;
        }
    }
}
