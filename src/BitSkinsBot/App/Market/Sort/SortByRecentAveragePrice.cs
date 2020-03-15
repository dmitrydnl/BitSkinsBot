using System.Collections.Generic;
using BitSkinsBot.EventsLog;

namespace BitSkinsBot.FastMarketAnalize
{
    internal class SortByRecentAveragePrice : ISortMethod
    {
        private readonly SortFilter sortFilter;

        internal SortByRecentAveragePrice(SortFilter sortFilter)
        {
            this.sortFilter = sortFilter;
        }

        public List<BitSkinsApi.Market.MarketItem> Sort(List<BitSkinsApi.Market.MarketItem> marketItems)
        {
            if (marketItems == null || marketItems.Count == 0 || sortFilter == null)
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

                double? minRecentAveragePrice = sortFilter.MinRecentAveragePricePercentFromLowestPrice == null ? null
                    : lowestPrice / 100 * sortFilter.MinRecentAveragePricePercentFromLowestPrice;
                double? maxRecentAveragePrice = sortFilter.MaxRecentAveragePricePercentFromLowestPrice == null ? null
                    : lowestPrice / 100 * sortFilter.MaxRecentAveragePricePercentFromLowestPrice;

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
