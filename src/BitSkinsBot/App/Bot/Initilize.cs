﻿using Newtonsoft.Json;

namespace BitSkinsBot.Bot
{
    public static class Initilize
    {
        public static void InitilizeAccount()
        {
            string jsonText = System.IO.File.ReadAllText("account_data.json");
            AccountData accountData = JsonConvert.DeserializeObject<AccountData>(jsonText);
            string apiKey = accountData.ApiKey;
            string secretCode = accountData.SecretCode;

            BitSkinsApi.Account.AccountData.SetupAccount(apiKey, secretCode);
        }
    }

    class AccountData
    {
        public string ApiKey { get; set; }
        public string SecretCode { get; set; }
    }
}
