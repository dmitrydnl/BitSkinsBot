using Newtonsoft.Json;

namespace BitSkinsBot.Bot
{
    internal static class Initilize
    {
        internal static void InitilizeAccount()
        {
            string jsonText = System.IO.File.ReadAllText("account_data.json");
            dynamic accountData = JsonConvert.DeserializeObject<dynamic>(jsonText);
            string apiKey = accountData.ApiKey;
            string secretCode = accountData.SecretCode;

            BitSkinsApi.Account.AccountData.SetupAccount(apiKey, secretCode);
        }
    }
}
