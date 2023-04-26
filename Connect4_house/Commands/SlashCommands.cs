using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4_house.Commands
{
    public class SlashCommands : ApplicationCommandModule
    {
        [SlashCommand("rules", "A slash command made to display the rules of the game.")]
        public async Task RulesCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("1. Players must connect 4 of the same colored discs in a row to win." +
               "\n2. Only one piece is played at a time." +
               "\n3. Players can be on the offensive or defensive." +
               "\n4. The game ends when there is a 4-in-a-row or a stalemate." +
               "\n5. The starter of the previous game goes second on the next game.").AsEphemeral());
        }
    }
}
