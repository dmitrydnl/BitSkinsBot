using Telegram.Bot;
using Newtonsoft.Json;

namespace TelegramBot
{
    public static class Bot
    {
        private static ITelegramBotClient botClient;

        public static void InitilizeBot()
        {
            string token = ReadToken();
            botClient = new TelegramBotClient(token);
        }

        private static string ReadToken()
        {
            string jsonText = System.IO.File.ReadAllText("bot_data.json");
            dynamic botData = JsonConvert.DeserializeObject<dynamic>(jsonText);
            string token = botData.Token;
            return token;
        }
    }
}
