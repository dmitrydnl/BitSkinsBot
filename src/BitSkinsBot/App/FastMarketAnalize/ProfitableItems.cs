using System;
using System.Collections.Generic;
using BitSkinsApi.Market;
using BitSkinsBot.EventsLog;

namespace BitSkinsBot.FastMarketAnalize
{
    internal static class ProfitableItems
    {
        internal static List<ProfitableMarketItem> GetProfitableItems(SearchFilter searchFilter)
        {
            ConsoleLog.WriteInfo("Start get profitable items");

            List<MarketItem> marketItems = GetMarketItems(searchFilter);
            marketItems = SortProfitableItems.Sort(marketItems, searchFilter);
            List<ProfitableMarketItem> profitableMarketItems = GetProfitableMarketItems(marketItems, searchFilter);

            ConsoleLog.WriteInfo("End get profitable items");

            return profitableMarketItems;
        }

        private static List<MarketItem> GetMarketItems(SearchFilter searchFilter)
        {
            List<MarketItem> marketItems = MarketData.GetMarketData(searchFilter.App);

            ConsoleLog.WriteInfo($"Count market items - {marketItems.Count}");

            return marketItems;
        }

        private static List<ProfitableMarketItem> GetProfitableMarketItems(List<MarketItem> marketItems, SearchFilter searchFilter)
        {
            if (marketItems == null || marketItems.Count == 0)
            {
                return new List<ProfitableMarketItem>();
            }

            ConsoleLog.WriteInfo($"Start get profitable market items. Count before getting - {marketItems.Count}");
            ConsoleLog.StartProgress("Get profitable market items");
            int done = 1;

            List<ProfitableMarketItem> profitableMarketItems = new List<ProfitableMarketItem>();
            foreach (MarketItem marketItem in marketItems)
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

                ProfitableMarketItem profitableMarketItem = new ProfitableMarketItem
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

        private static List<ItemOnSale> GetItemsOnSale(SearchFilter searchFilter, string marketHashName)
        {
            List<ItemOnSale> itemsOnSale = InventoryOnSale.GetInventoryOnSale(searchFilter.App, 1, marketHashName, 0, 0, InventoryOnSale.SortBy.Price,
                    InventoryOnSale.SortOrder.Asc, InventoryOnSale.ThreeChoices.NotImportant, InventoryOnSale.ThreeChoices.NotImportant,
                    InventoryOnSale.ThreeChoices.NotImportant, InventoryOnSale.ResultsPerPage.R30, InventoryOnSale.ThreeChoices.False);
            return itemsOnSale;
        }
    }
}
