using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BukBot.Modules
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commandService;
        public Help(CommandService commandService) => _commandService = commandService;

        [Command("Help")]
        [Summary("Komenda służąca do wyświetlania wszystkich komend oraz ich składni")]
        [Remarks("$Help")]
        public async Task OverallHelp()
        {
            List<CommandInfo> commands = _commandService.Commands.ToList();
            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.AddField("Ogólne", "klamry {} oznaczają parametr. Parametr, który posiada spację trzeba wziąć w cudzysłów **\"przykładowy parametr ze spacją\"** \n Wszelkie problemy i bugi zgłaszaj do Boka(**Woolfer#5654**)");
            foreach (var command in commands)
            {
                var commandName = command.Module.Group != null ? $"{command.Module.Group} {command.Name}" : command.Name;
                var commandSyntax = command.Remarks ?? "Brak składni komendy";
                var commandSummary = command.Summary ?? "Brak opisu komendy\n";
                embedBuilder.AddField($"__{commandName}__ ", $"**{commandSyntax}**\n{commandSummary}");
            }
            await ReplyAsync("Lista komend wraz z ich opisem oraz składnią", false, embedBuilder.Build());
        }
    }
}
