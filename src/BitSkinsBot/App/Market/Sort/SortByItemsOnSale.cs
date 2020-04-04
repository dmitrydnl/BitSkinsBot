using System.Collections.Generic;
using BitSkinsApi.Market;
using BitSkinsBot.EventsLog;

namespace BitSkinsBot.Market.Sort
{
    internal class SortByItemsOnSale : ISortMethod
    {
        private readonly SortFilter sortFilter;

        internal SortByItemsOnSale(SortFilter sortFilter)
        {
            this.sortFilter = sortFilter;
        }

        public List<BitSkinsApi.Market.MarketItem> Sort(List<BitSkinsApi.Market.MarketItem> marketItems)
        {
            if (marketItems == null || marketItems.Count == 0 || sortFilter == null)
            {
                return new List<BitSkinsApi.Market.MarketItem>();
            }

            ConsoleLog.WriteInfo($"Start sort by items on sale. Count before sort - {marketItems.Count}");
            ConsoleLog.StartProgress("Sort by items on sale");
            int done = 1;

            List<BitSkinsApi.Market.MarketItem> sortedMarketItems = new List<BitSkinsApi.Market.MarketItem>();
            foreach (BitSkinsApi.Market.MarketItem marketItem in marketItems)
            {
                ConsoleLog.WriteProgress("Sort by items on sale", done, marketItems.Count);
                done++;

                List<ItemOnSale> itemsOnSale = GetItemsOnSale(marketItem.MarketHashName);
                if (itemsOnSale.Count < 3)
                {
                    continue;
                }
                ItemOnSale itemOnSale1 = itemsOnSale[0];
                ItemOnSale itemOnSale3 = itemsOnSale[2];

                if (itemOnSale3.Price < itemOnSale1.Price * 1.05)
                {
                    continue;
                }

                if (itemOnSale1.IsMine)
                {
                    continue;
                }

                if (itemOnSale1.Price > marketItem.LowestPrice)
                {
                    continue;
                }

                sortedMarketItems.Add(marketItem);
            }

            ConsoleLog.WriteInfo($"End sort by items on sale. Count after sort - {sortedMarketItems.Count}");

            return sortedMarketItems;
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
