using BukBot.Models;
using BukBot.Services;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BukBot.Modules
{
    [Group("Activity")]
    public class Activity : ModuleBase<SocketCommandContext>
    {
        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("Commands")]
        [Summary("Komenda służąca do wyświetlenia ilości uzyć poszczególnych komend")]
        [Remarks("$Activity Commands")]
        public async Task ShowUserCommandsActivity()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder
            {
                Title = "**Używane komendy**"
            };
            foreach (var commands in (await GetUsersCommandsActivitiesTimesAsync()).OrderByDescending(key => key.Value))
            {
                embedBuilder.AddField($"__{commands.Key}__ ", $"**{commands.Value}** razy");
            }
            await Context.User.SendMessageAsync(embed: embedBuilder.Build());
        }

        [Command("VoiceChannels")]
        [Summary("Komenda służąca do wyświetlenia ilości godzin spędzonych na kanałach głosowych przez użytkowników")]
        [Remarks("$Activity VoiceChannels")]
        public async Task ShowUserVoiceChannelsActivity()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder
            {
                Title = "**Najaktywniejsi użytkownicy**"
            };
            foreach (var user in (await GetUsersVoiceChannelsActivitiesTimesAsync()).OrderByDescending(key => key.Value).Take(5))
                embedBuilder.AddField($"__{user.Key}__ ", $"**{(int)TimeSpan.FromMinutes(user.Value).TotalHours} godzin i {TimeSpan.FromMinutes(user.Value).Minutes} minut**");
            await ReplyAsync(embed: embedBuilder.Build());
        }

        // Credits to Ariello for helping in designing algorithm
        private async Task<Dictionary<string, double>> GetUsersVoiceChannelsActivitiesTimesAsync()
        {
            var logPath = $"{(await GetLogPathsAsync()).UsersVoiceChannelsActivityPath}{DateTime.Today.ToString("MMMM", CultureInfo.InvariantCulture)}.txt";
            var current = new Dictionary<string, DateTime>();
            var finished = new Dictionary<string, double>();
            using (StreamReader sr = File.OpenText(logPath))
            {
                while (sr.ReadLine() is var line && line != null)
                {
                    if(!DateTime.TryParse(Regex.Match(line, @"(?<=\[).+?(?=\])").Value, out var time)) throw new Exception($"Cant parse line: {line}");
                    var userName = Regex.Match(line, @"(?<=\<).+?(?=\>)").Value;
                    var action = Regex.Match(line, @"(?<=\().+?(?=\))").Value;
                    if (action.Contains("Joined"))
                        current.Add(userName, time);
                    else if (action.Contains("Disconnected"))
                    {
                        foreach (var user in current)
                        {
                            if (user.Key == userName)
                            {
                                if (finished.ContainsKey(userName))
                                    finished[userName] = finished[userName] + (time - user.Value).TotalMinutes;
                                else
                                    finished.Add(userName, (time - user.Value).TotalMinutes);
                                current.Remove(user.Key);
                                break;
                            }
                        }
                    }
                }
            }
            return finished;
        }

        private async Task<Dictionary<string, int>> GetUsersCommandsActivitiesTimesAsync()
        {
            var logPath = $"{(await GetLogPathsAsync()).UsersCommandsActivityPath}{DateTime.Today.ToString("MMMM", CultureInfo.InvariantCulture)}.txt";
            var usedCommands = new Dictionary<string, int>();
            using (StreamReader sr = File.OpenText(logPath))
            {
                while (sr.ReadLine() is var line && line != null)
                {                   
                    var action = Regex.Match(line, @"(?<=\().+?(?=\))").Value.ToLower();
                    if (usedCommands.ContainsKey(action))
                        usedCommands[action] += 1;
                    else
                        usedCommands.Add(action, 1);
                }
            }
            return usedCommands;
        }

        public async Task<LogPaths> GetLogPathsAsync() => await ConfigService.GetLogPathsAsync();
    }
}

