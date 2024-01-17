using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApp55
{
    public class TelegramPost
    {
        private string Token { get; set; }
        private string ChannelName { get; set; }
        private string PostText { get; set; }

        public TelegramPost(string token)
        {
            this.Token = token;
        }

        public async Task PostHandle()
        {
            var botClient = new TelegramBotClient(this.Token);

            using CancellationTokenSource cts = new();

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            botClient.StartReceiving(
                updateHandler:  HandleUpdateAsync,
                pollingErrorHandler:  HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            cts.Cancel();
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message || message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;

            if (messageText.StartsWith("/createpost"))
            {
                ChannelName = null; 
                PostText = null;   

                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"Enter channel name: (After entering, send '#')",
                    cancellationToken: cancellationToken);
            }
            else if (ChannelName == null)
            {
                ChannelName = messageText.Trim('#');

                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"Enter post text:",
                    cancellationToken: cancellationToken);
            }
            else if (PostText == null)
            {
                PostText = messageText;

                
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"Channel: {ChannelName}\nPost: {PostText}\n\nYou can now perform actions like 'Postni Joylash', 'Postni Edit Qilish', etc.",
                    cancellationToken: cancellationToken);
            }
        }

        private async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            await Task.CompletedTask;
        }
    }
}
