using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Newtonsoft.Json;

namespace TelegramBot
{
    public static class Bot
    {
        private static ITelegramBotClient botClient;
        private static List<Telegram.Bot.Types.Chat> chatsForNotification = new List<Telegram.Bot.Types.Chat>();

        public static void Initilize()
        {
            string token = ReadToken();
            botClient = new TelegramBotClient(token);

            Telegram.Bot.Types.User me = botClient.GetMeAsync().Result;
            Console.WriteLine($"Bot is ready. Bot ID: {me.Id}");

            botClient.OnMessage += OnMessage;
            botClient.StartReceiving();
        }

        private static string ReadToken()
        {
            string jsonText = System.IO.File.ReadAllText("bot_data.json");
            dynamic botData = JsonConvert.DeserializeObject<dynamic>(jsonText);
            string token = botData.Token;
            return token;
        }

        private static string ReadPassword()
        {
            string jsonText = System.IO.File.ReadAllText("bot_data.json");
            dynamic botData = JsonConvert.DeserializeObject<dynamic>(jsonText);
            string password = botData.Password;
            return password;
        }

        public static async void SendNotification(string text)
        {
            foreach (Telegram.Bot.Types.Chat chat in chatsForNotification)
            {
                await botClient.SendTextMessageAsync(
                  chatId: chat,
                  text: text
                );
            }
        }

        private static async void OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                string password = ReadPassword();

                if (string.Equals(e.Message.Text, password))
                {
                    chatsForNotification.Add(e.Message.Chat);

                    Console.WriteLine($"Added new user in notification. User chat ID: {e.Message.Chat.Id}.");
                    await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: "Success"
                    );
                }
            }
        }
    }
}
