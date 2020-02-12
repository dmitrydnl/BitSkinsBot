using System.IO;
using Newtonsoft.Json;

namespace BitSkinsBot
{
    internal class AccountData
    {
        public string ApiKey { get; set; }
        public string SecretCode { get; set; }

        private static AccountData instance;

        internal static AccountData GetInstance()
        {
            if (instance == null)
            {
                string jsonText = File.ReadAllText("account_data.json");
                instance = JsonConvert.DeserializeObject<AccountData>(jsonText);
            }

            return instance;
        }
    }
}
