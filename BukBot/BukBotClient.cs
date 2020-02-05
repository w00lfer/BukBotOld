using Discord.Commands;
using Discord.WebSocket;
using Discord;
using System.Threading.Tasks;
using System;
using BukBot.Services;
using Discord.Addons.Interactive;
using Microsoft.Extensions.DependencyInjection;
using Victoria;
using BukBot.Modules;
using System.Linq;

namespace BukBot
{
    public class BukBotClient
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly LogService _logger;
        private IServiceProvider _services;

        public BukBotClient(DiscordSocketClient client = null, CommandService commandService = null)
        {
            _client = client ?? new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 50,
                LogLevel = LogSeverity.Debug
            });
            _commandService = commandService ?? new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                CaseSensitiveCommands = false    
            });
            _logger = new LogService();
        }

        public async Task InitializeAsync()
        {
            _client.Log += _logger.LogAsync;
            await _client.LoginAsync(TokenType.Bot, "NjczODIwNzU4MjQ5ODMyNDQ4.Xjfmow.HP8YahBBZmH_LAF9n0AFr6tmlUc");
            await _client.StartAsync();
            _services = SetupServices();
            var commandHandler = new CommandHandler(_client, _commandService, _services);
            await commandHandler.InitializeAsync();
            var memberAssigmentService = new MemberAssignmentService(_client, "dupa");
            await memberAssigmentService.InitializeAsync();

            await Task.Delay(-1);
        }

        private IServiceProvider SetupServices() => new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_commandService)
            .AddSingleton<LavaNode>()
            .AddSingleton<InteractiveService>()
            .AddSingleton<MemberAssignmentService>()
            .BuildServiceProvider();
    }
}
