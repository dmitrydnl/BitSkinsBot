using System.Collections.Generic;

namespace BitSkinsBot.Market.Sort
{
    internal interface ISortMethod
    {
        List<BitSkinsApi.Market.MarketItem> Sort(List<BitSkinsApi.Market.MarketItem> marketItems);
    }
}
