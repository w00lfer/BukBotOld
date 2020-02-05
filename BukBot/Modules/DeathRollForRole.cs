using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Addons.Interactive;

namespace BukBot.Modules
{
	public class DeathRollForRole : InteractiveBase<SocketCommandContext>
	{
		[Command("DeathRollForRole", RunMode = RunMode.Async)]
		public async Task DeathRollForRoleBetweenUsers(params string[] commandArgs)
		{
			if (commandArgs?.Length != 4)
			{
				await ReplyAsync("Dzban, $DeathRollForRole {osoba1} {osoba2} {maxRoll} {rola}");
				return;
			}

			var firstUserFromMessage = commandArgs[0];
			var secondUserFromMessage = commandArgs[1];
			var maxRollFromMessage = commandArgs[2];
			var roleFromMessage = commandArgs[3];

			var firstUser = Context.Guild.Users.FirstOrDefault(u => u.Username == firstUserFromMessage);
			if (!await ValidateObjectIfItsNotNull(firstUserFromMessage, "Dzban, taki user nie istnieje na serwerze")) return;

			var secondUser = Context.Guild.Users.FirstOrDefault(u => u.Username == secondUserFromMessage);
			if (!await ValidateObjectIfItsNotNull(secondUserFromMessage, "Dzban, taki user nie istnieje na serwerze")) return;

			if (!int.TryParse(maxRollFromMessage, out int maxRoll) && maxRoll <= 1)
			{
				await ReplyAsync("Dzban, musisz podać liczbę naturalną większą od 1");
				return;
			}
			await PerformDeathRoll(firstUser, secondUser, maxRoll, roleFromMessage);
		}
		private async Task<bool> ValidateObjectIfItsNotNull(object objectToCheck, string errorMessage)
		{
			if (objectToCheck == null)
			{
				await ReplyAsync(errorMessage);
				return false;
			}
			return true;
		}
		private async Task PerformDeathRoll(SocketGuildUser currentRoller, SocketGuildUser nextRoller, int maxRoll, string role)
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
				else await ReplyAsync($"{currentRoller.Username} dzban, musisz wpisać Roll");
			}
			await ReplyAsync($"{currentRoller.Mention} wygrał rolę: {role}");
		}

		private void ChangeRollers(ref SocketGuildUser currentRoller, ref SocketGuildUser nextRoller) =>
			(currentRoller, nextRoller) = (nextRoller, currentRoller);
	}
}
