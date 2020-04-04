using System.Collections.Generic;
using BitSkinsBot.EventsLog;

namespace BitSkinsBot.Market.Sort
{
    internal class SortByTotalItems : ISortMethod
    {
        private readonly SortFilter sortFilter;

        internal SortByTotalItems(SortFilter sortFilter)
        {
            this.sortFilter = sortFilter;
        }

        public List<BitSkinsApi.Market.MarketItem> Sort(List<BitSkinsApi.Market.MarketItem> marketItems)
        {
            if (marketItems == null || marketItems.Count == 0 || sortFilter == null)
            {
                return new List<BitSkinsApi.Market.MarketItem>();
            }

            int? minTotalItems = sortFilter.MinTotalItems;
            int? maxTotalItems = sortFilter.MaxTotalItems;

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
