using System.Collections.Generic;

namespace BitSkinsBot.FastMarketAnalize
{
    internal interface IPurchase
    {
        List<MarketItem> PurchaseItems(List<MarketItem> marketItems);
    }
}
