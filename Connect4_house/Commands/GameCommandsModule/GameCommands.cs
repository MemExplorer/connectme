﻿using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Connect4_house.Commands.GameCommandsModule
{
    public class GameCommands : ApplicationCommandModule
    {
        [SlashCommand("creategame", "Creates a new instance of the game.")]
        public async Task CreateGame(InteractionContext ctx)
        {
            bool creationResult = await GameManager.TryCreateGameInstance(ctx);
            if (!creationResult)
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("You have already created a game!").AsEphemeral());

        }

        [SlashCommand("join", "Join a Connect4 Game.")]
        public async Task JoinGame(InteractionContext ctx, [Option("user", "Creator of the Game")] DiscordUser user)
        {
            DiscordMember m = await ctx.Guild.GetMemberAsync(user.Id);
            if(GameManager.GameInstances.TryGetValue(m, out Connect4DiscordGame g))
                await g.JoinGame(ctx);
            else
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Game does not exist!").AsEphemeral());
        }

        [SlashCommand("start", "Starts a Connect4 Game.")]
        public async Task StartGame(InteractionContext ctx)
        {
            if (GameManager.GameInstances.TryGetValue(ctx.Member, out Connect4DiscordGame g))
                await g.StartGame(ctx);
            else
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("You need to create your own game first!").AsEphemeral());
        }

        [SlashCommand("delgame", "Deletes the game that you created.")]
        public async Task TestRoleDeleter(InteractionContext ctx)
        {
            if (GameManager.GameInstances.TryGetValue(ctx.Member, out Connect4DiscordGame g))
            {
                await GameManager.TryDeleteGameInstance(ctx);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Successfully deleted game!").AsEphemeral());
            }
            else
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("You need to create your own game first!").AsEphemeral());
        }


        [SlashCommand("reset","Restart the Game")]
        public async Task ResetGame(InteractionContext ctx)
        {
            if (GameManager.GameInstances.TryGetValue(ctx.Member, out Connect4DiscordGame g))
                await g.ResetGame(ctx);
            else
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("You need to create your own game first!").AsEphemeral());
        }
    }
}
