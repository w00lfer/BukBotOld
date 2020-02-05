using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BukBot.PreconditionAttributes
{
    public class RequireRoleAttribute : PreconditionAttribute
    {
        private readonly string _roleName;

        public RequireRoleAttribute(string roleName)
        {
            _roleName = roleName;
        }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context,
            CommandInfo command, IServiceProvider services)
        {
            var guildUser = context.User as SocketGuildUser;
            if (guildUser == null)
                return PreconditionResult.FromError("Komenda nie może zostać wykonana poza serwerem Memiarze Areczka.");

            var guild = guildUser.Guild;
            if (guild.Roles.All(r => r.Name != _roleName))
                return PreconditionResult.FromError(
                    $"Na serwerze nie ma takiej roli jak: ({_roleName}) wymaganej do wykonania komendy.");

            return guildUser.Roles.Any(r => r.Name == _roleName)
                ? PreconditionResult.FromSuccess()
                : PreconditionResult.FromError("Nie posiadasz wymaganej roli do wykonanai komendy.");
        }
    }
}
