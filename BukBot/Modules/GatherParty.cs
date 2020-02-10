using BukBot.Enums;
using BukBot.Helpers;
using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace BukBot.Modules
{
    [Group("GatherParty")]
    public class GatherParty : ModuleBase<SocketCommandContext>
    {
        [Command("Liga")]
        [Summary("Komenda slużąca do zebrania drużyny na niebezpieczną przygodę na Rifta (wszyscy z rangą  **Liga**)")]
        [Remarks("$GatherParty Liga")]
        public async Task GatherLeagueOfLegendsParty()
        {
            var usersWithLolRole = Context.Guild.Roles.FirstOrDefault(r => r.Name == RoleFactory.GetRoleName(RoleTypeEnum.Liga)).Members
                .Where(u => u.Status != UserStatus.Offline && u.Status != UserStatus.Invisible)
                .ToList();
            if(!usersWithLolRole.Any())
            {
                await ReplyAsync($"Nikt nie ma rangi **{RoleFactory.GetRoleName(RoleTypeEnum.Liga)}**");
                return;
            }
            usersWithLolRole.ForEach(u => u.SendMessageAsync($"**{Context.User.Username}** zbiera drużyne do Ligi legend, przybądź!"));
            await ReplyAsync($"Wezwano drużynę: **{string.Join(" ", usersWithLolRole.Select(u => u.Username))}**");
        }
    }
}
