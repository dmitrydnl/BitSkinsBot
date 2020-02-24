using System.Collections.Generic;

namespace BitSkinsBot.FastMarketAnalize
{
    internal interface ICheckSoldItems
    {
        List<MarketItem> GetSoldItems(List<MarketItem> marketItems);
    }
}
