using System.Threading;
using System.Collections.Generic;
using BitSkinsBot.FastMarketAnalize;
using BitSkinsBot.EventsLog;

namespace BitSkinsBot
{
    internal class Bot
    {
        private List<SortFilter> searchFilters;
        private List<MarketItem> boughtItems;
        private List<MarketItem> onSaleItems;
        private List<MarketItem> soldItems;

        internal Bot()
        {
            AccountData accountData = AccountData.GetInstance();
            BitSkinsApi.Account.AccountData.SetupAccount(accountData.ApiKey, accountData.SecretCode);
            InitializeData();
        }

        private void InitializeData()
        {
            searchFilters = SortFilter.GetFilters();
            boughtItems = new List<MarketItem>();
            soldItems = new List<MarketItem>();
            onSaleItems = new List<MarketItem>();
        }

        internal void Start()
        {
            while (true)
            {
                foreach (SortFilter filter in searchFilters)
                {
                    List<MarketItem> profitableMarketItems = ProfitableItems.GetProfitableItems(filter);
                    if (profitableMarketItems == null || profitableMarketItems.Count == 0)
                    {
                        continue;
                    }

                    double availableBalance = BitSkinsApi.Balance.CurrentBalance.GetAccountBalance().AvailableBalance;
                    double balance = availableBalance / 5;
                    foreach (MarketItem marketItem in profitableMarketItems)
                    {
                        if (marketItem.BuyPrice > balance)
                        {
                            continue;
                        }

                        if (balance <= 0)
                        {
                            break;
                        }

                        List<MarketItem> items = Purchase.BuyItems(new List<MarketItem> { marketItem });
                        foreach (MarketItem item in items)
                        {
                            boughtItems.Add(item);
                            balance -= item.BuyPrice;
                        }
                    }
                }

                List<MarketItem> relistedItems = RelistForSale.RelistItems(boughtItems);
                foreach (MarketItem marketItem in relistedItems)
                {
                    boughtItems.Remove(marketItem);
                    onSaleItems.Add(marketItem);
                }

                List<MarketItem> recentlySoldItems = SoldItems.GetSoldItems(onSaleItems);
                foreach (MarketItem marketItem in recentlySoldItems)
                {
                    onSaleItems.Remove(marketItem);
                    soldItems.Add(marketItem);
                }

                ConsoleLog.WriteInfo("Sleep 1 hour");
                Thread.Sleep(60 * 60 * 1000);
            }
        }
    }
}
