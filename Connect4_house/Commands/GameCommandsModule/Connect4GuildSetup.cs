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
        public DiscordConnect4Team RedTeam { get; private set; }
        public DiscordConnect4Team YellowTeam { get; private set; }
        
        public Connect4GuildSetup(DiscordGuild guild)
        {
            _guild = guild;
            RedTeam = new DiscordConnect4Team();
            YellowTeam = new DiscordConnect4Team();
            
        }

        public async Task InitializeTeam(DiscordMember owner)
        {
            _channelCategory = await _guild.CreateChannelCategoryAsync("Game Category " + owner.DisplayName);
            await RedTeam.Initialize(_guild, owner, _channelCategory, "Red", DiscordColor.Red, PlayerType.RED);
            await YellowTeam.Initialize(_guild, owner, _channelCategory, "Yellow", DiscordColor.Yellow, PlayerType.YELLOW);
        }

        public async Task Dispose()
        {
            await RedTeam.Dispose();
            await YellowTeam.Dispose();
            await _channelCategory.DeleteAsync();
        }
    }
}
