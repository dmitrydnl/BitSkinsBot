using System.Collections.Generic;

namespace BitSkinsBot.FastMarketAnalize
{
    internal interface ISortMethod
    {
        void Sort(List<BitSkinsApi.Market.MarketItem> marketItems, SortFilter searchFilter);
    }
}
