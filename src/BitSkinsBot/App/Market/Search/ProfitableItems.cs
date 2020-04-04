using System;
using System.Collections.Generic;
using BitSkinsBot.Market.Sort;
using BitSkinsBot.EventsLog;
using BitSkinsApi.Market;

namespace BitSkinsBot.FastMarketAnalize
{
    internal class ProfitableItems : IProfitableItems
    {
        private readonly SortFilter sortFilter;
        private readonly ISortMethod sortItems;

        internal ProfitableItems(SortFilter sortFilter)
        {
            this.sortFilter = sortFilter;
            sortItems = new SortItems(sortFilter);
        }

        public List<MarketItem> SearchProfitableItems()
        {
            ConsoleLog.WriteInfo("Start get profitable items");

            List<BitSkinsApi.Market.MarketItem> marketItems = GetMarketItems();
            marketItems = sortItems.Sort(marketItems);
            List<MarketItem> profitableMarketItems = SearchProfitableMarketItems(marketItems);

            ConsoleLog.WriteInfo("End get profitable items");

            return profitableMarketItems;
        }

        private List<BitSkinsApi.Market.MarketItem> GetMarketItems()
        {
            List<BitSkinsApi.Market.MarketItem> marketItems = MarketData.GetMarketData(sortFilter.App);

            ConsoleLog.WriteInfo($"Count market items - {marketItems.Count}");

            return marketItems;
        }

        private List<MarketItem> SearchProfitableMarketItems(List<BitSkinsApi.Market.MarketItem> marketItems)
        {
            if (marketItems == null || marketItems.Count == 0 || sortFilter == null)
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

                List<ItemOnSale> itemsOnSale = GetItemsOnSale(marketItem.MarketHashName);
                if (itemsOnSale.Count < 2)
                {
                    continue;
                }
                ItemOnSale itemOnSale1 = itemsOnSale[0];
                ItemOnSale itemOnSale2 = itemsOnSale[1];

                int sellPricePercentFromBuyPrice = sortFilter.MinAveragePriceInLastWeekPercentFromLowestPrice == null ? 110
                    : Math.Max(110, sortFilter.MinAveragePriceInLastWeekPercentFromLowestPrice.Value);
                double sellPrice = itemOnSale1.Price / 100 * sellPricePercentFromBuyPrice;
                if (itemOnSale2.Price > sellPrice + 0.01)
                {
                    sellPrice = itemOnSale2.Price - 0.01;
                }
                sellPrice = Math.Round(sellPrice, 2);

                MarketItem profitableMarketItem = new MarketItem
                {
                    App = sortFilter.App,
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

        private List<ItemOnSale> GetItemsOnSale(string marketHashName)
        {
            List<ItemOnSale> itemsOnSale = InventoryOnSale.GetInventoryOnSale(sortFilter.App, 1, marketHashName, 0, 0, InventoryOnSale.SortBy.Price,
                    InventoryOnSale.SortOrder.Asc, InventoryOnSale.ThreeChoices.NotImportant, InventoryOnSale.ThreeChoices.NotImportant,
                    InventoryOnSale.ThreeChoices.NotImportant, InventoryOnSale.ResultsPerPage.R30, InventoryOnSale.ThreeChoices.False);
            return itemsOnSale;
        }
    }
}
