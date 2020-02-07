using BukBot.Enums;
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
        [Command("")]
        [Summary("Komenda służąca do czyszczenia czatu z wiadomości z ostatnich x minut/godzn/dni/tygodni")]
        [Remarks("$Clean {Week/Day/Hour/Minute} {ilość}")]
        public async Task DefaultCleanAsync(params string[] commandArgs)
        {
            if (commandArgs?.Length != 2)
            {
                await ReplyAsync("Dzban, Clean {Week/Day/Hour/Minute} {ilość}");
                return;
            }

            if (!(Enum.TryParse<TimeStampEnum>(commandArgs[0], out var timeStamp) && Enum.IsDefined(typeof(TimeStampEnum), timeStamp)))
            {
                await ReplyAsync("Dzban, podałeś zły enum, prawidłowe to: Week, Day, Hour, Minute");
                return;
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
        [Summary("Komenda służąca do czyszczenia czatu, na ktorym została wywołana ze wszystkich wiadomości bota")]
        [Remarks("$Clean Bot")]
        public async Task CleanBotMessages()
        {
            var botUserId = Context.Client.CurrentUser.Id;
            await ReplyAndDeleteAsync("Usuwam...", timeout: TimeSpan.FromSeconds(3));
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync((
                await Context.Channel.GetMessagesAsync(limit: 10000).FlattenAsync()).Where(m => m.Author.Id <= botUserId));
        }

        private double GetMinutesFromEnum(TimeStampEnum timeStamp, int amount) =>
            timeStamp switch
            {
                TimeStampEnum.Week => TimeSpan.FromDays(7 * amount).TotalMinutes,
                TimeStampEnum.Day => TimeSpan.FromDays(amount).TotalMinutes,
                TimeStampEnum.Hour => TimeSpan.FromHours(amount).TotalMinutes,
                TimeStampEnum.Minute => TimeSpan.FromMinutes(amount).TotalMinutes,
                _ => throw new ArgumentException(message: "zły enum"),
            };
    }
}
