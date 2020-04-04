using System.Collections.Generic;

namespace BitSkinsBot.Market.Buy
{
    internal interface ICheckSoldItems
    {
        List<MarketItem> GetSoldItems(List<MarketItem> marketItems);
    }
}
