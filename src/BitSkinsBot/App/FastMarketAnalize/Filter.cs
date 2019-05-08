using BitSkinsApi.Market;

namespace BitSkinsBot.FastMarketAnalize
{
    internal class Filter
    {
        internal AppId.AppName App { get; set; }
        internal int? MinTotalItems { get; set; }
        internal int? MaxTotalItems { get; set; }
        internal int? MinLowestPricePercentFromBalance { get; set; }
        internal int? MaxLowestPricePercentFromBalance { get; set; }
        internal int? MinHighestPricePercentFromLowestPrice { get; set; }
        internal int? MaxHighestPricePercentFromLowestPrice { get; set; }
        internal int? MinCumulativePricePercentFromLowestCumulativePrice { get; set; }
        internal int? MaxCumulativePricePercentFromLowestCumulativePrice { get; set; }
        internal int? MinRecentAveragePricePercentFromLowestPrice { get; set; }
        internal int? MaxRecentAveragePricePercentFromLowestPrice { get; set; }
    }
}
