using System.Threading.Tasks;

namespace BukBot
{
	public class Program
	{
		public static async Task Main(string[] args)
			=> await new BukBotClient().InitializeAsync();
	}
}
