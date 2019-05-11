using System;
using BitSkinsApi.Market;

namespace BitSkinsBot.FastMarketAnalize
{
    internal class ProfitableMarketItem
    {
        internal AppId.AppName App { get; set; }
        internal string Name { get; set; }
        internal string Id { get; set; }
        internal double BuyPrice { get; set; }
        internal double SellPrice { get; set; }
        internal DateTime BuyDate { get; set; }
        internal DateTime OfferedForSaleDate { get; set; }
        internal DateTime SaleDate { get; set; }
        internal DateTime WithdrawableAt { get; set; }
    }
}
