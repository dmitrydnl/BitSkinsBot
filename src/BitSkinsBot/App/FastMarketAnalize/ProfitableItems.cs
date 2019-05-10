using System;
using System.Linq;
using System.Collections.Generic;
using BitSkinsApi.Market;
using BitSkinsApi.Balance;

namespace BitSkinsBot.FastMarketAnalize
{
    internal class ProfitableItems
    {
        private readonly Filter currentFilter;

        internal ProfitableItems(Filter filter)
        {
            currentFilter = filter;
        }

        internal List<ProfitableMarketItem> GetProfitableItems()
        {
            Console.WriteLine("Start find profitable items");

            List<MarketItem> marketItems = GetMarketItems();
            Console.WriteLine("All items: " + marketItems.Count);

            marketItems = SortByTotalItems(marketItems);
            Console.WriteLine("1 sort count: " + marketItems.Count);

            marketItems = SortByLowestPrice(marketItems);
            Console.WriteLine("2 sort count: " + marketItems.Count);

            marketItems = SortByHighestPrice(marketItems);
            Console.WriteLine("3 sort count: " + marketItems.Count);

            marketItems = SortByCumulativePrice(marketItems);
            Console.WriteLine("4 sort count: " + marketItems.Count);

            marketItems = SortByRecentAveragePrice(marketItems);
            Console.WriteLine("5 sort count: " + marketItems.Count);

            marketItems = SortByRecentSales(marketItems);
            Console.WriteLine("6 sort count: " + marketItems.Count);

            List<ProfitableMarketItem> profitableMarketItems = GetProfitableMarketItems(marketItems);
            Console.WriteLine("Final sort count: " + profitableMarketItems.Count);

            return profitableMarketItems;
        }

        private double GetAvailableBalance()
        {
            double availableBalance = CurrentBalance.GetAccountBalance().AvailableBalance;
            return availableBalance;
        }

        private List<MarketItem> GetMarketItems()
        {
            List<MarketItem> marketItems = MarketData.GetMarketData(currentFilter.App);
            return marketItems;
        }

        private List<MarketItem> SortByTotalItems(List<MarketItem> marketItems)
        {
            List<MarketItem> sortMarketItems = new List<MarketItem>();
            foreach (MarketItem marketItem in marketItems)
            {
                int totalItems = marketItem.TotalItems;
                int? minTotalItems = currentFilter.MinTotalItems;
                int? maxTotalItems = currentFilter.MaxTotalItems;

                if (minTotalItems == null || totalItems >= minTotalItems)
                {
                    if (maxTotalItems == null || totalItems <= maxTotalItems)
                    {
                        sortMarketItems.Add(marketItem);
                    }
                }
            }

            return sortMarketItems;
        }

        private List<MarketItem> SortByLowestPrice(List<MarketItem> marketItems)
        {
            double availableBalance = GetAvailableBalance();

            List<MarketItem> sortMarketItems = new List<MarketItem>();
            foreach (MarketItem marketItem in marketItems)
            {
                double lowestPrice = marketItem.LowestPrice;
                double? minLowestPrice = currentFilter.MinLowestPricePercentFromBalance == null ? 
                    null : availableBalance / 100 * currentFilter.MinLowestPricePercentFromBalance;
                double? maxLowestPrice = currentFilter.MaxLowestPricePercentFromBalance == null ? 
                    null : availableBalance / 100 * currentFilter.MaxLowestPricePercentFromBalance;

                if (minLowestPrice == null || lowestPrice >= minLowestPrice)
                {
                    if (maxLowestPrice == null || lowestPrice <= maxLowestPrice)
                    {
                        sortMarketItems.Add(marketItem);
                    }
                }
            }

            return sortMarketItems;
        }

        private List<MarketItem> SortByHighestPrice(List<MarketItem> marketItems)
        {
            List<MarketItem> sortMarketItems = new List<MarketItem>();
            foreach (MarketItem marketItem in marketItems)
            {
                double lowestPrice = marketItem.LowestPrice;
                double highestPrice = marketItem.HighestPrice;
                double? minHighestPrice = currentFilter.MinHighestPricePercentFromLowestPrice == null ?
                    null : lowestPrice / 100 * currentFilter.MinHighestPricePercentFromLowestPrice;
                double? maxHighestPrice = currentFilter.MaxHighestPricePercentFromLowestPrice == null ?
                    null : lowestPrice / 100 * currentFilter.MaxHighestPricePercentFromLowestPrice;

                if (minHighestPrice == null || highestPrice >= minHighestPrice)
                {
                    if (maxHighestPrice == null || highestPrice <= maxHighestPrice)
                    {
                        sortMarketItems.Add(marketItem);
                    }
                }
            }

            return sortMarketItems;
        }

        private List<MarketItem> SortByCumulativePrice(List<MarketItem> marketItems)
        {
            List<MarketItem> sortMarketItems = new List<MarketItem>();
            foreach (MarketItem marketItem in marketItems)
            {
                int totalItems = marketItem.TotalItems;
                double lowestPrice = marketItem.LowestPrice;
                double cumulativePrice = marketItem.CumulativePrice;
                double? minCumulativePrice = currentFilter.MinCumulativePricePercentFromLowestCumulativePrice == null ?
                    null : totalItems * lowestPrice / 100 * currentFilter.MinCumulativePricePercentFromLowestCumulativePrice;
                double? maxCumulativePrice = currentFilter.MaxCumulativePricePercentFromLowestCumulativePrice == null ?
                    null : totalItems * lowestPrice / 100 * currentFilter.MaxCumulativePricePercentFromLowestCumulativePrice;

                if (minCumulativePrice == null || cumulativePrice >= minCumulativePrice)
                {
                    if (maxCumulativePrice == null || cumulativePrice <= maxCumulativePrice)
                    {
                        sortMarketItems.Add(marketItem);
                    }
                }
            }

            return sortMarketItems;
        }

        private List<MarketItem> SortByRecentAveragePrice(List<MarketItem> marketItems)
        {
            List<MarketItem> sortMarketItems = new List<MarketItem>();
            foreach (MarketItem marketItem in marketItems)
            {
                double lowestPrice = marketItem.LowestPrice;
                double recentAveragePrice = marketItem.RecentAveragePrice;
                double? minRecentAveragePrice = currentFilter.MinRecentAveragePricePercentFromLowestPrice == null ?
                    null : lowestPrice / 100 * currentFilter.MinRecentAveragePricePercentFromLowestPrice;
                double? maxRecentAveragePrice = currentFilter.MaxRecentAveragePricePercentFromLowestPrice == null ?
                    null : lowestPrice / 100 * currentFilter.MaxRecentAveragePricePercentFromLowestPrice;

                if (minRecentAveragePrice == null || recentAveragePrice >= minRecentAveragePrice)
                {
                    if (maxRecentAveragePrice == null || recentAveragePrice <= maxRecentAveragePrice)
                    {
                        sortMarketItems.Add(marketItem);
                    }
                }
            }

            return sortMarketItems;
        }

        private List<MarketItem> SortByRecentSales(List<MarketItem> marketItems)
        {
            int count = 0;
            List<MarketItem> sortMarketItems = new List<MarketItem>();
            foreach (MarketItem marketItem in marketItems)
            {
                string itemName = marketItem.MarketHashName;
                List<ItemRecentSale> itemRecentSales = new List<ItemRecentSale>();
                for (int i = 1; i <= 5; i++)
                {
                    List<ItemRecentSale> items = RecentSaleInfo.GetRecentSaleInfo(currentFilter.App, itemName, i);
                    foreach (ItemRecentSale item in items)
                    {
                        itemRecentSales.Add(item);
                    }
                }

                int countOfSalesInLastWeek = 0;
                List<double> priceSales = new List<double>();
                foreach (ItemRecentSale item in itemRecentSales)
                {
                    if (item.SoldAt < DateTime.Now.AddDays(-7))
                    {
                        break;
                    }

                    countOfSalesInLastWeek++;
                    priceSales.Add(item.Price);
                }

                double averagePriceInLastWeek = 0;
                if (priceSales.Count > 0)
                {
                    averagePriceInLastWeek = priceSales.Average();
                }

                double lowestPrice = marketItem.LowestPrice;
                int? minCountOfSalesInLastWeek = currentFilter.MinCountOfSalesInLastWeek;
                int? maxCountOfSalesInLastWeek = currentFilter.MaxCountOfSalesInLastWeek;
                double? minAveragePriceInLastWeek = currentFilter.MinAveragePriceInLastWeekPercentFromLowestPrice == null ?
                    null : lowestPrice / 100 * currentFilter.MinAveragePriceInLastWeekPercentFromLowestPrice;
                double? maxAveragePriceInLastWeek = currentFilter.MaxAveragePriceInLastWeekPercentFromLowestPrice == null ?
                    null : lowestPrice / 100 * currentFilter.MaxAveragePriceInLastWeekPercentFromLowestPrice;

                if (minCountOfSalesInLastWeek == null || countOfSalesInLastWeek >= minCountOfSalesInLastWeek)
                {
                    if (maxCountOfSalesInLastWeek == null || countOfSalesInLastWeek <= maxCountOfSalesInLastWeek)
                    {
                        if (minAveragePriceInLastWeek == null || averagePriceInLastWeek >= minAveragePriceInLastWeek)
                        {
                            if (maxAveragePriceInLastWeek == null || averagePriceInLastWeek <= maxAveragePriceInLastWeek)
                            {
                                sortMarketItems.Add(marketItem);
                            }
                        }
                    }
                }

                count++;
                Console.WriteLine(count + " from " + marketItems.Count);
            }

            return sortMarketItems;
        }
    
        private List<ProfitableMarketItem> GetProfitableMarketItems(List<MarketItem> marketItems)
        {
            int count = 0;
            List<ProfitableMarketItem> profitableMarketItems = new List<ProfitableMarketItem>();
            foreach (MarketItem marketItem in marketItems)
            {
                string name = marketItem.MarketHashName;
                List<ItemOnSale> ItemsOnSale = InventoryOnSale.GetInventoryOnSale(currentFilter.App, 1, name, 0, 0, InventoryOnSale.SortBy.Price,
                    InventoryOnSale.SortOrder.Asc, InventoryOnSale.ThreeChoices.NotImportant, InventoryOnSale.ThreeChoices.NotImportant,
                    InventoryOnSale.ThreeChoices.NotImportant, InventoryOnSale.ResultsPerPage.R30, InventoryOnSale.ThreeChoices.NotImportant);

                ItemOnSale itemOnSale = ItemsOnSale[0];
                ItemOnSale itemOnSale3 = null;
                if (ItemsOnSale.Count >= 3)
                {
                    itemOnSale3 = ItemsOnSale[2];
                }

                if (itemOnSale3 == null || itemOnSale3.Price >= itemOnSale.Price * 1.05)
                {
                    if (!itemOnSale.IsMine)
                    {
                        if (itemOnSale.Price <= marketItem.LowestPrice)
                        {
                            ProfitableMarketItem profitableMarketItem = new ProfitableMarketItem
                            {
                                Name = marketItem.MarketHashName,
                                Id = itemOnSale.ItemId,
                                BuyPrice = itemOnSale.Price,
                                SellPrice = Math.Round(itemOnSale.Price / 100 * (currentFilter.MinAveragePriceInLastWeekPercentFromLowestPrice == null ?
                                110 : (double)currentFilter.MinAveragePriceInLastWeekPercentFromLowestPrice), 2)
                            };
                            profitableMarketItems.Add(profitableMarketItem);
                        }
                    }
                }

                count++;
                Console.WriteLine(count + " from " + marketItems.Count);
            }

            return profitableMarketItems;
        }
    }
}
