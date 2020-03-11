using System.Collections.Generic;
using BitSkinsBot.EventsLog;

namespace BitSkinsBot.FastMarketAnalize
{
    internal class SortByCumulativePrice : ISortMethod
    {
        private SortFilter searchFilter;

        internal SortByCumulativePrice(SortFilter searchFilter)
        {
            this.searchFilter = searchFilter;
        }

        public List<BitSkinsApi.Market.MarketItem> Sort(List<BitSkinsApi.Market.MarketItem> marketItems)
        {
            if (marketItems == null || marketItems.Count == 0 || searchFilter == null)
            {
                return new List<BitSkinsApi.Market.MarketItem>();
            }

            ConsoleLog.WriteInfo($"Start sort by cumulative price. Count before sort - {marketItems.Count}");
            ConsoleLog.StartProgress("Sort by cumulative price");
            int done = 1;

            List<BitSkinsApi.Market.MarketItem> sortedMarketItems = new List<BitSkinsApi.Market.MarketItem>();
            foreach (BitSkinsApi.Market.MarketItem marketItem in marketItems)
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

            ConsoleLog.WriteInfo($"End sort by cumulative price. Count after sort  - {sortedMarketItems.Count}");

            return sortedMarketItems;
        }
    }
}
