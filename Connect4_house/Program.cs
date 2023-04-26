using DSharpPlus;
using Microsoft.Extensions.Logging;
using DSharpPlus.SlashCommands;
using Connect4_house.Commands;
using Connect4_house.GameLogic;
using Connect4_house.GameLogic.Structures;

namespace Connect4_house
{
    internal class Program
    {

        static DiscordClient _discordClient { get; set; }

        static void Main(string[] args)
        {

            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            _discordClient = new DiscordClient(new DiscordConfiguration
            {
                Intents = DiscordIntents.All,
                Token = BotConfiguration.Token,
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Debug
            });
            var slashExt = _discordClient.UseSlashCommands();
            slashExt.RegisterCommands<SlashCommands>();
            await _discordClient.ConnectAsync();

            await Task.Delay(-1);
        }
    }
}