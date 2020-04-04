using System.Collections.Generic;

namespace BitSkinsBot.Market.Sale
{
    internal interface IRelistForSale
    {
        List<MarketItem> RelistItemsForSale(List<MarketItem> marketItems);
    }
}
