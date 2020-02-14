using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BukBot.Services
{
    public class LogService
    {
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;

        public LogService(DiscordSocketClient client, CommandService commandService)
        {
            _semaphoreSlim = new SemaphoreSlim(1);
            _client = client;
            _commandService = commandService;
        }

        public async Task StartLoggingAsync()
        {
            _commandService.Log += LogAsync;
            _commandService.CommandExecuted += LogUserCommandsActivityToFileAsync;
            _client.Log += LogAsync;
            _client.UserVoiceStateUpdated += LogUsersVoiceChannelsActivityToFileAsync;
        }

        private async Task LogAsync(LogMessage arg)
        {
            await _semaphoreSlim.WaitAsync();

            var timeStamp = DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt");
            const string format = "{0,-10} {1,10}";

            Console.WriteLine($"[{timeStamp}] {string.Format(format, arg.Source, $": {arg.Message}")}");

            _semaphoreSlim.Release();
        }
        private async Task LogUsersVoiceChannelsActivityToFileAsync(SocketUser user, SocketVoiceState firstState, SocketVoiceState secondState)
        {
            var firstAction = firstState.VoiceChannel == null ? $"Joined to {firstState.VoiceChannel}" : $"{firstState.VoiceChannel} changed to ";
            var secondAction = secondState.VoiceChannel == null ? "Disconnected" : secondState.VoiceChannel.ToString();
            using (var file = new StreamWriter($@"{(await ConfigService.GetFilePathsAsync()).UsersVoiceChannelsActivityPath}{DateTime.Today.ToString("MMMM", CultureInfo.InvariantCulture)}.txt", true))
                await file.WriteLineAsync($"[{DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt")}] {string.Format("{0,-20} {1,-25}", $"<{user.Username}>", $": ({firstAction}{secondAction})")}");
        }
        private async Task LogUserCommandsActivityToFileAsync(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
        {
            using (var file = new StreamWriter($@"{(await ConfigService.GetFilePathsAsync()).UsersCommandsActivityPath}{DateTime.Today.ToString("MMMM", CultureInfo.InvariantCulture)}.txt", true))
                await file.WriteLineAsync($"[{DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt")}] {string.Format("{0,-20} {1,-100}", $"<{commandContext.User.Username}>", $": ({commandContext.Message.Content}{result.ErrorReason?? ""})")}");
        }
    }
}
