using System.Collections.Generic;
using BitSkinsBot.EventsLog;

namespace BitSkinsBot.FastMarketAnalize
{
    internal static class SortItems
    {
        private static ISortMethod sortByTotalItems;
        private static ISortMethod sortByLowestPrice;
        private static ISortMethod sortByHighestPrice;
        private static ISortMethod sortByCumulativePrice;
        private static ISortMethod sortByRecentAveragePrice;
        private static ISortMethod sortByItemsOnSale;
        private static ISortMethod sortByRecentSales;
        private static ISortMethod sortByCountInInventory;

        internal static List<BitSkinsApi.Market.MarketItem> Sort(List<BitSkinsApi.Market.MarketItem> marketItems, SortFilter searchFilter)
        {
            List<BitSkinsApi.Market.MarketItem> sortedMarketItems = marketItems;

            ConsoleLog.WriteInfo($"Start sort profitable items. Count before sort - {sortedMarketItems.Count}");

            sortByTotalItems = new SortByTotalItems(searchFilter);
            sortedMarketItems = sortByTotalItems.Sort(sortedMarketItems);

            sortByLowestPrice = new SortByLowestPrice(searchFilter);
            sortedMarketItems = sortByLowestPrice.Sort(sortedMarketItems);

            sortByHighestPrice = new SortByHighestPrice(searchFilter);
            sortedMarketItems = sortByHighestPrice.Sort(sortedMarketItems);

            sortByCumulativePrice = new SortByCumulativePrice(searchFilter);
            sortedMarketItems = sortByCumulativePrice.Sort(sortedMarketItems);

            sortByRecentAveragePrice = new SortByRecentAveragePrice(searchFilter);
            sortedMarketItems = sortByRecentAveragePrice.Sort(sortedMarketItems);

            sortByItemsOnSale = new SortByItemsOnSale(searchFilter);
            sortedMarketItems = sortByItemsOnSale.Sort(sortedMarketItems);

            sortByRecentSales = new SortByRecentSales(searchFilter);
            sortedMarketItems = sortByRecentSales.Sort(sortedMarketItems);

            sortByCountInInventory = new SortByCountInInventory(searchFilter);
            sortedMarketItems = sortByCountInInventory.Sort(sortedMarketItems);

            ConsoleLog.WriteInfo($"End sort profitable items. Count after sort - {sortedMarketItems.Count}");

            return sortedMarketItems;
        }
    }
}
