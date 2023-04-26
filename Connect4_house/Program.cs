using DSharpPlus;
using Microsoft.Extensions.Logging;
using DSharpPlus.SlashCommands;
using static System.Runtime.InteropServices.JavaScript.JSType;
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
            Connect4Game game = new Connect4Game();
            game.DropCoin(1, PlayerType.TEAM);
            game.DropCoin(4, PlayerType.TEAM);
            game.DropCoin(3, PlayerType.TEAM);
            game.DropCoin(2, PlayerType.TEAM);
            game.PrintBoard();

            //(args).ConfigureAwait(false).GetAwaiter().GetResult();
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