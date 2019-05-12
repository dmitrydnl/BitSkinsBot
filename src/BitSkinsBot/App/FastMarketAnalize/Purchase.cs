using System;
using System.Collections.Generic;
using BitSkinsApi.Market;
using BitSkinsBot.EventsLog;

namespace BitSkinsBot.FastMarketAnalize
{
    internal static class Purchase
    {
        internal static List<ProfitableMarketItem> BuyItems(List<ProfitableMarketItem> profitableMarketItems)
        {
            ConsoleLog.WriteInfo($"Start buy items. Count to buy - {profitableMarketItems.Count}");

            List<ProfitableMarketItem> boughtItems = new List<ProfitableMarketItem>();
            foreach (ProfitableMarketItem marketItem in profitableMarketItems)
            {
                AppId.AppName app = marketItem.App;
                List<string> itemId = new List<string> { marketItem.Id };
                List<double> itemPrice = new List<double> { marketItem.BuyPrice };

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
                    ConsoleLog.WriteBuyItem(app, marketItem.Name, marketItem.BuyPrice);

                    marketItem.Id = successfullyBoughtItems[0].ItemId;
                    marketItem.WithdrawableAt = successfullyBoughtItems[0].WithdrawableAt;
                    marketItem.BuyDate = DateTime.Now;
                    boughtItems.Add(marketItem);
                }
            }

            ConsoleLog.WriteInfo($"End buy items. Successful bought - {boughtItems.Count}");

            return boughtItems;
        }
    }
}
