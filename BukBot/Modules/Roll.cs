using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BukBot.Modules
{
    public class Roll : ModuleBase<SocketCommandContext>
    {
        [Command("Roll")]
        public async Task RollPatryk(params string[] rollArgs)
        {
            if (rollArgs.Length != 2)
            {
                await ReplyAsync("Dzban, @BukBot Roll {osoba} {kanał do}");
                return;
            }
            var userFromMessage = rollArgs[0];
            var desitinationChannelFromMessage = rollArgs[1];
            var user = Context.Guild.Users.Where(u => u.Username == userFromMessage).FirstOrDefault();
            var originChannel = user.VoiceChannel;
            var destinationChannel = Context.Guild.VoiceChannels.Where(c => c.Name == desitinationChannelFromMessage).FirstOrDefault();
            if (originChannel.Name == destinationChannel.Name)
            {
                await ReplyAsync("Dzban, nie możesz podać takiej samej nazwy kanału");
                return;
            }
            await user.ModifyAsync(x => x.Channel = destinationChannel);
            await user.ModifyAsync(x => x.Channel = originChannel);
            await user.ModifyAsync(x => x.Channel = destinationChannel);
            await user.ModifyAsync(x => x.Channel = originChannel);
            await user.ModifyAsync(x => x.Channel = destinationChannel);
            await user.ModifyAsync(x => x.Channel = originChannel);
            var users = Context.Guild.Users.Select(u => u.Username);
            await ReplyAsync($"Bokiem jest {user}");
        }
    }
}
