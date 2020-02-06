using BukBot.Models;
using System.IO;
using System.Threading.Tasks;

namespace BukBot.Services
{
    public class ConfigService
    {
        public static async Task<Config> GetConfigAsync(string configPath)
        {
            using (StreamReader sr = File.OpenText(configPath))
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(await sr.ReadToEndAsync());
            }
        }
    }
}
