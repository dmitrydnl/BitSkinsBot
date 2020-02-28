using System;
using System.Collections.Generic;
using BitSkinsApi.Market;
using BitSkinsBot.EventsLog;

namespace BitSkinsBot.FastMarketAnalize
{
    internal static class ProfitableItems
    {
        internal static List<MarketItem> GetProfitableItems(SortFilter searchFilter)
        {
            ConsoleLog.WriteInfo("Start get profitable items");

            List<BitSkinsApi.Market.MarketItem> marketItems = GetMarketItems(searchFilter);
            marketItems = SortItems.Sort(marketItems, searchFilter);
            List<MarketItem> profitableMarketItems = GetProfitableMarketItems(marketItems, searchFilter);

            ConsoleLog.WriteInfo("End get profitable items");

            return profitableMarketItems;
        }

        private static List<BitSkinsApi.Market.MarketItem> GetMarketItems(SortFilter searchFilter)
        {
            List<BitSkinsApi.Market.MarketItem> marketItems = MarketData.GetMarketData(searchFilter.App);

            ConsoleLog.WriteInfo($"Count market items - {marketItems.Count}");

            return marketItems;
        }

        private static List<MarketItem> GetProfitableMarketItems(List<BitSkinsApi.Market.MarketItem> marketItems, SortFilter searchFilter)
        {
            if (marketItems == null || marketItems.Count == 0)
            {
                return new List<MarketItem>();
            }

            ConsoleLog.WriteInfo($"Start get profitable market items. Count before getting - {marketItems.Count}");
            ConsoleLog.StartProgress("Get profitable market items");
            int done = 1;

            List<MarketItem> profitableMarketItems = new List<MarketItem>();
            foreach (BitSkinsApi.Market.MarketItem marketItem in marketItems)
            {
                ConsoleLog.WriteProgress("Get profitable market items", done, marketItems.Count);
                done++;

                string marketHashName = marketItem.MarketHashName;
                List<ItemOnSale> itemsOnSale = GetItemsOnSale(searchFilter, marketHashName);
                if (itemsOnSale.Count < 2)
                {
                    continue;
                }
                ItemOnSale itemOnSale1 = itemsOnSale[0];
                ItemOnSale itemOnSale2 = itemsOnSale[1];

                int sellPricePercentFromBuyPrice = searchFilter.MinAveragePriceInLastWeekPercentFromLowestPrice == null ? 110
                    : Math.Max(110, searchFilter.MinAveragePriceInLastWeekPercentFromLowestPrice.Value);
                double sellPrice = itemOnSale1.Price / 100 * sellPricePercentFromBuyPrice;
                if (itemOnSale2.Price > sellPrice + 0.01)
                {
                    sellPrice = itemOnSale2.Price - 0.01;
                }
                sellPrice = Math.Round(sellPrice, 2);

                MarketItem profitableMarketItem = new MarketItem
                {
                    App = searchFilter.App,
                    Name = marketItem.MarketHashName,
                    Id = itemOnSale1.ItemId,
                    BuyPrice = itemOnSale1.Price,
                    SellPrice = sellPrice
                };
                profitableMarketItems.Add(profitableMarketItem);
            }

            ConsoleLog.WriteInfo($"End get profitable market items. Count after gettnig - {profitableMarketItems.Count}");

            return profitableMarketItems;
        }

        private static List<ItemOnSale> GetItemsOnSale(SortFilter searchFilter, string marketHashName)
        {
            List<ItemOnSale> itemsOnSale = InventoryOnSale.GetInventoryOnSale(searchFilter.App, 1, marketHashName, 0, 0, InventoryOnSale.SortBy.Price,
                    InventoryOnSale.SortOrder.Asc, InventoryOnSale.ThreeChoices.NotImportant, InventoryOnSale.ThreeChoices.NotImportant,
                    InventoryOnSale.ThreeChoices.NotImportant, InventoryOnSale.ResultsPerPage.R30, InventoryOnSale.ThreeChoices.False);
            return itemsOnSale;
        }
    }
}
