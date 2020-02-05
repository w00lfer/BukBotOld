using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BukBot.Modules
{
    [Group("Clean")]
    public class Clean : InteractiveBase<SocketCommandContext>
    {
        [Command]
        public async Task DefaultCleanAsync(params string[] commandArgs)
        {
            if (commandArgs?.Length != 2)
            {
                await ReplyAsync("Dzban, Clean {Week/Day/Hour/Minute} {ilość}");
                return;
            }

            if (!Enum.TryParse<TimeStampEnum>(commandArgs[0], out var timeStamp))
            {
                await ReplyAsync("Dzban, podałeś zły enum, prawidłowe to: Week, Day, Hour, Minute");
            }

            if (!int.TryParse(commandArgs[1], out int amount) && amount < 1)
            {
                await ReplyAsync("Dzban, musisz podać liczbę naturalną większą lub równą 1");
                return;
            }
            var timestampp = (await Context.Channel.GetMessagesAsync().FlattenAsync()).FirstOrDefault();
            await ReplyAndDeleteAsync("Usuwam...", timeout: TimeSpan.FromSeconds(3));

            var messages = (await Context.Channel.GetMessagesAsync(limit: 10000)
                .FlattenAsync())
                .Where(m => m.CreatedAt >= new DateTimeOffset(DateTime.Now).AddMinutes(-GetMinutesFromEnum(timeStamp, amount)));
            if(!messages.Skip(1).Any())
            {
                await ReplyAsync("Dzban, nie było żadnych wiadomości w tym przezdiale");
                return;
            }

            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
        }

        [Command("Bot")]
        public async Task CleanBotMessages()
        {
            var botUserId = Context.Client.CurrentUser.Id;
            await ReplyAndDeleteAsync("Usuwam...", timeout: TimeSpan.FromSeconds(3));
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync((
                await Context.Channel.GetMessagesAsync(limit: 10000).FlattenAsync()).Where(m => m.Author.Id <= botUserId));
        }

        private int GetMinutesFromEnum(TimeStampEnum timeStamp, int amount) =>
            timeStamp switch
            {
                TimeStampEnum.Week => TimeSpan.FromDays(7 * amount).Minutes,
                TimeStampEnum.Day => TimeSpan.FromDays(amount).Minutes,
                TimeStampEnum.Hour => TimeSpan.FromHours(amount).Minutes,
                TimeStampEnum.Minute => TimeSpan.FromMinutes(amount).Minutes,
                _ => throw new ArgumentException(message: "zły enum"),
            };

        internal enum TimeStampEnum
        {
            Week,
            Day,
            Hour,
            Minute
        }
    }
}
