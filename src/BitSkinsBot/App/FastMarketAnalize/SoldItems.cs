using System;
using System.Collections.Generic;
using BitSkinsApi.Market;
using BitSkinsBot.EventsLog;

namespace BitSkinsBot.FastMarketAnalize
{
    internal static class SoldItems
    {
        internal static List<MarketItem> GetSoldItems(List<MarketItem> profitableMarketItems)
        {
            ConsoleLog.WriteInfo($"Start getting sold items. Items for checking - {profitableMarketItems.Count}");

            List<MarketItem> soldItems = new List<MarketItem>();
            foreach (MarketItem marketItem in profitableMarketItems)
            {
                AppId.AppName app = marketItem.App;
                DateTime offeredForSaleDate = marketItem.OfferedForSaleDate;
                string itemId = marketItem.Id;

                List<SellHistoryRecord> sellHistoryRecords = null;
                try
                {
                    sellHistoryRecords = SellHistory.GetSellHistory(app, 1);
                }
                catch (Exception exception)
                {
                    ConsoleLog.WriteError(exception.Message);
                }

                if (sellHistoryRecords != null)
                {
                    foreach (SellHistoryRecord sellHistoryRecord in sellHistoryRecords)
                    {
                        if (sellHistoryRecord.Time < offeredForSaleDate)
                        {
                            break;
                        }

                        if (sellHistoryRecord.ItemId == itemId)
                        {
                            ConsoleLog.WriteSellItem(app, marketItem.Name, marketItem.SellPrice);

                            marketItem.SaleDate = sellHistoryRecord.Time;
                            soldItems.Add(marketItem);
                            break;
                        }
                    }
                }
            }

            ConsoleLog.WriteInfo($"End getting sold items. Items sold - {soldItems.Count}");

            return soldItems;
        }
    }
}
