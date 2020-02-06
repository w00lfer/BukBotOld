using BukBot.Enums;
using BukBot.PreconditionAttributes;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace BukBot.Modules
{
    public class WakeUp : ModuleBase<SocketCommandContext>
    {
        [RequireRole(RoleTypeEnum.Dupa)]
        [Command("WakeUp")]
        [Summary("Komenda do przerzucania osoby")]
        [Remarks("$WakeUp {osoba} {kanał do}")]
        public async Task WakeUpUserAsync(params string[] commandArgs)
        {
            if (commandArgs?.Length != 2)
            {
                await ReplyAsync("Dzban, $WakeUp {osoba} {kanał do}");
                return;
            }

            var user = Context.Guild.Users.FirstOrDefault(u => u.Username == commandArgs[0]);
            if (!await ValidateObjectIfItsNotNullAsync(user, "Dzban, taki user nie istnieje na serwerze")) return;

            var originChannel = user.VoiceChannel;
            if (!await ValidateObjectIfItsNotNullAsync(originChannel, "Dzban, usera nie jest na żadnym z kanałów głosowyc")) return;

            var destinationChannel = Context.Guild.VoiceChannels.FirstOrDefault(c => c.Name == commandArgs[1]);
            if (!await ValidateObjectIfItsNotNullAsync(destinationChannel, "Dzban, taki kanał na serwerze nie istnieje")) return;
            
            if (originChannel.Name == destinationChannel.Name)
            {
                await ReplyAsync("Dzban, nie możesz podać takiej samej nazwy kanału");
                return;
            }

            await MoveUserAroundChannelsAsync(user, originChannel, destinationChannel);
        }

        private async Task MoveUserAroundChannelsAsync(SocketGuildUser user, SocketVoiceChannel originChannel, SocketVoiceChannel destinationChannel, int multiplier = 5)
        {
            for (int i = 0; i < multiplier; i++)
            {
                await user.ModifyAsync(u => u.Channel = destinationChannel);
                await user.ModifyAsync(u => u.Channel = originChannel);
            }
        }

        private async Task<bool> ValidateObjectIfItsNotNullAsync(object objectToCheck, string errorMessage)
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
