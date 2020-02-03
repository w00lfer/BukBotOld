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
            if (rollArgs.Length != 3) await ReplyAsync("Dzban, @BukBot Roll {osoba} {kanał z} {kanał do}");
            var userFromMessage = rollArgs[0];
            var firstChannelFromMessage = rollArgs[1];
            var secondChannelFromMessage = rollArgs[2];
            var user = Context.Guild.Users.Where(u => u.Username == userFromMessage).FirstOrDefault();
            var firstChannel = Context.Guild.VoiceChannels.Where(c => c.Name == firstChannelFromMessage).FirstOrDefault();
            var secondChannel = Context.Guild.VoiceChannels.Where(c => c.Name == secondChannelFromMessage).FirstOrDefault();
            await user.ModifyAsync(x => x.Channel = secondChannel);
            await user.ModifyAsync(x => x.Channel = firstChannel);
            await user.ModifyAsync(x => x.Channel = secondChannel);
            await user.ModifyAsync(x => x.Channel = firstChannel);
            await user.ModifyAsync(x => x.Channel = secondChannel);
            await user.ModifyAsync(x => x.Channel = firstChannel);
            var users = Context.Guild.Users.Select(u => u.Username);
            await ReplyAsync($"Bokiem jest {user}");
        }
    }
}
