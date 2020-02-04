using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace BukBot
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _services;
        private readonly Logger _logger;

        public CommandHandler(DiscordSocketClient client, CommandService commandService, IServiceProvider services)
        {
            _client = client;
            _commandService = commandService;
            _services = services;
            _logger = new Logger();
        }

        public async Task InitializeAsync()
        {
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            _commandService.Log += _logger.LogAsync;
            _client.MessageReceived += HandleMessageAsync;
        }

        private async Task HandleMessageAsync(SocketMessage socketMessage)
        {
            var argPos = 0;
            if (socketMessage.Author.IsBot) return;

            var userMessage = socketMessage as SocketUserMessage;
            if (userMessage is null) return;

            if (!(userMessage.HasMentionPrefix(_client.CurrentUser, ref argPos) ||
                userMessage.HasCharPrefix('$', ref argPos) ||
                userMessage.Author.IsBot))
                return;

            var context = new SocketCommandContext(_client, userMessage);
            var result = await _commandService.ExecuteAsync(context, argPos, _services); 
        }
    }
}
