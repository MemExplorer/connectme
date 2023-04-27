using DSharpPlus;
using Microsoft.Extensions.Logging;
using Connect4_house.Commands.GameCommandsModule;
using DSharpPlus.SlashCommands;
using Connect4_house.Commands;

namespace Connect4_house
{
    internal class Program
    {
        static DiscordClient _discordClient { get; set; }

        static void Main(string[] args)
        {
            Console.Title = "MultiConnect4";
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

            _discordClient.Ready += _discordClient_Ready;

            var slashExt = _discordClient.UseSlashCommands();
            slashExt.RegisterCommands<GameCommands>();
            slashExt.RegisterCommands<SlashCommands>();
            _discordClient.ComponentInteractionCreated += Connect4DiscordGame.ButtonHandler;

            await _discordClient.ConnectAsync();
            await Task.Delay(-1);
        }

        private static Task _discordClient_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)
        {
            Console.WriteLine("Bot is online!");
            return Task.CompletedTask;
        }
    }
}