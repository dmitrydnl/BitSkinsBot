using System;
using System.Linq;
using System.Collections.Generic;
using BitSkinsApi.Market;
using BitSkinsBot.Log;

namespace BitSkinsBot.FastMarketAnalize
{
    internal static class ProfitableItems
    {
        internal static List<ProfitableMarketItem> GetProfitableItems(SearchFilter searchFilter)
        {
            ConsoleLog.WriteInfo("Start get profitable items");

            List<MarketItem> marketItems = GetMarketItems(searchFilter);

            ConsoleLog.WriteInfo($"Market items - {marketItems.Count}");

            marketItems = SortProfitableItems(marketItems, searchFilter);
            List<ProfitableMarketItem> profitableMarketItems = GetProfitableMarketItems(marketItems, searchFilter);

            ConsoleLog.WriteInfo("End get profitable items");

            return profitableMarketItems;
        }


        private static List<MarketItem> GetMarketItems(SearchFilter searchFilter)
        {
            List<MarketItem> marketItems = MarketData.GetMarketData(searchFilter.App);
            return marketItems;
        }

        private static List<ItemRecentSale> GetItemRecentSales(SearchFilter searchFilter, string marketHashName)
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

        private static List<ItemOnSale> GetItemsOnSale(SearchFilter searchFilter, string marketHashName)
        {
            List<ItemOnSale> itemsOnSale = InventoryOnSale.GetInventoryOnSale(searchFilter.App, 1, marketHashName, 0, 0, InventoryOnSale.SortBy.Price,
                    InventoryOnSale.SortOrder.Asc, InventoryOnSale.ThreeChoices.NotImportant, InventoryOnSale.ThreeChoices.NotImportant,
                    InventoryOnSale.ThreeChoices.NotImportant, InventoryOnSale.ResultsPerPage.R30, InventoryOnSale.ThreeChoices.NotImportant);
            return itemsOnSale;
        }

        private static List<ProfitableMarketItem> GetProfitableMarketItems(List<MarketItem> marketItems, SearchFilter searchFilter)
        {
            if (marketItems == null || marketItems.Count == 0)
            {
                return new List<ProfitableMarketItem>();
            }

            ConsoleLog.WriteInfo($"Start get profitable market items. Count - {marketItems.Count}");
            ConsoleLog.StartProgeress("Get profitable market items");
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

            ConsoleLog.WriteInfo($"End get profitable market items. Count - {marketItems.Count}");

            return profitableMarketItems;
        }


        private static List<MarketItem> SortProfitableItems(List<MarketItem> marketItems, SearchFilter searchFilter)
        {
            List<MarketItem> sortedMarketItems = marketItems;

            ConsoleLog.WriteInfo("Start sort profitable items");

            sortedMarketItems = SortByTotalItems(sortedMarketItems, searchFilter);
            sortedMarketItems = SortByLowestPrice(sortedMarketItems, searchFilter);
            sortedMarketItems = SortByHighestPrice(sortedMarketItems, searchFilter);
            sortedMarketItems = SortByCumulativePrice(sortedMarketItems, searchFilter);
            sortedMarketItems = SortByRecentAveragePrice(sortedMarketItems, searchFilter);
            sortedMarketItems = SortByRecentSales(sortedMarketItems, searchFilter);
            sortedMarketItems = SortByItemsOnSale(sortedMarketItems, searchFilter);

            ConsoleLog.WriteInfo("End sort profitable items");

            return sortedMarketItems;
        }

        private static List<MarketItem> SortByTotalItems(List<MarketItem> marketItems, SearchFilter searchFilter)
        {
            if (marketItems == null || marketItems.Count == 0)
            {
                return new List<MarketItem>();
            }

            int? minTotalItems = searchFilter.MinTotalItems;
            int? maxTotalItems = searchFilter.MaxTotalItems;

            ConsoleLog.WriteInfo($"Start sort by total items. Count - {marketItems.Count}");
            ConsoleLog.StartProgeress("Sort by total items");
            int done = 1;

            List<MarketItem> sortedMarketItems = new List<MarketItem>();
            foreach (MarketItem marketItem in marketItems)
            {
                ConsoleLog.WriteProgress("Sort by total items", done, marketItems.Count);
                done++;

                int totalItems = marketItem.TotalItems;

                if (minTotalItems != null && totalItems < minTotalItems)
                {
                    continue;
                }
                if (maxTotalItems != null && totalItems > maxTotalItems)
                {
                    continue;
                }

                sortedMarketItems.Add(marketItem);
            }

            ConsoleLog.WriteInfo($"End sort by total items. Count - {sortedMarketItems.Count}");

            return sortedMarketItems;
        }

        private static List<MarketItem> SortByLowestPrice(List<MarketItem> marketItems, SearchFilter searchFilter)
        {
            if (marketItems == null || marketItems.Count == 0)
            {
                return new List<MarketItem>();
            }

            double? minLowestPrice = searchFilter.MinLowestPrice;
            double? maxLowestPrice = searchFilter.MaxLowestPrice;

            ConsoleLog.WriteInfo($"Start sort by lowest price. Count - {marketItems.Count}");
            ConsoleLog.StartProgeress("Sort by lowest price");
            int done = 1;

            List<MarketItem> sortedMarketItems = new List<MarketItem>();
            foreach (MarketItem marketItem in marketItems)
            {
                ConsoleLog.WriteProgress("Sort by lowest price", done, marketItems.Count);
                done++;

                double lowestPrice = marketItem.LowestPrice;

                if (minLowestPrice != null && lowestPrice < minLowestPrice)
                {
                    continue;
                }
                if (maxLowestPrice != null && lowestPrice > maxLowestPrice)
                {
                    continue;
                }

                sortedMarketItems.Add(marketItem);
            }

            ConsoleLog.WriteInfo($"End sort by lowest price. Count - {sortedMarketItems.Count}");

            return sortedMarketItems;
        }

        private static List<MarketItem> SortByHighestPrice(List<MarketItem> marketItems, SearchFilter searchFilter)
        {
            if (marketItems == null || marketItems.Count == 0)
            {
                return new List<MarketItem>();
            }

            ConsoleLog.WriteInfo($"Start sort by highest price. Count - {marketItems.Count}");
            ConsoleLog.StartProgeress("Sort by highest price");
            int done = 1;

            List<MarketItem> sortedMarketItems = new List<MarketItem>();
            foreach (MarketItem marketItem in marketItems)
            {
                ConsoleLog.WriteProgress("Sort by highest price", done, marketItems.Count);
                done++;

                double lowestPrice = marketItem.LowestPrice;
                double highestPrice = marketItem.HighestPrice;

                double? minHighestPrice = searchFilter.MinHighestPricePercentFromLowestPrice == null ? null
                    : lowestPrice / 100 * searchFilter.MinHighestPricePercentFromLowestPrice;
                double? maxHighestPrice = searchFilter.MaxHighestPricePercentFromLowestPrice == null ? null
                    : lowestPrice / 100 * searchFilter.MaxHighestPricePercentFromLowestPrice;

                if (minHighestPrice != null && highestPrice < minHighestPrice)
                {
                    continue;
                }
                if (maxHighestPrice != null && highestPrice > maxHighestPrice)
                {
                    continue;
                }

                sortedMarketItems.Add(marketItem);
            }

            ConsoleLog.WriteInfo($"End sort by highest price. Count - {sortedMarketItems.Count}");

            return sortedMarketItems;
        }

        private static List<MarketItem> SortByCumulativePrice(List<MarketItem> marketItems, SearchFilter searchFilter)
        {
            if (marketItems == null || marketItems.Count == 0)
            {
                return new List<MarketItem>();
            }

            ConsoleLog.WriteInfo($"Start sort by cumulative price. Count - {marketItems.Count}");
            ConsoleLog.StartProgeress("Sort by cumulative price");
            int done = 1;

            List<MarketItem> sortedMarketItems = new List<MarketItem>();
            foreach (MarketItem marketItem in marketItems)
            {
                ConsoleLog.WriteProgress("Sort by cumulative price", done, marketItems.Count);
                done++;

                int totalItems = marketItem.TotalItems;
                double lowestPrice = marketItem.LowestPrice;
                double cumulativePrice = marketItem.CumulativePrice;
                double lowestCumulativePrice = totalItems * lowestPrice;

                double? minCumulativePrice = searchFilter.MinCumulativePricePercentFromLowestCumulativePrice == null ? null 
                    : lowestCumulativePrice / 100 * searchFilter.MinCumulativePricePercentFromLowestCumulativePrice;
                double? maxCumulativePrice = searchFilter.MaxCumulativePricePercentFromLowestCumulativePrice == null ? null 
                    : lowestCumulativePrice / 100 * searchFilter.MaxCumulativePricePercentFromLowestCumulativePrice;

                if (minCumulativePrice != null && cumulativePrice < minCumulativePrice)
                {
                    continue;
                }
                if (maxCumulativePrice != null && cumulativePrice > maxCumulativePrice)
                {
                    continue;
                }

                sortedMarketItems.Add(marketItem);
            }

            ConsoleLog.WriteInfo($"End sort by cumulative price. Count - {sortedMarketItems.Count}");

            return sortedMarketItems;
        }

        private static List<MarketItem> SortByRecentAveragePrice(List<MarketItem> marketItems, SearchFilter searchFilter)
        {
            if (marketItems == null || marketItems.Count == 0)
            {
                return new List<MarketItem>();
            }

            ConsoleLog.WriteInfo($"Start sort by recent average price. Count - {marketItems.Count}");
            ConsoleLog.StartProgeress("Sort by recent average price");
            int done = 1;

            List<MarketItem> sortedMarketItems = new List<MarketItem>();
            foreach (MarketItem marketItem in marketItems)
            {
                ConsoleLog.WriteProgress("Sort by recent average price", done, marketItems.Count);
                done++;

                double lowestPrice = marketItem.LowestPrice;
                double recentAveragePrice = marketItem.RecentAveragePrice;

                double? minRecentAveragePrice = searchFilter.MinRecentAveragePricePercentFromLowestPrice == null ? null 
                    : lowestPrice / 100 * searchFilter.MinRecentAveragePricePercentFromLowestPrice;
                double? maxRecentAveragePrice = searchFilter.MaxRecentAveragePricePercentFromLowestPrice == null ? null 
                    : lowestPrice / 100 * searchFilter.MaxRecentAveragePricePercentFromLowestPrice;

                if (minRecentAveragePrice != null && recentAveragePrice < minRecentAveragePrice)
                {
                    continue;
                }
                if (maxRecentAveragePrice != null && recentAveragePrice > maxRecentAveragePrice)
                {
                    continue;
                }

                sortedMarketItems.Add(marketItem);
            }

            ConsoleLog.WriteInfo($"End sort by recent average price. Count - {sortedMarketItems.Count}");

            return sortedMarketItems;
        }

        private static List<MarketItem> SortByRecentSales(List<MarketItem> marketItems, SearchFilter searchFilter)
        {
            if (marketItems == null || marketItems.Count == 0)
            {
                return new List<MarketItem>();
            }

            int? minCountOfSalesInLastWeek = searchFilter.MinCountOfSalesInLastWeek;
            int? maxCountOfSalesInLastWeek = searchFilter.MaxCountOfSalesInLastWeek;

            ConsoleLog.WriteInfo($"Start sort by recent sales. Count - {marketItems.Count}");
            ConsoleLog.StartProgeress("Sort by recent sales");
            int done = 1;

            List<MarketItem> sortedMarketItems = new List<MarketItem>();
            foreach (MarketItem marketItem in marketItems)
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

            ConsoleLog.WriteInfo($"End sort by recent sales. Count - {sortedMarketItems.Count}");

            return sortedMarketItems;
        }

        private static List<MarketItem> SortByItemsOnSale(List<MarketItem> marketItems, SearchFilter searchFilter)
        {
            if (marketItems == null || marketItems.Count == 0)
            {
                return new List<MarketItem>();
            }

            ConsoleLog.WriteInfo($"Start sort by items on sale. Count - {marketItems.Count}");
            ConsoleLog.StartProgeress("Sort by items on sale");
            int done = 1;

            List<MarketItem> sortedMarketItems = new List<MarketItem>();
            foreach (MarketItem marketItem in marketItems)
            {
                ConsoleLog.WriteProgress("Sort by items on sale", done, marketItems.Count);
                done++;

                string marketHashName = marketItem.MarketHashName;
                List<ItemOnSale> itemsOnSale = GetItemsOnSale(searchFilter, marketHashName);
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

            ConsoleLog.WriteInfo($"End sort by items on sale. Count - {sortedMarketItems.Count}");

            return sortedMarketItems;
        }
    }
}
