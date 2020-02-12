using BukBot.Enums;
using BukBot.Helpers;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace BukBot.Services
{
    public class MemberAssignmentService
    {
        private readonly string _roleName;
        private readonly DiscordSocketClient _client;
        public MemberAssignmentService(DiscordSocketClient client, RoleTypeEnum roleTypeEnum)
        {
            _client = client;
            _roleName = RoleFactory.GetRoleName(roleTypeEnum);
        }
        
        public async Task InitializeAsync() => _client.UserJoined += AssignMemberAsync;

 
        private async Task AssignMemberAsync(SocketGuildUser guildUser)
        {

            var role = guildUser.Guild.Roles.Where(r => r.Name == _roleName).FirstOrDefault();
            if (role == null) return;
            await guildUser.AddRoleAsync(role);
        }
    }
}
