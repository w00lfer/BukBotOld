using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Victoria;
using Victoria.Enums;

namespace BukBot.Modules
{
    public sealed class LavalinkModule : ModuleBase<SocketCommandContext>
    {
        private readonly LavaNode _lavaNode;

        public LavalinkModule(LavaNode lavaNode)
        {
            _lavaNode = lavaNode;
        }

        [Command("Join")]
        public async Task JoinAsync()
        {
            await _lavaNode.JoinAsync(((SocketGuildUser)Context.User).VoiceChannel, (ITextChannel)Context.Channel);
            await ReplyAsync($"Joined {((SocketGuildUser)Context.User).VoiceChannel} channel!");
        }

        [Command("Leave")]
        public async Task LeaveAsync()
        {
            await _lavaNode.LeaveAsync(((SocketGuildUser)Context.User).VoiceChannel);
            await ReplyAsync($"Left {((SocketGuildUser)Context.User).VoiceChannel} channel!");
        }

        [Command("Move")]
        public async Task MoveAsync()
        {
            await _lavaNode.MoveChannelAsync(((SocketGuildUser)Context.User).VoiceChannel);
            var player = _lavaNode.GetPlayer(Context.Guild);
            await ReplyAsync($"Moved from {player.VoiceChannel} to {((SocketGuildUser)Context.User).VoiceChannel}!");
        }

        [Command("Play")]
        public async Task PlayAsync([Remainder] string query)
        {
            var search = await _lavaNode.SearchYouTubeAsync(query);
            var track = search.Tracks.FirstOrDefault();

            var player = _lavaNode.HasPlayer(Context.Guild)
                ? _lavaNode.GetPlayer(Context.Guild)
                : await _lavaNode.JoinAsync(((SocketGuildUser)Context.User).VoiceChannel, (ITextChannel)Context.Channel);

            if (player.PlayerState == PlayerState.Playing)
            {
                player.Queue.Enqueue(track);
                await ReplyAsync($"Enqeued {track.Title}.");
            }
            else
            {
                await player.PlayAsync(track);
                await ReplyAsync($"Playing {track.Title}.");
            }
        }
    }
}