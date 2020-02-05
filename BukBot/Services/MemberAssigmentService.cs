using Discord.WebSocket;
using System.Threading.Tasks;

namespace BukBot.Services
{
    public class MemberAssignmentService
    {
        private readonly ulong _roleId;
        private readonly DiscordSocketClient _client;
        public MemberAssignmentService(DiscordSocketClient client, ulong roleId)
        {
            _client = client;
            _roleId = roleId;
        }
        
        public void Initialize() => _client.UserJoined += AssignMemberAsync;


        private async Task AssignMemberAsync(SocketGuildUser guildUser)
        {
            var role = guildUser.Guild.GetRole(_roleId);
            if (role == null) return;
            await guildUser.AddRoleAsync(role);
        }
    }
}
