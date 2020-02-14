using BukBot.Helpers;
using BukBot.Services;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BukBot.Modules
{
    [Group("Memes")]
    public class Memes : InteractiveBase<SocketCommandContext>
    {
        [Command("")]
        [Summary("Wyświetla listę memów")]
        [Remarks("$Memes")]
        public async Task ShowListOfMemes()
        {
            if (new DirectoryInfo(await GetMemePathAsync()).GetFiles() is var filesInDirectory && filesInDirectory.Length == 0)
            {
                await ReplyAsync("Nie ma żadnych memów");
                return;
            }

            await ReplyAsync($"**Memy**:\n **\\*** {string.Join("\n **\\*** ", filesInDirectory.Select(f => Path.GetFileNameWithoutExtension(f.Name)))}");
        }

        [Command("Show")]
        [Summary("Wyświetla mema")]
        [Remarks("$Memes Show {nazwa mema}")]
        public async Task ShowMeme(string memeName)
        {
            if(new DirectoryInfo(await GetMemePathAsync()).GetFiles() is var filesInDirectory && filesInDirectory.Length == 0)
            {
                await ReplyAsync("Nie ma żadnych memów");
                return;
            }

            if(filesInDirectory.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f.Name) == memeName) is var file && file == null)
            {
                await ReplyAsync("Nie ma takiego mema");
                return;
            }

            var memesChannel = Context.Guild.Channels.FirstOrDefault(c => c.Name == "memy") as ISocketMessageChannel;
            await (memesChannel.SendFileAsync(file.FullName));
            await ReplyAsync($"Mem wylądował na kanał: **{memesChannel.Name}**");
        }

        [Command("Random")]
        [Summary("Wyświetla losowego mema z listy memów")]
        [Remarks("$Memes Random")]
        public async Task ShowRandomMeme()
        {
            if (new DirectoryInfo(await GetMemePathAsync()).GetFiles() is var filesInDirectory && filesInDirectory.Length == 0)
            {
                await ReplyAsync("Nie ma żadnych memów");
                return;
            }
            var randomMeme = filesInDirectory[new Random().Next(0, filesInDirectory.Length)];
            var memesChannel = Context.Guild.Channels.FirstOrDefault(c => c.Name == "memy") as ISocketMessageChannel;
            await (memesChannel.SendFileAsync(randomMeme.FullName));
            await ReplyAsync($"Randomowy mem wylądował na kanał: **{memesChannel.Name}**");
        }

        [Command("Add")]
        [Summary("Dodaje mema")]
        [Remarks("$Memes Add {nazwa mema}")]
        public async Task AddMeme(string memeName)
        {
                if (Context.Message.Attachments.FirstOrDefault() is var attachment && attachment == null)
                {
                    await ReplyAsync("Nie było żadnego załącznika");
                    return;
                }
                var imgFormat = ImageFormatFactory.GetImageFormat(attachment.Filename.Split('.')[1]);
                var stream = new WebClient().OpenRead(attachment.Url);
                var img = System.Drawing.Image.FromStream(stream, false, false);
                img.Save(@$"{await GetMemePathAsync()}{memeName}.{imgFormat.ToString()}", imgFormat);
                await ReplyAsync($"Udało się pomyślnie dodać mema: **{memeName}**");
                await Context.Message.DeleteAsync();
        }
        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("Delete")]
        [Summary("Usuwa mema")]
        [Remarks("$Memes Delete {nazwa mema")]
        public async Task DeleteMeme(string memeName)
        {
            if (new DirectoryInfo(await GetMemePathAsync()).GetFiles() is var filesInDirectory && filesInDirectory.Length == 0)
            {
                await ReplyAsync("Nie ma żadnych memów");
                return;
            }

            if (filesInDirectory.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f.Name) == memeName) is var file && file == null)
            {
                await ReplyAsync("Nie ma takiego mema");
                return;
            }

            file.Delete();
            await ReplyAsync($"Usunięto mema: {memeName}");
        }

        private async Task<string> GetMemePathAsync() => (await ConfigService.GetFilePathsAsync()).MemesPath;
    }
}
