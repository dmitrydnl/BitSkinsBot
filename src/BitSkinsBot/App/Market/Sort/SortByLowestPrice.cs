using System.Collections.Generic;
using BitSkinsBot.EventsLog;

namespace BitSkinsBot.Market.Sort
{
    internal class SortByLowestPrice : ISortMethod
    {
        private readonly SortFilter sortFilter;

        internal SortByLowestPrice(SortFilter sortFilter)
        {
            this.sortFilter = sortFilter;
        }

        public List<BitSkinsApi.Market.MarketItem> Sort(List<BitSkinsApi.Market.MarketItem> marketItems)
        {
            if (marketItems == null || marketItems.Count == 0 || sortFilter == null)
            {
                return new List<BitSkinsApi.Market.MarketItem>();
            }

            double? minLowestPrice = sortFilter.MinLowestPrice;
            double? maxLowestPrice = sortFilter.MaxLowestPrice;

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

            return sortedMarketItems;
        }
    }
}
