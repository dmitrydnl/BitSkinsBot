using System;
using System.Collections.Generic;
using BitSkinsApi.Market;
using BitSkinsBot.EventsLog;

namespace BitSkinsBot.FastMarketAnalize
{
    internal class CheckSoldItems : ICheckSoldItems
    {
        public List<MarketItem> GetSoldItems(List<MarketItem> marketItems)
        {
            ConsoleLog.WriteInfo($"Start getting sold items. Items for checking - {marketItems.Count}");

            List<MarketItem> soldItems = new List<MarketItem>();
            foreach (MarketItem item in marketItems)
            {
                AppId.AppName app = item.App;
                DateTime offeredForSaleDate = item.OfferedForSaleDate;
                string itemId = item.Id;

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
                            ConsoleLog.WriteSellItem(app, item.Name, item.SellPrice);

                            item.SaleDate = sellHistoryRecord.Time;
                            soldItems.Add(item);
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
