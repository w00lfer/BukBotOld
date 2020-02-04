using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BukBot.Modules
{
    public class DeathRollForRole : ModuleBase<SocketCommandContext>
    {
        [Command("DeathRollForRole")]
        public async Task DeathRollForRoleBetweenUsers(params string[] commandArgs)
        {
            if (commandArgs.Length != 4)
            {
                await ReplyAsync("Dzban, $DeathRollForRole {osoba1} {osoba2} {maxRoll} {rola}");
                return;
            }

            var firstUserFromMessage = commandArgs[0];
            var secondUserFromMessage = commandArgs[1];
            var maxRollFromMessage = commandArgs[2];
            var roleFromMessage = commandArgs[3];

            var firstUser = Context.Guild.Users.FirstOrDefault(u => u.Username == firstUserFromMessage);
            if (!await ValidateObjectIfItsNotNull(firstUserFromMessage, "Dzban, taki user nie istnieje na serwerze")) return;

            var secondUser = Context.Guild.Users.FirstOrDefault(u => u.Username == secondUserFromMessage);
            if (!await ValidateObjectIfItsNotNull(secondUserFromMessage, "Dzban, taki user nie istnieje na serwerze")) return;

            if(!int.TryParse(maxRollFromMessage, out int maxRoll))
            {
                await ReplyAsync("Dzban, musisz podać liczbę naturalną");
                return;
            }
            await PerformDeathRoll(firstUser, secondUser, maxRoll);
        }
        private async Task<bool> ValidateObjectIfItsNotNull(object objectToCheck, string errorMessage)
        {
            if (objectToCheck == null)
            {
                await ReplyAsync(errorMessage);
                return false;
            }
            return true;
        }
        private async Task PerformDeathRoll(SocketGuildUser firstUser, SocketGuildUser seconduser, int maxRoll)
        {
            Random rnd = new Random();     
            while(maxRoll != 1)
            {
                maxRoll = rnd.Next(maxRoll) + 1;
                await ReplyAsync($"{firstUser.Username} roll za: {maxRoll}");
                
            }
        }
    }
}
