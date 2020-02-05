using Discord.Commands;
using System.Threading.Tasks;

namespace BukBot.Modules
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command("Ping")]
        public async Task Pong()
        {
            await ReplyAsync("Pong");
        }
    }

}
