using System.Collections.Generic;
using BitSkinsBot.EventsLog;

namespace BitSkinsBot.FastMarketAnalize
{
    internal class SortByLowestPrice : ISortMethod
    {
        private SortFilter searchFilter;

        internal SortByLowestPrice(SortFilter searchFilter)
        {
            this.searchFilter = searchFilter;
        }

        public void Sort(List<BitSkinsApi.Market.MarketItem> marketItems)
        {
            if (marketItems == null || marketItems.Count == 0 || searchFilter == null)
            {
                return;
            }

            double? minLowestPrice = searchFilter.MinLowestPrice;
            double? maxLowestPrice = searchFilter.MaxLowestPrice;

            ConsoleLog.WriteInfo($"Start sort by lowest price. Count before sort  - {marketItems.Count}");
            ConsoleLog.StartProgress("Sort by lowest price");
            int done = 1;

            List<BitSkinsApi.Market.MarketItem> sortedMarketItems = new List<BitSkinsApi.Market.MarketItem>();
            foreach (BitSkinsApi.Market.MarketItem marketItem in marketItems)
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

            ConsoleLog.WriteInfo($"End sort by lowest price. Count after sort - {sortedMarketItems.Count}");

            marketItems = sortedMarketItems;
        }
    }
}
