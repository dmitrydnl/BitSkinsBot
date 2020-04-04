using System.Threading;
using System.Collections.Generic;
using BitSkinsBot.Market;
using BitSkinsBot.Market.Buy;
using BitSkinsBot.Market.Sale;
using BitSkinsBot.Market.Search;
using BitSkinsBot.Market.Sort;
using BitSkinsBot.EventsLog;

namespace BitSkinsBot
{
    internal class Bot
    {
        private List<IProfitableItems> profitableItems;

        private IRelistForSale relistForSale;
        private IPurchase purchase;
        private ICheckSoldItems checkSoldItems;

        private List<MarketItem> boughtItems;
        private List<MarketItem> onSaleItems;
        private List<MarketItem> soldItems;

        internal Bot()
        {
            AccountData accountData = AccountData.GetInstance();
            BitSkinsApi.Account.AccountData.SetupAccount(accountData.ApiKey, accountData.SecretCode);
            InitializeFilters();
            InitializeModules();
            InitializeData();
        }

        private void InitializeFilters()
        {
            profitableItems = new List<IProfitableItems>();
            foreach (SortFilter sortFilter in SortFilter.GetFilters())
            {
                profitableItems.Add(new ProfitableItems(sortFilter));
            }
        }

        private void InitializeModules()
        {
            relistForSale = new RelistForSale();
            purchase = new Purchase();
            checkSoldItems = new CheckSoldItems();
        }

        private void InitializeData()
        {
            boughtItems = new List<MarketItem>();
            soldItems = new List<MarketItem>();
            onSaleItems = new List<MarketItem>();
        }

        internal void Start()
        {
            while (true)
            {
                foreach (IProfitableItems profitable in profitableItems)
                {
                    List<MarketItem> profitableMarketItems = profitable.SearchProfitableItems();
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
