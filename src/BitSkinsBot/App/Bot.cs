using System.Threading;
using System.Collections.Generic;
using BitSkinsBot.FastMarketAnalize;
using BitSkinsBot.EventsLog;

namespace BitSkinsBot
{
    internal class Bot
    {
        private IRelistForSale relistForSale;
        private IPurchase purchase;
        private ICheckSoldItems checkSoldItems;

        private List<SortFilter> searchFilters;
        private List<MarketItem> boughtItems;
        private List<MarketItem> onSaleItems;
        private List<MarketItem> soldItems;

        internal Bot()
        {
            AccountData accountData = AccountData.GetInstance();
            BitSkinsApi.Account.AccountData.SetupAccount(accountData.ApiKey, accountData.SecretCode);
            InitializeModules();
            InitializeData();
        }

        private void InitializeModules()
        {
            relistForSale = new RelistForSale();
            purchase = new Purchase();
            checkSoldItems = new CheckSoldItems();
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
                    IProfitableItems profitableItems = new ProfitableItems(filter);
                    List<MarketItem> profitableMarketItems = profitableItems.SearchProfitableItems();
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

                        balance -= PurchaseItem(marketItem);
                    }
                }

                RelistItemsForSale();
                CheckSoldItems();

                ConsoleLog.WriteInfo("Sleep 1 hour");
                Thread.Sleep(60 * 60 * 1000);
            }
        }

        private double PurchaseItem(MarketItem marketItem)
        {
            double price = 0;
            List<MarketItem> items = purchase.PurchaseItems(new List<MarketItem> { marketItem });
            foreach (MarketItem item in items)
            {
                boughtItems.Add(item);
                price += item.BuyPrice;
            }

            return price;
        }

        private void RelistItemsForSale()
        {
            List<MarketItem> relistedItems = relistForSale.RelistItemsForSale(boughtItems);
            foreach (MarketItem item in relistedItems)
            {
                boughtItems.Remove(item);
                onSaleItems.Add(item);
            }
        }

        private void CheckSoldItems()
        {
            List<MarketItem> recentlySoldItems = checkSoldItems.GetSoldItems(onSaleItems);
            foreach (MarketItem marketItem in recentlySoldItems)
            {
                onSaleItems.Remove(marketItem);
                soldItems.Add(marketItem);
            }
        }
    }
}
