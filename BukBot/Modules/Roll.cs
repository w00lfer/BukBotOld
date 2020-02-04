using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace BukBot.Modules
{
    public class Roll : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Rolls user from one channel to second channel,
        /// </summary>
        /// <param name="rollArgs">
        /// rollArgs[0] is username of user to be rolled
        /// rollArgs[1] is name of channel to move user to it
        /// </param>
        /// <returns></returns>
        [Command("Roll")]
        public async Task RollUser(params string[] rollArgs)
        {
            if (rollArgs.Length != 2)
            {
                await ReplyAsync("Dzban, @BukBot Roll {osoba} {kanał do}");
                return;
            }

            var userFromMessage = rollArgs[0];
            var destinationChannelFromMessage = rollArgs[1];

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
