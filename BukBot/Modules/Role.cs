using BukBot.Enums;
using BukBot.Helpers;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace BukBot.Modules
{
    [RequireUserPermission(GuildPermission.Administrator)]
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
            await Context.Guild.CreateRoleAsync(commandArgs[0], color: new Color(uint.Parse(commandArgs[1], System.Globalization.NumberStyles.HexNumber)));
            await ReplyAsync($"Udało ci się stworzyć rolę: **{commandArgs[0]}**");
        }

        [Command("Delete")]
        [Summary("Komenda służąca do usunięcia roli na serwerze")]
        [Remarks("$Role Delete {rola}")]
        public async Task DeleteRole(string roleName)
        {
            var role = Context.Guild.Roles.FirstOrDefault(r => r.Name == roleName);
            if (!await ValidateObjectIfItsNotNullAsync(role, "Podana rola nie istnieje na serwerze")) return;

            if (role.Permissions.Administrator)
            {
                await ReplyAsync("Nie możesz usunąć żadnej z ról administratorskich");
                return;
            }
            await role.DeleteAsync();
            await ReplyAsync($"Pomyślnie udało się usunąć rolę: **{role.Name}**");
        }

        [Command("Assign")]
        [Summary("Komenda służąca do nadania użytkownikowi roli")]
        [Remarks("$Role Assign {osoba} {rola}")]
        public async Task AssignRoleToUser(params string[] commandArgs)
        {
            if (commandArgs?.Length != 2)
            {
                await ReplyAsync("Dzban, $Role Assign {osoba} {rola}");
                return;
            }

            var user = Context.Guild.Users.FirstOrDefault(u => u.Username == commandArgs[0]);
            if (!await ValidateObjectIfItsNotNullAsync(user, "Podany user nie istnieje na serwerze")) return;

            var role = Context.Guild.Roles.FirstOrDefault(r => r.Name == commandArgs[1]);
            if (!await ValidateObjectIfItsNotNullAsync(role, "Podana rola nie istnieje na serwerze")) return;

            if (role.Permissions.Administrator &&
                (SocketGuildUser)Context.User is var invoker &&
                !invoker.Roles.Any(r => r.Name == RoleFactory.GetRoleName(RoleTypeEnum.Buk)))
            {
                await ReplyAsync("Tylko administrator może dawać role administratorskie");
                return;
            }

            await user.AddRoleAsync(role);
            await ReplyAsync($"Pomyślnie udało się dać rolę **{role.Name}** użytkownikowi **{user.Username}**");
        }

        [Command("Unassign")]
        [Summary("Komenda służąca do zabierania użytkownikowi roli")]
        [Remarks("$Role Unassign {osoba} {rola}")]
        public async Task UmassignRoleFromUser(params string[] commandArgs)
        {
            if (commandArgs?.Length != 2)
            {
                await ReplyAsync("Dzban, $Role Unassign {osoba} {rola}");
                return;
            }

            var user = Context.Guild.Users.FirstOrDefault(u => u.Username == commandArgs[0]);
            if (!await ValidateObjectIfItsNotNullAsync(user, "Podany user nie istnieje na serwerze")) return;

            var role = Context.Guild.Roles.FirstOrDefault(r => r.Name == commandArgs[1]);
            if (!await ValidateObjectIfItsNotNullAsync(role, "Podana rola nie istnieje na serwerze")) return;

            if (role.Permissions.Administrator &&
                (SocketGuildUser)Context.User is var invoker &&
                !invoker.Roles.Any(r => r.Name == RoleFactory.GetRoleName(RoleTypeEnum.Buk)))
            {
                await ReplyAsync("Tylko administrator może zabierać role administratorskie");
                return;
            }

            await user.RemoveRoleAsync(role);
            await ReplyAsync($"Pomyślnie udało się zabrać rolę **{role.Name}** użytkownikowi **{user.Username}**");
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
