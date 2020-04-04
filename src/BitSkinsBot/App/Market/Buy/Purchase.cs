using System;
using System.Collections.Generic;
using BitSkinsBot.EventsLog;
using BitSkinsApi.Market;

namespace BitSkinsBot.Market.Buy
{
    internal class Purchase : IPurchase
    {
        public List<MarketItem> PurchaseItems(List<MarketItem> marketItems)
        {
            ConsoleLog.WriteInfo($"Start buy items. Count to buy - {marketItems.Count}");

            List<MarketItem> boughtItems = new List<MarketItem>();
            foreach (MarketItem item in marketItems)
            {
                AppId.AppName app = item.App;
                List<string> itemId = new List<string> { item.Id };
                List<double> itemPrice = new List<double> { item.BuyPrice };

                List<BoughtItem> successfullyBoughtItems = null;
                try
                {
                    successfullyBoughtItems = BitSkinsApi.Market.Purchase.BuyItem(app, itemId, itemPrice, false, false);
                }
                catch (Exception exception)
                {
                    ConsoleLog.WriteError(exception.Message);
                }

                if (successfullyBoughtItems != null)
                {
                    ConsoleLog.WriteBuyItem(app, item.Name, item.BuyPrice);

                    item.Id = successfullyBoughtItems[0].ItemId;
                    item.WithdrawableAt = successfullyBoughtItems[0].WithdrawableAt;
                    item.BuyDate = DateTime.Now;
                    boughtItems.Add(item);
                }
            }

            ConsoleLog.WriteInfo($"End buy items. Successful bought - {boughtItems.Count}");

            return boughtItems;
        }
    }
}
