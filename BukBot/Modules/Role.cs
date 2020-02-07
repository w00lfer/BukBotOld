using BukBot.Enums;
using BukBot.Helpers;
using BukBot.PreconditionAttributes;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace BukBot.Modules
{
    [RequireRole(RoleTypeEnum.Dupa)]
    [Group("Role")]
    public class Role : ModuleBase<SocketCommandContext>
    {
        [Command("Add")]
        [Summary("Komenda służąca do dodania roli na serwerze")]
        [Remarks("$Role Add {nazwa} {kolor w RGB}")]
        public async Task AddRole(params string[] commandArgs)
        {
            if (commandArgs?.Length != 2)
            {
                await ReplyAsync("Dzban, $Role Add {nazwa} {kolor w RGB}");
                return;
            }
            await Context.Guild.CreateRoleAsync(commandArgs[0], color: new Discord.Color(uint.Parse(commandArgs[1], System.Globalization.NumberStyles.HexNumber)));
            await ReplyAsync($"Udało ci się stworzyć rolę: **{commandArgs[0]}**");
        }

        [Command("Delete")]
        [Summary("Komenda służąca do usunięcia roli na serwerze")]
        [Remarks("$Role Delete {nazwa}")]
        public async Task DeleteRole(string roleName)
        {
            if(Context.Guild.Roles.FirstOrDefault(r => r.Name == roleName) is var role && role.Permissions.Administrator)
            {
                await ReplyAsync("Nie możesz usunąć żadnej z ról administratorskich");
                return;
            }
            await role.DeleteAsync();
            await ReplyAsync($"Pomyślnie udało się usunąć rolę: {role.Name}");
        }

        [Command("Assign")]
        [Summary("Komenda służąca do nadania użytkownikowi roli")]
        [Remarks("$Role Assign {nazwa}")]
        public async Task AssignRoleToUser(string roleName)
        {
            if(roleName == RoleFactory.GetRoleName(RoleTypeEnum.Buk))
            {
                await ReplyAsync("No już bez przesady, Buk jest tylko jeden ;)");
                return;
            }
            if(roleName == RoleFactory.GetRoleName(RoleTypeEnum.ZastepcaBoka) && 
                (SocketGuildUser)Context.User is SocketGuildUser user &&
                !user.Roles.Any(r => r.Name == RoleFactory.GetRoleName(RoleTypeEnum.Buk)))
            {
                await ReplyAsync("Tylko Buk może wyznaczać swoich zastępców");
                return;
            }
        }
    }
}
