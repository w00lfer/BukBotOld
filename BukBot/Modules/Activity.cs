using BukBot.Enums;
using BukBot.Models;
using BukBot.PreconditionAttributes;
using BukBot.Services;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace BukBot.Modules
{
    [RequireRole(RoleTypeEnum.Dupa)]
    [Group("Activity")]
    public class Activity : ModuleBase<SocketCommandContext>
    {
        [Command("Commands")]
        [Summary("Komenda służąca do wyświetlenia ilości uzyć poszczególnych komend")]
        [Remarks("$Activity Commands")]
        public async Task ShowUserCommandsActivity()
        {
            string logPath = (await GetLogPaths()).UsersCommandsActivityPath;
        }

        [Command("VoiceChannels")]
        [Summary("Komenda służąca do wyświetlenia ilości godzin spędzonych na kanałach głosowych przez użytkowników")]
        [Remarks("$Activity VoiceChannels")]
        public async Task ShowUserVoiceChannelsActivity()
        {
            string logPath = $"{(await GetLogPaths()).UsersVoiceChannelsActivityPath}{DateTime.Today.ToString("MMMM", CultureInfo.InvariantCulture)}.txt";

            List<Data> current = new List<Data>();
            List<Proccesed> finished = new List<Proccesed>();

            using (StreamReader sr = File.OpenText(logPath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string lineTime = ""; // regex for date
                    string userName = ""; // regex for username
                    string command = ""; //regex for command

                    if (command.Contains("Joined"))//ktos dolaczyl
                    {
                        current.Add(new Data() { Id = userName, Times = new Tuple<string, string>(lineTime, "") });//dodaj go
                    }
                    else if (command.Contains("Disconnected"))//ktos wyszedl
                    {
                        foreach (Data user in current)
                        {
                            if (user.Id == userName)
                            {
                                user.Times = new Tuple<string, string>(user.Times.Item1, lineTime);//tutaj overkill
                                finished.Add(new Proccesed() { Id = user.Id, Elapsed = user.Times.Item1 + user.Times.Item2 });// w drugiej linijce np. roznica czasow
                                current.Remove(user);//wywalmy goscia z listy przeszukiwanych
                                break;
                            }
                        }
                    }
                }
            }
            //pod koniec mamy w finished pary:
            // Woolfer 00:40
            // Woolfer 00:20
            // Ariello 00:10
            // itd...
        }

        public async Task<LogPaths> GetLogPaths() => await ConfigService.GetLogPathsAsync();
    }

    internal class Data
    {
        public string Id { get; set; }
        public Tuple<string, string> Times { get; set; }// wystarczy jeden Time object, ale chcialem sie pobawic
    }

    internal class Proccesed//wystarczy jedna klasa, patrz uwaga wyżej
    {
        public string Id { get; set; }
        public string Elapsed { get; set; }
    }

}

