using System;
using System.Linq;
using System.Collections.Generic;
using BitSkinsApi.Market;
using BitSkinsApi.Inventory;
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

            sortedMarketItems = SortByRecentSales(sortedMarketItems, searchFilter);
            sortedMarketItems = SortByCountInInventory(sortedMarketItems, searchFilter);

            ConsoleLog.WriteInfo($"End sort profitable items. Count after sort - {sortedMarketItems.Count}");

            return sortedMarketItems;
        }

        private static List<BitSkinsApi.Market.MarketItem> SortByRecentSales(List<BitSkinsApi.Market.MarketItem> marketItems, SortFilter searchFilter)
        {
            if (marketItems == null || marketItems.Count == 0)
            {
                return new List<BitSkinsApi.Market.MarketItem>();
            }

            int? minCountOfSalesInLastWeek = searchFilter.MinCountOfSalesInLastWeek;
            int? maxCountOfSalesInLastWeek = searchFilter.MaxCountOfSalesInLastWeek;

            ConsoleLog.WriteInfo($"Start sort by recent sales. Count before sort - {marketItems.Count}");
            ConsoleLog.StartProgress("Sort by recent sales");
            int done = 1;

            List<BitSkinsApi.Market.MarketItem> sortedMarketItems = new List<BitSkinsApi.Market.MarketItem>();
            foreach (BitSkinsApi.Market.MarketItem marketItem in marketItems)
            {
                ConsoleLog.WriteProgress("Sort by recent sales", done, marketItems.Count);
                done++;

                string marketHashName = marketItem.MarketHashName;
                double lowestPrice = marketItem.LowestPrice;
                List<ItemRecentSale> itemRecentSales = GetItemRecentSales(searchFilter, marketHashName);
                int countOfSalesInLastWeek = GetCountOfSalesInLastWeek(itemRecentSales);
                double averagePriceInLastWeek = GetAveragePriceInLastWeek(itemRecentSales);

                double? minAveragePriceInLastWeek = searchFilter.MinAveragePriceInLastWeekPercentFromLowestPrice == null ? null
                    : lowestPrice / 100 * searchFilter.MinAveragePriceInLastWeekPercentFromLowestPrice;
                double? maxAveragePriceInLastWeek = searchFilter.MaxAveragePriceInLastWeekPercentFromLowestPrice == null ? null
                    : lowestPrice / 100 * searchFilter.MaxAveragePriceInLastWeekPercentFromLowestPrice;

                if (minCountOfSalesInLastWeek != null && countOfSalesInLastWeek < minCountOfSalesInLastWeek)
                {
                    continue;
                }
                if (maxCountOfSalesInLastWeek != null && countOfSalesInLastWeek > maxCountOfSalesInLastWeek)
                {
                    continue;
                }
                if (minAveragePriceInLastWeek != null && averagePriceInLastWeek < minAveragePriceInLastWeek)
                {
                    continue;
                }
                if (maxAveragePriceInLastWeek != null && averagePriceInLastWeek > maxAveragePriceInLastWeek)
                {
                    continue;
                }

                sortedMarketItems.Add(marketItem);
            }

            ConsoleLog.WriteInfo($"End sort by recent sales. Count after sort - {sortedMarketItems.Count}");

            return sortedMarketItems;
        }

        private static List<BitSkinsApi.Market.MarketItem> SortByCountInInventory(List<BitSkinsApi.Market.MarketItem> marketItems, SortFilter searchFilter)
        {
            if (marketItems == null || marketItems.Count == 0)
            {
                return new List<BitSkinsApi.Market.MarketItem>();
            }

            int? minItemsOnSale = searchFilter.MinItemsOnSale;
            int? maxItemsOnSale = searchFilter.MaxItemsOnSale;
            BitSkinsInventory bitSkinsInventory = Inventories.GetAccountInventory(searchFilter.App, 1).BitSkinsInventory;

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


        private static List<ItemRecentSale> GetItemRecentSales(SortFilter searchFilter, string marketHashName)
        {
            List<ItemRecentSale> itemRecentSales = new List<ItemRecentSale>();
            for (int i = 1; i <= 5; i++)
            {
                List<ItemRecentSale> items = RecentSaleInfo.GetRecentSaleInfo(searchFilter.App, marketHashName, i);
                foreach (ItemRecentSale item in items)
                {
                    itemRecentSales.Add(item);
                }
            }

            return itemRecentSales;
        }

        private static int GetCountOfSalesInLastWeek(List<ItemRecentSale> itemRecentSales)
        {
            int countOfSalesInLastWeek = 0;
            foreach (ItemRecentSale recentSale in itemRecentSales)
            {
                if (recentSale.SoldAt >= DateTime.Now.AddDays(-7))
                {
                    countOfSalesInLastWeek++;
                }
            }

            return countOfSalesInLastWeek;
        }

        private static double GetAveragePriceInLastWeek(List<ItemRecentSale> itemRecentSales)
        {
            double averagePriceInLastWeek = 0;
            List<double> itemPriceOfSales = new List<double>();
            foreach (ItemRecentSale recentSale in itemRecentSales)
            {
                if (recentSale.SoldAt >= DateTime.Now.AddDays(-7))
                {
                    itemPriceOfSales.Add(recentSale.Price);
                }
            }

            if (itemPriceOfSales.Count > 0)
            {
                averagePriceInLastWeek = itemPriceOfSales.Average();
            }

            return averagePriceInLastWeek;
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
