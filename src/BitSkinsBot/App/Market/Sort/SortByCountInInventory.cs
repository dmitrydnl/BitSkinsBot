using System.Collections.Generic;
using BitSkinsBot.EventsLog;
using BitSkinsApi.Inventory;

namespace BitSkinsBot.Market.Sort
{
    internal class SortByCountInInventory : ISortMethod
    {
        private readonly SortFilter sortFilter;

        internal SortByCountInInventory(SortFilter sortFilter)
        {
            this.sortFilter = sortFilter;
        }

        public List<BitSkinsApi.Market.MarketItem> Sort(List<BitSkinsApi.Market.MarketItem> marketItems)
        {
            if (marketItems == null || marketItems.Count == 0 || sortFilter == null)
            {
                return new List<BitSkinsApi.Market.MarketItem>();
            }

            int? minItemsOnSale = sortFilter.MinItemsOnSale;
            int? maxItemsOnSale = sortFilter.MaxItemsOnSale;
            BitSkinsInventory bitSkinsInventory = Inventories.GetAccountInventory(sortFilter.App, 1).BitSkinsInventory;

            ConsoleLog.WriteInfo($"Start sort by count in inventory. Count before sort  - {marketItems.Count}");
            ConsoleLog.StartProgress("Sort by count in inventory");
            int done = 1;

            List<BitSkinsApi.Market.MarketItem> sortedMarketItems = new List<BitSkinsApi.Market.MarketItem>();
            foreach (BitSkinsApi.Market.MarketItem marketItem in marketItems)
            {
                ConsoleLog.WriteProgress("Sort by count in inventory", done, marketItems.Count);
                done++;

                int countItemsOnSale = 0;
                foreach (BitSkinsInventoryItem inventoryItem in bitSkinsInventory.BitSkinsInventoryItems)
                {
                    if (string.Equals(marketItem.MarketHashName, inventoryItem.MarketHashName))
                    {
                        countItemsOnSale = inventoryItem.NumberOfItems;
                        break;
                    }
                }

                if (minItemsOnSale != null && countItemsOnSale < minItemsOnSale)
                {
                    continue;
                }

                if (maxItemsOnSale != null && countItemsOnSale >= maxItemsOnSale)
                {
                    continue;
                }

                sortedMarketItems.Add(marketItem);
            }

            ConsoleLog.WriteInfo($"End sort by count in inventory. Count after sort - {sortedMarketItems.Count}");

            return sortedMarketItems;
        }
    }
}
