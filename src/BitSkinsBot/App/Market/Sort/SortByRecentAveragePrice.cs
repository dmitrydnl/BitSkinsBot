using System.Collections.Generic;
using BitSkinsBot.EventsLog;

namespace BitSkinsBot.FastMarketAnalize
{
    internal class SortByRecentAveragePrice : ISortMethod
    {
        private readonly SortFilter searchFilter;

        internal SortByRecentAveragePrice(SortFilter searchFilter)
        {
            this.searchFilter = searchFilter;
        }

        public List<BitSkinsApi.Market.MarketItem> Sort(List<BitSkinsApi.Market.MarketItem> marketItems)
        {
            if (marketItems == null || marketItems.Count == 0 || searchFilter == null)
            {
                return new List<BitSkinsApi.Market.MarketItem>();
            }

            ConsoleLog.WriteInfo($"Start sort by recent average price. Count before sort - {marketItems.Count}");
            ConsoleLog.StartProgress("Sort by recent average price");
            int done = 1;

            List<BitSkinsApi.Market.MarketItem> sortedMarketItems = new List<BitSkinsApi.Market.MarketItem>();
            foreach (BitSkinsApi.Market.MarketItem marketItem in marketItems)
            {
                ConsoleLog.WriteProgress("Sort by recent average price", done, marketItems.Count);
                done++;

                double lowestPrice = marketItem.LowestPrice;
                double? recentAveragePrice = marketItem.RecentAveragePrice;
                if (recentAveragePrice == null)
                {
                    continue;
                }

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

            ConsoleLog.WriteInfo($"End sort by recent average price. Count after sort - {sortedMarketItems.Count}");

            return sortedMarketItems;
        }
    }
}
