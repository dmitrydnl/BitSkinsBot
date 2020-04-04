using System;
using System.Linq;
using System.Collections.Generic;
using BitSkinsBot.EventsLog;
using BitSkinsApi.Market;

namespace BitSkinsBot.Market.Sort
{
    internal class SortByRecentSales : ISortMethod
    {
        private readonly SortFilter sortFilter;

        internal SortByRecentSales(SortFilter sortFilter)
        {
            this.sortFilter = sortFilter;
        }

        public List<BitSkinsApi.Market.MarketItem> Sort(List<BitSkinsApi.Market.MarketItem> marketItems)
        {
            if (marketItems == null || marketItems.Count == 0 || sortFilter == null)
            {
                return new List<BitSkinsApi.Market.MarketItem>();
            }

            int? minCountOfSalesInLastWeek = sortFilter.MinCountOfSalesInLastWeek;
            int? maxCountOfSalesInLastWeek = sortFilter.MaxCountOfSalesInLastWeek;

            ConsoleLog.WriteInfo($"Start sort by recent sales. Count before sort - {marketItems.Count}");
            ConsoleLog.StartProgress("Sort by recent sales");
            int done = 1;

            List<BitSkinsApi.Market.MarketItem> sortedMarketItems = new List<BitSkinsApi.Market.MarketItem>();
            foreach (BitSkinsApi.Market.MarketItem marketItem in marketItems)
            {
                ConsoleLog.WriteProgress("Sort by recent sales", done, marketItems.Count);
                done++;

                double lowestPrice = marketItem.LowestPrice;
                List<ItemRecentSale> itemRecentSales = GetItemRecentSales(marketItem.MarketHashName);
                int countOfSalesInLastWeek = GetCountOfSalesInLastWeek(itemRecentSales);
                double averagePriceInLastWeek = GetAveragePriceInLastWeek(itemRecentSales);

                double? minAveragePriceInLastWeek = sortFilter.MinAveragePriceInLastWeekPercentFromLowestPrice == null ? null
                    : lowestPrice / 100 * sortFilter.MinAveragePriceInLastWeekPercentFromLowestPrice;
                double? maxAveragePriceInLastWeek = sortFilter.MaxAveragePriceInLastWeekPercentFromLowestPrice == null ? null
                    : lowestPrice / 100 * sortFilter.MaxAveragePriceInLastWeekPercentFromLowestPrice;

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

        private List<ItemRecentSale> GetItemRecentSales(string marketHashName)
        {
            List<ItemRecentSale> itemRecentSales = new List<ItemRecentSale>();
            for (int i = 1; i <= 5; i++)
            {
                List<ItemRecentSale> items = RecentSaleInfo.GetRecentSaleInfo(sortFilter.App, marketHashName, i);
                foreach (ItemRecentSale item in items)
                {
                    itemRecentSales.Add(item);
                }
            }

            return itemRecentSales;
        }

        private int GetCountOfSalesInLastWeek(List<ItemRecentSale> itemRecentSales)
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

        private double GetAveragePriceInLastWeek(List<ItemRecentSale> itemRecentSales)
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
    }
}
