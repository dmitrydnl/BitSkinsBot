using System.Collections.Generic;
using BitSkinsBot.EventsLog;

namespace BitSkinsBot.Market.Sort
{
    internal class SortByCumulativePrice : ISortMethod
    {
        private readonly SortFilter sortFilter;

        internal SortByCumulativePrice(SortFilter sortFilter)
        {
            this.sortFilter = sortFilter;
        }

        public List<BitSkinsApi.Market.MarketItem> Sort(List<BitSkinsApi.Market.MarketItem> marketItems)
        {
            if (marketItems == null || marketItems.Count == 0 || sortFilter == null)
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

                double? minCumulativePrice = sortFilter.MinCumulativePricePercentFromLowestCumulativePrice == null ? null
                    : lowestCumulativePrice / 100 * sortFilter.MinCumulativePricePercentFromLowestCumulativePrice;
                double? maxCumulativePrice = sortFilter.MaxCumulativePricePercentFromLowestCumulativePrice == null ? null
                    : lowestCumulativePrice / 100 * sortFilter.MaxCumulativePricePercentFromLowestCumulativePrice;

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
