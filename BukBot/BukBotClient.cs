using BukBot.Models;
using BukBot.Models.DbModels;
using BukBot.Repositories;
using BukBot.Repositories.Interfaces;
using BukBot.Services;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Victoria;

namespace BukBot
{
    public class BukBotClient
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly LogService _logger;
        private IServiceProvider _services;
        private ServerConfig _serverConfig;

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
            _logger = new LogService(_client, _commandService);
        }

        public async Task InitializeAsync()
        {
            _serverConfig = await ConfigService.GetConfigAsync();
            _services = SetupServices();
            await _logger.StartLoggingAsync();
            await _client.LoginAsync(TokenType.Bot, _serverConfig.Token);
            await _client.StartAsync();
            var commandHandler = new CommandHandler(_client, _commandService, _services);
            await commandHandler.InitializeAsync();
            var memberAssigmentService = new MemberAssignmentService(_client, _serverConfig.MemberAssignmentRole);
            await memberAssigmentService.InitializeAsync();

            await Task.Delay(-1);
        }

        private IServiceProvider SetupServices() => new ServiceCollection()
            .AddDbContext<AppDbContext>(options => options.UseNpgsql("User ID =postgres;Password=1234;Server=localhost;Port=5432;Database=BukBotDb;Integrated Security = true; Pooling=true;"))
            .AddSingleton(_client)
            .AddSingleton(_commandService)
            .AddSingleton<InteractiveService>()
            .AddSingleton<LavaConfig>()
            .AddSingleton<LavaNode>()
            .AddScoped<ISoundRepository, SoundRepository>()
            .BuildServiceProvider();
    }
}
