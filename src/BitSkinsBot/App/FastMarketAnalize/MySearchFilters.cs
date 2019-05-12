using System.Collections.Generic;
using Newtonsoft.Json;
using BitSkinsApi.Market;

namespace BitSkinsBot.FastMarketAnalize
{
    internal class MySearchFilters
    {
        internal readonly List<SearchFilter> searchFilters = new List<SearchFilter>();

        internal MySearchFilters()
        {
            InitilizeFilters();
        }

        private void InitilizeFilters()
        {
            string jsonText = System.IO.File.ReadAllText("filters.json");
            dynamic filters = JsonConvert.DeserializeObject<dynamic>(jsonText).Filters;

            foreach (dynamic filter in filters)
            {
                SearchFilter searchFilter = new SearchFilter
                {
                    App = (AppId.AppName)filter.App,
                    MinTotalItems = filter.MinTotalItems,
                    MaxTotalItems = filter.MaxTotalItems,
                    MinLowestPrice = filter.MinLowestPrice,
                    MaxLowestPrice = filter.MaxLowestPrice,
                    MinHighestPricePercentFromLowestPrice = filter.MinHighestPricePercentFromLowestPrice,
                    MaxHighestPricePercentFromLowestPrice = filter.MaxHighestPricePercentFromLowestPrice,
                    MinCumulativePricePercentFromLowestCumulativePrice = filter.MinCumulativePricePercentFromLowestCumulativePrice,
                    MaxCumulativePricePercentFromLowestCumulativePrice = filter.MaxCumulativePricePercentFromLowestCumulativePrice,
                    MinRecentAveragePricePercentFromLowestPrice = filter.MinRecentAveragePricePercentFromLowestPrice,
                    MaxRecentAveragePricePercentFromLowestPrice = filter.MaxRecentAveragePricePercentFromLowestPrice,
                    MinCountOfSalesInLastWeek = filter.MinCountOfSalesInLastWeek,
                    MaxCountOfSalesInLastWeek = filter.MaxCountOfSalesInLastWeek,
                    MinAveragePriceInLastWeekPercentFromLowestPrice = filter.MinAveragePriceInLastWeekPercentFromLowestPrice,
                    MaxAveragePriceInLastWeekPercentFromLowestPrice = filter.MaxAveragePriceInLastWeekPercentFromLowestPrice
                };

                searchFilters.Add(searchFilter);
            }
        }
    }

    internal class SearchFilter
    {
        internal AppId.AppName App { get; set; }
        internal int? MinTotalItems { get; set; }
        internal int? MaxTotalItems { get; set; }
        internal double? MinLowestPrice { get; set; }
        internal double? MaxLowestPrice { get; set; }
        internal int? MinHighestPricePercentFromLowestPrice { get; set; }
        internal int? MaxHighestPricePercentFromLowestPrice { get; set; }
        internal int? MinCumulativePricePercentFromLowestCumulativePrice { get; set; }
        internal int? MaxCumulativePricePercentFromLowestCumulativePrice { get; set; }
        internal int? MinRecentAveragePricePercentFromLowestPrice { get; set; }
        internal int? MaxRecentAveragePricePercentFromLowestPrice { get; set; }
        internal int? MinCountOfSalesInLastWeek { get; set; }
        internal int? MaxCountOfSalesInLastWeek { get; set; }
        internal int? MinAveragePriceInLastWeekPercentFromLowestPrice { get; set; }
        internal int? MaxAveragePriceInLastWeekPercentFromLowestPrice { get; set; }
    }
}