using System;
using System.Collections.Generic;
using BitSkinsApi.Market;
using BitSkinsApi.Balance;

namespace BitSkinsBot.FastMarketAnalize
{
    internal class ProfitableItems
    {
        private readonly Filter currentFilter;

        internal ProfitableItems()
        {
            Filter filter = new Filter
            {
                App = AppId.AppName.CounterStrikGlobalOffensive,
                MinTotalItems = 10,
                MaxTotalItems = null,
                MinLowestPricePercentFromBalance = 2,
                MaxLowestPricePercentFromBalance = 10,
                MinHighestPricePercentFromLowestPrice = 130,
                MaxHighestPricePercentFromLowestPrice = null,
                MinCumulativePricePercentFromLowestCumulativePrice = 130,
                MaxCumulativePricePercentFromLowestCumulativePrice = null,
                MinRecentAveragePricePercentFromLowestPrice = 130,
                MaxRecentAveragePricePercentFromLowestPrice = null
            };

            currentFilter = filter;
        }

        internal void GetProfitableItems()
        {
            List<MarketItem> marketItems = GetMarketItems();
            Console.WriteLine(marketItems.Count);

            marketItems = SortByTotalItems(marketItems);
            Console.WriteLine(marketItems.Count);

            marketItems = SortByLowestPrice(marketItems);
            Console.WriteLine(marketItems.Count);

            marketItems = SortByHighestPrice(marketItems);
            Console.WriteLine(marketItems.Count);

            marketItems = SortByCumulativePrice(marketItems);
            Console.WriteLine(marketItems.Count);

            marketItems = SortByRecentAveragePrice(marketItems);
            Console.WriteLine(marketItems.Count);
        }

        private double GetAvailableBalance()
        {
            double availableBalance = CurrentBalance.GetAccountBalance().AvailableBalance;
            return availableBalance;
        }

        private List<MarketItem> GetMarketItems()
        {
            List<MarketItem> marketItems = MarketData.GetMarketData(currentFilter.App);
            return marketItems;
        }

        private List<MarketItem> SortByTotalItems(List<MarketItem> marketItems)
        {
            List<MarketItem> sortMarketItems = new List<MarketItem>();
            foreach (MarketItem marketItem in marketItems)
            {
                int totalItems = marketItem.TotalItems;
                int? minTotalItems = currentFilter.MinTotalItems;
                int? maxTotalItems = currentFilter.MaxTotalItems;

                if (minTotalItems == null || totalItems >= minTotalItems)
                {
                    if (maxTotalItems == null || totalItems <= maxTotalItems)
                    {
                        sortMarketItems.Add(marketItem);
                    }
                }
            }

            return sortMarketItems;
        }

        private List<MarketItem> SortByLowestPrice(List<MarketItem> marketItems)
        {
            double availableBalance = GetAvailableBalance();

            List<MarketItem> sortMarketItems = new List<MarketItem>();
            foreach (MarketItem marketItem in marketItems)
            {
                double lowestPrice = marketItem.LowestPrice;
                double? minLowestPrice = currentFilter.MinLowestPricePercentFromBalance == null ? 
                    null : availableBalance / 100 * currentFilter.MinLowestPricePercentFromBalance;
                double? maxLowestPrice = currentFilter.MaxLowestPricePercentFromBalance == null ? 
                    null : availableBalance / 100 * currentFilter.MaxLowestPricePercentFromBalance;

                if (minLowestPrice == null || lowestPrice >= minLowestPrice)
                {
                    if (maxLowestPrice == null || lowestPrice <= maxLowestPrice)
                    {
                        sortMarketItems.Add(marketItem);
                    }
                }
            }

            return sortMarketItems;
        }

        private List<MarketItem> SortByHighestPrice(List<MarketItem> marketItems)
        {
            List<MarketItem> sortMarketItems = new List<MarketItem>();
            foreach (MarketItem marketItem in marketItems)
            {
                double lowestPrice = marketItem.LowestPrice;
                double highestPrice = marketItem.HighestPrice;
                double? minHighestPrice = currentFilter.MinHighestPricePercentFromLowestPrice == null ?
                    null : lowestPrice / 100 * currentFilter.MinHighestPricePercentFromLowestPrice;
                double? maxHighestPrice = currentFilter.MaxHighestPricePercentFromLowestPrice == null ?
                    null : lowestPrice / 100 * currentFilter.MaxHighestPricePercentFromLowestPrice;

                if (minHighestPrice == null || highestPrice >= minHighestPrice)
                {
                    if (maxHighestPrice == null || highestPrice <= maxHighestPrice)
                    {
                        sortMarketItems.Add(marketItem);
                    }
                }
            }

            return sortMarketItems;
        }

        private List<MarketItem> SortByCumulativePrice(List<MarketItem> marketItems)
        {
            List<MarketItem> sortMarketItems = new List<MarketItem>();
            foreach (MarketItem marketItem in marketItems)
            {
                int totalItems = marketItem.TotalItems;
                double lowestPrice = marketItem.LowestPrice;
                double cumulativePrice = marketItem.CumulativePrice;
                double? minCumulativePrice = currentFilter.MinCumulativePricePercentFromLowestCumulativePrice == null ?
                    null : totalItems * lowestPrice / 100 * currentFilter.MinCumulativePricePercentFromLowestCumulativePrice;
                double? maxCumulativePrice = currentFilter.MaxCumulativePricePercentFromLowestCumulativePrice == null ?
                    null : totalItems * lowestPrice / 100 * currentFilter.MaxCumulativePricePercentFromLowestCumulativePrice;

                if (minCumulativePrice == null || cumulativePrice >= minCumulativePrice)
                {
                    if (maxCumulativePrice == null || cumulativePrice <= maxCumulativePrice)
                    {
                        sortMarketItems.Add(marketItem);
                    }
                }
            }

            return sortMarketItems;
        }

        private List<MarketItem> SortByRecentAveragePrice(List<MarketItem> marketItems)
        {
            List<MarketItem> sortMarketItems = new List<MarketItem>();
            foreach (MarketItem marketItem in marketItems)
            {
                double lowestPrice = marketItem.LowestPrice;
                double recentAveragePrice = marketItem.RecentAveragePrice;
                double? minRecentAveragePrice = currentFilter.MinRecentAveragePricePercentFromLowestPrice == null ?
                    null : lowestPrice / 100 * currentFilter.MinRecentAveragePricePercentFromLowestPrice;
                double? maxRecentAveragePrice = currentFilter.MaxRecentAveragePricePercentFromLowestPrice == null ?
                    null : lowestPrice / 100 * currentFilter.MaxRecentAveragePricePercentFromLowestPrice;

                if (minRecentAveragePrice == null || recentAveragePrice >= minRecentAveragePrice)
                {
                    if (maxRecentAveragePrice == null || recentAveragePrice <= maxRecentAveragePrice)
                    {
                        sortMarketItems.Add(marketItem);
                    }
                }
            }

            return sortMarketItems;
        }
    }
}
