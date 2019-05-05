using System;
using BitSkinsApi;

namespace BitSkinsBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Bot.Initilize.InitilizeAccount();

            var a = BitSkinsApi.Balance.CurrentBalance.GetAccountBalance();

            Console.WriteLine(a.AvailableBalance);

            Console.WriteLine("Hello World!");
        }
    }
}
