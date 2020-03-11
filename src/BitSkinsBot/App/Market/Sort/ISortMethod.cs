using System.Collections.Generic;

namespace BitSkinsBot.FastMarketAnalize
{
    internal interface ISortMethod
    {
        List<BitSkinsApi.Market.MarketItem> Sort(List<BitSkinsApi.Market.MarketItem> marketItems);
    }
}
