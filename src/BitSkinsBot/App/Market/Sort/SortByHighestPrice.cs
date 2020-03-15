using System.Collections.Generic;
using BitSkinsBot.EventsLog;

namespace BitSkinsBot.FastMarketAnalize
{
    internal class SortByHighestPrice : ISortMethod
    {
        private readonly SortFilter sortFilter;

        internal SortByHighestPrice(SortFilter sortFilter)
        {
            this.sortFilter = sortFilter;
        }

        public List<BitSkinsApi.Market.MarketItem> Sort(List<BitSkinsApi.Market.MarketItem> marketItems)
        {
            if (marketItems == null || marketItems.Count == 0 || sortFilter == null)
            {
                return new List<BitSkinsApi.Market.MarketItem>();
            }

            ConsoleLog.WriteInfo($"Start sort by highest price. Count before sort - {marketItems.Count}");
            ConsoleLog.StartProgress("Sort by highest price");
            int done = 1;

            List<BitSkinsApi.Market.MarketItem> sortedMarketItems = new List<BitSkinsApi.Market.MarketItem>();
            foreach (BitSkinsApi.Market.MarketItem marketItem in marketItems)
            {
                ConsoleLog.WriteProgress("Sort by highest price", done, marketItems.Count);
                done++;

                double lowestPrice = marketItem.LowestPrice;
                double highestPrice = marketItem.HighestPrice;

                double? minHighestPrice = sortFilter.MinHighestPricePercentFromLowestPrice == null ? null
                    : lowestPrice / 100 * sortFilter.MinHighestPricePercentFromLowestPrice;
                double? maxHighestPrice = sortFilter.MaxHighestPricePercentFromLowestPrice == null ? null
                    : lowestPrice / 100 * sortFilter.MaxHighestPricePercentFromLowestPrice;

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

            ConsoleLog.WriteInfo($"End sort by highest price. Count after sort - {sortedMarketItems.Count}");

            return sortedMarketItems;
        }
    }
}
