using DSharpPlus;
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

        [SlashCommand("changeteam", "Changes the specified user's team.")]
        public async Task ChangeUserTeam(InteractionContext ctx, 
            [Option("user", "User to change team.")] DiscordUser user,
            [Choice("Red Team", 0)]
            [Choice("Yellow Team", 1)]
            [Option("Team", "Select from the list of teams.")]
            long teamCode)
        {
            DiscordMember m = await ctx.Guild.GetMemberAsync(user.Id);
            if (GameManager.GameInstances.TryGetValue(ctx.Member, out Connect4DiscordGame g))
                await g.ChangeTeam(ctx, m, teamCode);
            else
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Game does not exist!").AsEphemeral());
        }

        [SlashCommand("start", "Starts a Connect4 Game.")]
        public async Task StartGame(InteractionContext ctx)
        {
            if (GameManager.GameInstances.TryGetValue(ctx.Member, out Connect4DiscordGame g))
                await g.StartGame(ctx.Interaction);
            else
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("You need to create your own game first!").AsEphemeral());
        }

        [SlashCommand("delgame", "Deletes the game that you created.")]
        public async Task GameDeleteCommand(InteractionContext ctx)
        {
            if (GameManager.GameInstances.TryGetValue(ctx.Member, out Connect4DiscordGame g))
            {
                await GameManager.TryDeleteGameInstance(ctx.Interaction, ctx.Member);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Successfully deleted game!").AsEphemeral());
            }
            else
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("You need to create your own game first!").AsEphemeral());
        }


        [SlashCommand("reset","Restart the Game")]
        public async Task ResetGame(InteractionContext ctx)
        {
            if (GameManager.GameInstances.TryGetValue(ctx.Member, out Connect4DiscordGame g) && g.started)
            {
                bool res = await GameManager.TryResetGameInstance(ctx.Interaction, ctx.Member);
                string message = (res) ? "Reset Successfully." : "Failed to reset the game.";
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent(message).AsEphemeral());
                if(res && GameManager.GameInstances.TryGetValue(ctx.Member, out g))
                    await g.StartGame(ctx.Interaction, true);
            }
            else if(!g.started)
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("You can only reset if the game has started!").AsEphemeral());
            else
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("You need to create your own game first!").AsEphemeral());
        }
    }
}
