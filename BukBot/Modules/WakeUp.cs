using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace BukBot.Modules
{
    public class WakeUp : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Moves user around channels to wake him up from being AFK,
        /// Example: 
        /// </summary>
        /// <param name="commandArgs">
        /// rollArgs[0] is username of user to be rolled
        /// rollArgs[1] is name of channel to move user to it
        /// </param>
        /// <returns></returns>
        [Command("WakeUp")]
        public async Task WakeUpUser(params string[] commandArgs)
        {
            if (commandArgs.Length != 2)
            {
                await ReplyAsync("Dzban, $WakeUp {osoba} {kanał do}");
                return;
            }

            var userFromMessage = commandArgs[0];
            var destinationChannelFromMessage = commandArgs[1];

            var user = Context.Guild.Users.FirstOrDefault(u => u.Username == userFromMessage);
            if (!await ValidateObjectIfItsNotNull(user, "Dzban, taki user nie istnieje na serwerze")) return;

            var originChannel = user.VoiceChannel;
            if (!await ValidateObjectIfItsNotNull(originChannel, "Dzban, usera nie jest na żadnym z kanałów głosowyc")) return;

            var destinationChannel = Context.Guild.VoiceChannels.FirstOrDefault(c => c.Name == destinationChannelFromMessage);
            if (!await ValidateObjectIfItsNotNull(destinationChannel, "Dzban, taki kanał na serwerze nie istnieje")) return;
            
            if (originChannel.Name == destinationChannel.Name)
            {
                await ReplyAsync("Dzban, nie możesz podać takiej samej nazwy kanału");
                return;
            }

            await MoveUserAroundChannels(user, originChannel, destinationChannel);
        }

        private async Task MoveUserAroundChannels(SocketGuildUser user, SocketVoiceChannel originChannel, SocketVoiceChannel destinationChannel, int multiplier = 5)
        {
            for (int i = 0; i < multiplier; i++)
            {
                await user.ModifyAsync(u => u.Channel = destinationChannel);
                await user.ModifyAsync(u => u.Channel = originChannel);
            }
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
    }
}
