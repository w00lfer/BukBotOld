﻿using Discord;
using System;
using System.Threading.Tasks;

namespace BukBot
{
    class Logger
    {
        public Task LogAsync(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}