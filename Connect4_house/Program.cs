using DSharpPlus;
using Microsoft.Extensions.Logging;
using Connect4_house.Commands.GameCommandsModule;
using DSharpPlus.SlashCommands;
using Connect4_house.Commands;
using TinyINIController;

namespace Connect4_house
{
    internal class Program
    {
        static DiscordClient _discordClient { get; set; }

        static void Main(string[] args)
        {
            Console.Title = "ConnectMe";

            //read config
            IniFile config = new IniFile();
            BotConfiguration.Token = config.Read("token");
            BotConfiguration.BotName = config.Read("name");

            if(BotConfiguration.Token.Length == 0 && BotConfiguration.BotName.Length == 0)
            {
                config.Write("token", "");
                config.Write("name", "");
                Console.WriteLine("A configuration file has been generated in the same directory as this executable.");
                Console.WriteLine("Please mofify the configuration to run the bot.");
                Console.WriteLine();
                Console.Write("Press any key to continue...");
                Console.ReadKey();
                return;
            }

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