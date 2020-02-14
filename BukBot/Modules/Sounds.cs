using BukBot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BukBot.Modules
{
    [Group("Sounds")]
    public class Sounds : ModuleBase<SocketCommandContext>
    {
        private readonly AudioService _audioService;
        public Sounds(AudioService audioService) => _audioService = audioService;

        [Command("")]
        [Summary("Wyświetla listę soundów")]
        [Remarks("$Sounds")]
        public async Task ShowListOfSounds()
        {
            if (new DirectoryInfo(await GetSoundsPathAsync()).GetFiles() is var filesInDirectory && filesInDirectory.Length == 0)
            {
                await ReplyAsync("Nie ma żadnych soundów");
                return;
            }

            await ReplyAsync($"**Soundy**:\n **\\*** {string.Join("\n **\\*** ", filesInDirectory.Select(f => Path.GetFileNameWithoutExtension(f.Name)))}");
        }

        [Command("Play", RunMode = RunMode.Async)]
        [Summary("Gra sound")]
        [Remarks("$Sounds Play {nazwa sounda}")]
        public async Task PlaySound(string soundName)
        {
            if (new DirectoryInfo(await GetSoundsPathAsync()).GetFiles() is var filesInDirectory && filesInDirectory.Length == 0)
            {
                await ReplyAsync("Nie ma żadnych soundów");
                return;
            }

            if (filesInDirectory.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f.Name) == soundName) is var file && file == null)
            {
                await ReplyAsync("Nie ma takiego sounda");
                return;
            }

            if ((Context.User as SocketGuildUser)?.VoiceChannel is var voiceChannel && voiceChannel == null)
            {
                await ReplyAsync("Nie jesteś na kanale głosowym");
                return;
            }

            if (Context.Guild.CurrentUser.VoiceChannel != voiceChannel)
            {
                await _audioService.JoinChannelAsync(voiceChannel, Context.Guild.Id);
            }

            await _audioService.SendAudioAsync(Context.Guild, Context.Channel, file.FullName);
            
        }

        [Command("Add")]
        [Summary("Dodaje sound")]
        [Remarks("$Sounds Add {nazwa sounda}")]
        public async Task AddSound(string soundName)
        {
            if (Context.Message.Attachments.FirstOrDefault() is var attachment && attachment == null)
            {
                await ReplyAsync("Nie było żadnego załącznika");
                return;
            }

            var stream = new WebClient().OpenRead(attachment.Url);
            using (Stream file = File.Create($"{await GetSoundsPathAsync()}{soundName}.{attachment.Filename.Split('.')[1]}"))
            {
                CopyStream(stream, file);
            }
            await ReplyAsync($"Udało się pomyślnie dodać sound: **{soundName}**");
        }

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("Delete")]
        [Summary("Usuwa sounda")]
        [Remarks("$Sounds Delete {nazwa sounda")]
        public async Task DeleteSound(string soundName)
        {
            if (new DirectoryInfo(await GetSoundsPathAsync()).GetFiles() is var filesInDirectory && filesInDirectory.Length == 0)
            {
                await ReplyAsync("Nie ma żadnych soundów");
                return;
            }

            if (filesInDirectory.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f.Name) == soundName) is var file && file == null)
            {
                await ReplyAsync("Nie ma takiego sounda");
                return;
            }

            file.Delete();
            await ReplyAsync($"Usunięto sound: {soundName}");
        }

        private async Task<string> GetSoundsPathAsync() => (await ConfigService.GetFilePathsAsync()).SoundsPath;

        private void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }
    }
}