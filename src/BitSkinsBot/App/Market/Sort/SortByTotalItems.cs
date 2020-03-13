using System.Collections.Generic;
using BitSkinsBot.EventsLog;

namespace BitSkinsBot.FastMarketAnalize
{
    internal class SortByTotalItems : ISortMethod
    {
        private readonly SortFilter searchFilter;

        internal SortByTotalItems(SortFilter searchFilter)
        {
            this.searchFilter = searchFilter;
        }

        public List<BitSkinsApi.Market.MarketItem> Sort(List<BitSkinsApi.Market.MarketItem> marketItems)
        {
            if (marketItems == null || marketItems.Count == 0 || searchFilter == null)
            {
                return new List<BitSkinsApi.Market.MarketItem>();
            }

            int? minTotalItems = searchFilter.MinTotalItems;
            int? maxTotalItems = searchFilter.MaxTotalItems;

            ConsoleLog.WriteInfo($"Start sort by total items. Count before sort - {marketItems.Count}");
            ConsoleLog.StartProgress("Sort by total items");
            int done = 1;

            List<BitSkinsApi.Market.MarketItem> sortedMarketItems = new List<BitSkinsApi.Market.MarketItem>();
            foreach (BitSkinsApi.Market.MarketItem marketItem in marketItems)
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

            ConsoleLog.WriteInfo($"End sort by total items. Count after sort - {sortedMarketItems.Count}");

            return sortedMarketItems;
        }
    }
}
