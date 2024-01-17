using System;
using Telegram.Bot;


namespace ConsoleApp55;

class Program
{
    static async Task Main(string[] args)
    {

        const string token = "6963507776:AAGytlWgOC5Y4WYBRp7utDo1StTAlwm8vvc";

        TelegramPost telegramPost = new TelegramPost(token);
        await telegramPost.PostHandle();
      

    }
}