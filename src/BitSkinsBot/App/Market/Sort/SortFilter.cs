using System.Collections.Generic;
using Newtonsoft.Json;
using BitSkinsApi.Market;

namespace BitSkinsBot.Market.Sort
{
    internal class SortFilter
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
        internal int? MinItemsOnSale { get; set; }
        internal int? MaxItemsOnSale { get; set; }

        private static List<SortFilter> filters;

        internal static List<SortFilter> GetFilters()
        {
            if (filters == null || filters.Count == 0)
            {
                string jsonText = System.IO.File.ReadAllText("filters.json");
                dynamic newFilters = JsonConvert.DeserializeObject<dynamic>(jsonText).Filters;
                foreach (dynamic filter in newFilters)
                {
                    SortFilter searchFilter = new SortFilter
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
                        MaxAveragePriceInLastWeekPercentFromLowestPrice = filter.MaxAveragePriceInLastWeekPercentFromLowestPrice,
                        MinItemsOnSale = filter.MinItemsOnSale,
                        MaxItemsOnSale = filter.MaxItemsOnSale
                    };

                    if (filters == null)
                    {
                        filters = new List<SortFilter>();
                    }

                    filters.Add(searchFilter);
                }
            }

            return filters;
        }
    }
}
