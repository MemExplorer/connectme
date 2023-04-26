using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4_house.Commands.GameCommandsModule
{
    public class GameCommands : ApplicationCommandModule
    {
        [SlashCommand("creategame", "Creates a new instance of the game.")]
        public async Task CreateGame(InteractionContext ctx)
        {
            bool creationResult = await GameInstance.TryCreateGameInstance(ctx);
            if (!creationResult)
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("You have already created a game!").AsEphemeral());

        }

        [SlashCommand("join", "Join a Connect4 Game.")]
        public async Task JoinGame(InteractionContext ctx, [Option("user", "Creator of the Game")] DiscordUser user)
        {
            DiscordMember m = await ctx.Guild.GetMemberAsync(user.Id);
            if(GameInstance.GameInstances.TryGetValue(m, out Connect4DiscordGame g))
                await g.JoinGame(ctx);
            else
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Game does not exist!").AsEphemeral());
        }

        [SlashCommand("start", "Starts a Connect4 Game.")]
        public async Task StartGame(InteractionContext ctx)
        {
            if (GameInstance.GameInstances.TryGetValue(ctx.Member, out Connect4DiscordGame g))
                await g.StartGame(ctx);
            else
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("You need to create your own game first!").AsEphemeral());
        }

        [SlashCommand("delgame", "Deletes the game that you created.")]
        public async Task TestRoleDeleter(InteractionContext ctx)
        {
            if (GameInstance.GameInstances.TryGetValue(ctx.Member, out Connect4DiscordGame g))
            {
                await GameInstance.TryDeleteGameInstance(ctx);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Successfully deleted game!").AsEphemeral());
            }
            else
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("You need to create your own game first!").AsEphemeral());
        }


        [SlashCommand("reset","Restart the Game")]
        public async Task ResetGame(InteractionContext ctx)
        {
            if (GameInstance.GameInstances.TryGetValue(ctx.Member, out Connect4DiscordGame g))
                await g.ResetGame(ctx);
            else
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("You need to create your own game first!").AsEphemeral());
        }

        /*private static GameRoleMaker rolemaker = null;
        [SlashCommand("makerole", "role maker")]
        public async Task TestRoleMaker(InteractionContext ctx)
        {
            rolemaker = new GameRoleMaker(ctx.Guild);
            await rolemaker.InitializeTeam();
        }

        [SlashCommand("delrole", "role deleter")]
        public async Task TestRoleDeleter(InteractionContext ctx)
        {
            await rolemaker.Dispose();
        }*/
    }
}
