using Connect4_house.GameLogic;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4_house.Commands.GameCommandsModule
{
    internal class Connect4DiscordGame
    {
        private DiscordGuild _guild;
        private Connect4GuildSetup _guildSetup;
        private Connect4Game _game;
        private bool inited = false;
        public async Task InitializeGame(DiscordGuild guild)
        {
            inited = true;
            _guild = guild;
            _guildSetup = new Connect4GuildSetup(_guild);
            _game = new Connect4Game();
            await _guildSetup.InitializeTeam();
        }

        public async Task AssignTeam(DiscordInteraction ctx, DiscordMember member, long teamCode)
        {
            DiscordRole chosenRole = teamCode == 0 ? _guildSetup.RedRole.Role : _guildSetup.YellowRole.Role;
           if(!(member.Roles.Contains(_guildSetup.YellowRole.Role) && member.Roles.Contains(_guildSetup.RedRole.Role)))
            {
                await member.GrantRoleAsync(chosenRole);
                await ctx.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent($"You have joined the {chosenRole.Name}!").AsEphemeral());
            }
            else
                await ctx.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent($"You are already in the {chosenRole.Name}!").AsEphemeral());
        }

        public async Task ChangeTeamResponse(DiscordInteraction ctx, DiscordMember member, long teamCode)
        {
            DiscordRole chosenRole = teamCode == 0 ? _guildSetup.RedRole.Role : _guildSetup.YellowRole.Role;
            if (member.Roles.Contains(_guildSetup.RedRole.Role) && chosenRole == _guildSetup.YellowRole.Role)
            {
                await member.RevokeRoleAsync(_guildSetup.RedRole.Role);
                await member.GrantRoleAsync(chosenRole);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Successfully changed role from {_guildSetup.RedRole.Role} to {chosenRole.Name}!").AsEphemeral());
            }
            else if (member.Roles.Contains(_guildSetup.YellowRole.Role) && chosenRole == _guildSetup.RedRole.Role)
            {
                await member.RevokeRoleAsync(_guildSetup.YellowRole.Role);
                await member.GrantRoleAsync(chosenRole);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Successfully changed role from {_guildSetup.YellowRole.Role} to {chosenRole.Name}!").AsEphemeral());
            }
        }

        public async Task DisposeGame(InteractionContext ctx)
        {
            await _guildSetup.Dispose();
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Successfully disposed game!").AsEphemeral());
        }

        public async Task JoinGame(InteractionContext ctx)
        {
            await GameInstance.Game.InitializeGame(ctx.Guild);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
                new DiscordInteractionResponseBuilder().WithContent("Select your role: ").AddComponents(
                    new DiscordComponent[]
                    {
                        _guildSetup.RedRole.buttonComponent,
                        _guildSetup.YellowRole.buttonComponent,
                    }).AsEphemeral());
        }

        public async Task ChangeTeam(InteractionContext ctx, DiscordMember member, long teamCode)
        {
            await ChangeTeamResponse(ctx.Interaction, member, teamCode);
        }

        public static async Task ButtonHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e)
        {
            DiscordMember m = await e.Guild.GetMemberAsync(e.User.Id);
            if (e.Id == GameInstance.Game._guildSetup.RedRole.Channel.Id.ToString())
                await GameInstance.Game.AssignTeam(e.Interaction, m, 0);
            else if (e.Id == GameInstance.Game._guildSetup.YellowRole.Channel.Id.ToString())
                await GameInstance.Game.AssignTeam(e.Interaction, m, 1);
        }
    }
}
