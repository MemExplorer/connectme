using Connect4_house.Commands.GameCommandsModule.Structures;
using Connect4_house.GameLogic.Structures;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4_house.Commands.GameCommandsModule
{
    internal class Connect4GuildSetup
    {
        private DiscordGuild _guild;
        private DiscordChannel _channelCategory;
        public DiscordConnect4Team RedRole { get; private set; }
        public DiscordConnect4Team YellowRole { get; private set; }
        
        public Connect4GuildSetup(DiscordGuild guild)
        {
            _guild = guild;
            RedRole = new DiscordConnect4Team();
            YellowRole = new DiscordConnect4Team();
            
        }

        public async Task InitializeTeam()
        {
            _channelCategory = await _guild.CreateChannelCategoryAsync("Game Category");
            await RedRole.Initialize(_guild, _channelCategory, "Red", DiscordColor.Red, PlayerType.RED);
            await YellowRole.Initialize(_guild, _channelCategory, "Yellow", DiscordColor.Yellow, PlayerType.YELLOW);
        }

        public async Task Dispose()
        {
            await RedRole.Dispose();
            await YellowRole.Dispose();
            await _channelCategory.DeleteAsync();
        }
    }
}
