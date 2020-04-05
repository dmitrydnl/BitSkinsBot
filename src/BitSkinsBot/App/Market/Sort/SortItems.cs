using System.Collections.Generic;
using BitSkinsBot.EventsLog;

namespace BitSkinsBot.Market.Sort
{
    internal class SortItems : ISortMethod
    {
        private ISortMethod sortByTotalItems;
        private ISortMethod sortByLowestPrice;
        private ISortMethod sortByHighestPrice;
        private ISortMethod sortByCumulativePrice;
        private ISortMethod sortByRecentAveragePrice;
        private ISortMethod sortByItemsOnSale;
        private ISortMethod sortByRecentSales;
        private ISortMethod sortByCountInInventory;

        internal SortItems(SortFilter sortFilter)
        {
            sortByTotalItems = new SortByTotalItems(sortFilter);
            sortByLowestPrice = new SortByLowestPrice(sortFilter);
            sortByHighestPrice = new SortByHighestPrice(sortFilter);
            sortByCumulativePrice = new SortByCumulativePrice(sortFilter);
            sortByRecentAveragePrice = new SortByRecentAveragePrice(sortFilter);
            sortByItemsOnSale = new SortByItemsOnSale(sortFilter);
            sortByRecentSales = new SortByRecentSales(sortFilter);
            sortByCountInInventory = new SortByCountInInventory(sortFilter);
        }

        public List<BitSkinsApi.Market.MarketItem> Sort(List<BitSkinsApi.Market.MarketItem> marketItems)
        {
            List<BitSkinsApi.Market.MarketItem> sortedMarketItems = marketItems;

            ConsoleLog.WriteInfo($"Start sort profitable items. Count before sort - {sortedMarketItems.Count}");

            sortedMarketItems = sortByTotalItems.Sort(sortedMarketItems);
            sortedMarketItems = sortByLowestPrice.Sort(sortedMarketItems);
            sortedMarketItems = sortByHighestPrice.Sort(sortedMarketItems);
            sortedMarketItems = sortByCumulativePrice.Sort(sortedMarketItems);
            sortedMarketItems = sortByRecentAveragePrice.Sort(sortedMarketItems);
            sortedMarketItems = sortByItemsOnSale.Sort(sortedMarketItems);
            sortedMarketItems = sortByRecentSales.Sort(sortedMarketItems);
            sortedMarketItems = sortByCountInInventory.Sort(sortedMarketItems);

            ConsoleLog.WriteInfo($"End sort profitable items. Count after sort - {sortedMarketItems.Count}");

            return sortedMarketItems;
        }
    }
}
