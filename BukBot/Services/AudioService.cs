using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;


namespace BukBot.Services
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> _audioClients;
        private IAudioClient _audioClient;

        public AudioService()
        {
            _audioClients = new ConcurrentDictionary<ulong, IAudioClient>();
        }

        public async Task JoinChannelAsync(IVoiceChannel channel, ulong guildID)
        {
         _audioClient = await channel.ConnectAsync();
            var lol = _audioClient.ConnectionState;
         //   _audioClients.TryAdd(guildID, audioClient);
        }

        public async Task LeaveChannelAsync(SocketCommandContext Context)
        {
            if (_audioClients.TryGetValue(Context.Guild.Id, out IAudioClient audioClient))
            {
                try
                {
                    await audioClient.StopAsync();
                    _audioClients.TryRemove(Context.Guild.Id, out audioClient);
                }
                catch (Exception e)
                {
                    throw new Exception("Disconnected");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("I'm not connected");
            }
        }

        public async Task SendAudioAsync(SocketGuild guild, ISocketMessageChannel channel, string filePath)
        {
            // string filePath = $"Data/Music/{path}.mp3";

            //if (!File.Exists(filePath))
            //{
            //    await channel.SendMessageAsync("File does not exist.");
            //    return;
            //}

            //if (_audioClients.TryGetValue(guild.Id, out IAudioClient client))
            //{
            using (Stream output = CreateStream(filePath).StandardOutput.BaseStream)
            using (AudioOutStream stream = _audioClient.CreatePCMStream(AudioApplication.Music))
            {
                try
                {
                    await output.CopyToAsync(stream);
                }
                catch (Exception e)
                {
                    throw new Exception("Stopped audio stream");
                }
                finally
                {
                    await stream.FlushAsync();
                }
            }
        }
    

        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = @"C:\Users\apaz02\Downloads\ffmpeg-20200213-6d37ca8-win64-static\ffmpeg-20200213-6d37ca8-win64-static\bin\ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }

    }
}