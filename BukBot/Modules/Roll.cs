using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BukBot.Modules
{
    public class Roll : ModuleBase<SocketCommandContext>
    {
        [Command("Roll")]
        [Summary("Komenda do rollowania n razy n rzeczy")]
        [Remarks("$Roll {ilość powtórzeń} {1 rzecz} {2 rzecz}... {n rzecz}")]
        public async Task PerformRoll(params string[] commandArgs)
        {
            if(commandArgs.Length <= 2)
            {
                await ReplyAsync("Ilość argumentów musi być większa od 2, $Roll {ilość powtórzeń} {1 rzecz} {2 rzecz}... {n rzecz}");
            }
            if (!int.TryParse(commandArgs[0], out var repetitions) && repetitions < 1)
            {
                await ReplyAsync("1 argument musi być liczbą naturalną większą lub równą 1");
                return;
            }
            var wonItems = new List<string>();
            var rnd = new Random();
            for (int i = 0; i < repetitions; i++)
                wonItems.Add(commandArgs[rnd.Next(1, commandArgs.Length - 1)]);
            await ReplyAsync($"wygrane to {string.Join(" ", wonItems)}");
            return;
            
        }
    }
}
