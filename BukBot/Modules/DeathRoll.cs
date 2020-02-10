using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Addons.Interactive;

namespace BukBot.Modules
{
	public class DeathRoll : InteractiveBase<SocketCommandContext>
	{
		[Command("DeathRoll", RunMode = RunMode.Async)]
		[Summary("Komenda do gry w death roll. Gra polega na rollowaniu przez graczy coraz to mniejszych liczb, aż któryś z nich wylosuje 1 i przegra")]
		[Remarks("$DeathRoll {gracz1} {gracz2} {maxRoll} {nagroda}")]
		public async Task StartDeathRoll(params string[] commandArgs)
		{
			if (commandArgs?.Length != 4)
			{
				await ReplyAsync("Dzban, $DeathRollForRole {osoba1} {osoba2} {maxRoll} {nagroda}");
				return;
			}

			var roleFromMessage = commandArgs[3];

			var firstUser = Context.Guild.Users.FirstOrDefault(u => u.Username == commandArgs[0]);
			if (!await ValidateObjectIfItsNotNullAsync(firstUser, "Dzban, taki user nie istnieje na serwerze")) return;

			var secondUser = Context.Guild.Users.FirstOrDefault(u => u.Username == commandArgs[1]);
			if (!await ValidateObjectIfItsNotNullAsync(secondUser, "Dzban, taki user nie istnieje na serwerze")) return;

			if (!int.TryParse(commandArgs[2], out int maxRoll) && maxRoll <= 1)
			{
				await ReplyAsync("Dzban, musisz podać liczbę naturalną większą od 1");
				return;
			}
			await PerformDeathRollsAsync(firstUser, secondUser, maxRoll, commandArgs[3]);
		}
		private async Task<bool> ValidateObjectIfItsNotNullAsync(object objectToCheck, string errorMessage)
		{
			if (objectToCheck == null)
			{
				await ReplyAsync(errorMessage);
				return false;
			}
			return true;
		}
		private async Task PerformDeathRollsAsync(SocketGuildUser currentRoller, SocketGuildUser nextRoller, int maxRoll, string prize)
		{
			var rnd = new Random();
            await ReplyAsync($"Zaczynamy rollowanie, startuje: {currentRoller}");
			while (maxRoll != 1)
			{
				var userRoll = await NextMessageAsync(false);
				if (userRoll.Content == "Roll" && userRoll.Author == currentRoller)
				{
					maxRoll = rnd.Next(maxRoll) + 1;
					await ReplyAsync($"{currentRoller.Username} roll za: {maxRoll}");
					ChangeRollers(ref currentRoller, ref nextRoller);
				}
				else await ReplyAsync($"{currentRoller.Username} dzban, musi wpisać Roll");
			}
			await ReplyAsync($"{currentRoller.Mention} wygrał: {prize}");
		}

		private void ChangeRollers(ref SocketGuildUser currentRoller, ref SocketGuildUser nextRoller) =>
			(currentRoller, nextRoller) = (nextRoller, currentRoller);
	}
}
