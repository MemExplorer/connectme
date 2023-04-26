using Connect4_house.GameLogic.Structures;
using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4_house.Commands.GameCommandsModule.Structures
{
    internal class DiscordConnect4Team
    {
        public DiscordRole Role { get; private set; }
        public DiscordChannel Channel { get; private set; }
        public PlayerType PlayerType { get; private set; }
        public DiscordButtonComponent buttonComponent { get; private set; } //button id is also the channel id
        public DiscordMessageBuilder BoardMessageContents { get; set; }
        public DiscordMessage BoardMessage { get; set; }
        public async Task Initialize(DiscordGuild guild, DiscordMember m, DiscordChannel category, string roleName, DiscordColor roleColor, PlayerType p)
        {
            PlayerType = p;
            Role = await guild.CreateRoleAsync(roleName, color:  roleColor);
            List<DiscordOverwriteBuilder> builderList = new List<DiscordOverwriteBuilder>
            {
                new DiscordOverwriteBuilder(Role).Allow(DSharpPlus.Permissions.UseVoice),
                new DiscordOverwriteBuilder(guild.EveryoneRole).Deny(DSharpPlus.Permissions.UseVoice)
            };
            Channel = await guild.CreateChannelAsync(roleName + " Team", DSharpPlus.ChannelType.Voice ,category, overwrites: builderList);
            buttonComponent = new DiscordButtonComponent(ButtonStyle.Primary, Channel.Id.ToString() + "|" + m.Id, roleName + " Team");

        }

        public async Task Dispose()
        {
            await Role.DeleteAsync();
            await Channel.DeleteAsync();
        }
    }
}
