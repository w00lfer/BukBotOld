using Discord.Addons.Interactive;
using Discord.Commands;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BukBot.Modules
{
    [Group("Meme")]
    public class Meme : InteractiveBase<SocketCommandContext>
    {
        [Command("")]
        [Summary("")]
        [Remarks("")]
        public async Task ShowListOfMemes()
        {

        }

        [Command("Add")]
        [Summary("")]
        [Remarks("")]
        public async Task AddMeme(string name)
        {
            using (var client = new HttpClient())
            {
                var file = await client.GetStringAsync(Context.Message.Attachments.First().Url);
                var stream = new WebClient().OpenRead(Context.Message.Attachments.First().Url);

                var img = Image.FromStream(stream, false, false);
                img.Save(@$"C:\users\apaz02\desktop\{name}.png", ImageFormat.Png);
                await ReplyAsync(await client.GetStringAsync(Context.Message.Attachments.First().Url));
            }
        }
    }
}
