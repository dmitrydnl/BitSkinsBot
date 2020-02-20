using System.Collections.Generic;

namespace BitSkinsBot.FastMarketAnalize
{
    internal interface IRelistForSale
    {
        List<MarketItem> RelistItemsForSale(List<MarketItem> marketItems);
    }
}
