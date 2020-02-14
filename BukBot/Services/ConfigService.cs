using BukBot.Models;
using System.IO;
using System.Threading.Tasks;

namespace BukBot.Services
{
    public static class ConfigService
    {
        public static async Task<ServerConfig> GetConfigAsync()
        {
            using (StreamReader sr = File.OpenText(@"C:\Users\apaz02\Desktop\ServerConfig.json")) // TODO reading from env var
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<ServerConfig>(await sr.ReadToEndAsync());
            }
        }
        public static async Task<FilesPaths> GetFilePathsAsync()
        {
            using (StreamReader sr = File.OpenText(@"C:\Users\apaz02\Desktop\FIlesPaths.json")) // TODO reading from env var
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<FilesPaths>(await sr.ReadToEndAsync());
            }
        }
    }
}
