using System.Collections.Generic;
using Newtonsoft.Json;
using BitSkinsApi.Market;

namespace BitSkinsBot.FastMarketAnalize
{
    internal class Filters
    {
        internal readonly List<Filter> filters = new List<Filter>();

        internal Filters()
        {
            InitilizeFilters();
        }

        private void InitilizeFilters()
        {
            string jsonText = System.IO.File.ReadAllText("filters.json");
            dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonText);

            foreach (dynamic jsonFilter in jsonObject.filters)
            {
                Filter filter = new Filter
                {
                    App = (AppId.AppName)jsonFilter.App,
                    MinTotalItems = jsonFilter.MinTotalItems,
                    MaxTotalItems = jsonFilter.MaxTotalItems,
                    MinLowestPricePercentFromBalance = jsonFilter.MinLowestPricePercentFromBalance,
                    MaxLowestPricePercentFromBalance = jsonFilter.MaxLowestPricePercentFromBalance,
                    MinHighestPricePercentFromLowestPrice = jsonFilter.MinHighestPricePercentFromLowestPrice,
                    MaxHighestPricePercentFromLowestPrice = jsonFilter.MaxHighestPricePercentFromLowestPrice,
                    MinCumulativePricePercentFromLowestCumulativePrice = jsonFilter.MinCumulativePricePercentFromLowestCumulativePrice,
                    MaxCumulativePricePercentFromLowestCumulativePrice = jsonFilter.MaxCumulativePricePercentFromLowestCumulativePrice,
                    MinRecentAveragePricePercentFromLowestPrice = jsonFilter.MinRecentAveragePricePercentFromLowestPrice,
                    MaxRecentAveragePricePercentFromLowestPrice = jsonFilter.MaxRecentAveragePricePercentFromLowestPrice,
                    MinCountOfSalesInLastWeek = jsonFilter.MinCountOfSalesInLastWeek,
                    MaxCountOfSalesInLastWeek = jsonFilter.MaxCountOfSalesInLastWeek,
                    MinAveragePriceInLastWeekPercentFromLowestPrice = jsonFilter.MinAveragePriceInLastWeekPercentFromLowestPrice,
                    MaxAveragePriceInLastWeekPercentFromLowestPrice = jsonFilter.MaxAveragePriceInLastWeekPercentFromLowestPrice
                };

                filters.Add(filter);
            }
        }
    }

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
        internal int? MinCountOfSalesInLastWeek { get; set; }
        internal int? MaxCountOfSalesInLastWeek { get; set; }
        internal int? MinAveragePriceInLastWeekPercentFromLowestPrice { get; set; }
        internal int? MaxAveragePriceInLastWeekPercentFromLowestPrice { get; set; }
    }
}
