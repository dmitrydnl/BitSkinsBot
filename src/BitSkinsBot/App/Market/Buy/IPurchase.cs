using System.Collections.Generic;

namespace BitSkinsBot.Market.Buy
{
    internal interface IPurchase
    {
        List<MarketItem> PurchaseItems(List<MarketItem> marketItems);
    }
}
