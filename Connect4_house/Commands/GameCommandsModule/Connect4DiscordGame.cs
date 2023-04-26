using Connect4_house.GameLogic;
using Connect4_house.GameLogic.Structures;
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
        private bool gameStarted = false;
        private DiscordButtonComponent[] optionsBtns;
        private PlayerType turnFlag;
        public async Task InitializeGame(DiscordGuild guild)
        {
            inited = true;
            gameStarted = false;
            _guild = guild;
            _guildSetup = new Connect4GuildSetup(_guild);
            _game = new Connect4Game();
            optionsBtns = new DiscordButtonComponent[7];
            for (int i = 0; i < optionsBtns.Length; i++)
                optionsBtns[i] = new DiscordButtonComponent(ButtonStyle.Secondary, "_" + (i + 1).ToString(), (i + 1).ToString());
            await _guildSetup.InitializeTeam();

            turnFlag = PlayerType.RED;
        }

        public async Task AssignTeam(DiscordInteraction ctx, DiscordMember member, long teamCode)
        {
            DiscordRole chosenRole = teamCode == 0 ? _guildSetup.RedTeam.Role : _guildSetup.YellowTeam.Role;
           if(!(member.Roles.Contains(_guildSetup.YellowTeam.Role) && member.Roles.Contains(_guildSetup.RedTeam.Role)))
            {
                await member.GrantRoleAsync(chosenRole);
                await ctx.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent($"You have joined the {chosenRole.Name}!").AsEphemeral());
            }
            else
                await ctx.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent($"You are already in the {chosenRole.Name}!").AsEphemeral());
        }

        public async Task ChangeTeamResponse(DiscordInteraction ctx, DiscordMember member, long teamCode)
        {
            DiscordRole chosenRole = teamCode == 0 ? _guildSetup.RedTeam.Role : _guildSetup.YellowTeam.Role;
            if (member.Roles.Contains(_guildSetup.RedTeam.Role) && chosenRole == _guildSetup.YellowTeam.Role)
            {
                await member.RevokeRoleAsync(_guildSetup.RedTeam.Role);
                await member.GrantRoleAsync(chosenRole);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Successfully changed role from {_guildSetup.RedTeam.Role} to {chosenRole.Name}!").AsEphemeral());
            }
            else if (member.Roles.Contains(_guildSetup.YellowTeam.Role) && chosenRole == _guildSetup.RedTeam.Role)
            {
                await member.RevokeRoleAsync(_guildSetup.YellowTeam.Role);
                await member.GrantRoleAsync(chosenRole);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Successfully changed role from {_guildSetup.YellowTeam.Role} to {chosenRole.Name}!").AsEphemeral());
            }
        }

        public async Task DisposeGame(InteractionContext ctx)
        {
            await _guildSetup.Dispose();
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Successfully disposed game!").AsEphemeral());
        }

        public async Task JoinGame(InteractionContext ctx)
        {
            if(!inited)
                await GameInstance.Game.InitializeGame(ctx.Guild);

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
                new DiscordInteractionResponseBuilder().WithContent("Select your role: ").AddComponents(
                    new DiscordComponent[]
                    {
                        _guildSetup.RedTeam.buttonComponent,
                        _guildSetup.YellowTeam.buttonComponent,
                    }).AsEphemeral());
        }

        public async Task StartGame(InteractionContext ctx)
        {
            await UpdateBoard();
            gameStarted = true;
        }

        private string GetBoardUpdateMessage(PlayerType p)
        {
            string message = _game.GetDiscordBoard().ToString();
            if (turnFlag == p)
                return "It is now your turn.\r\n" + message;
            else
                return "Waiting for turn...\r\n" + message;
        }

        public async Task UpdateBoard()
        {
            if(_guildSetup.RedTeam.BoardMessageContents == null && _guildSetup.YellowTeam.BoardMessageContents == null)
            {
                DiscordMessageBuilder msgBuilderRed = new DiscordMessageBuilder();
                msgBuilderRed.Content = GetBoardUpdateMessage(PlayerType.RED);
                msgBuilderRed.AddComponents(optionsBtns.Take(3));
                msgBuilderRed.AddComponents(optionsBtns.Skip(3).Take(4));
                _guildSetup.RedTeam.BoardMessage = await _guildSetup.RedTeam.Channel.SendMessageAsync(msgBuilderRed);
                _guildSetup.RedTeam.BoardMessageContents = msgBuilderRed;

                DiscordMessageBuilder msgBuilderYellow = new DiscordMessageBuilder();
                msgBuilderYellow.Content = GetBoardUpdateMessage(PlayerType.YELLOW);
                msgBuilderYellow.AddComponents(optionsBtns.Take(3));
                msgBuilderYellow.AddComponents(optionsBtns.Skip(3).Take(4));
                _guildSetup.YellowTeam.BoardMessage = await _guildSetup.YellowTeam.Channel.SendMessageAsync(msgBuilderYellow);
                _guildSetup.YellowTeam.BoardMessageContents = msgBuilderYellow;
            }
            else
            {
                _guildSetup.RedTeam.BoardMessageContents.Content = GetBoardUpdateMessage(PlayerType.RED);
                _guildSetup.YellowTeam.BoardMessageContents.Content  = GetBoardUpdateMessage(PlayerType.YELLOW);

                await _guildSetup.RedTeam.BoardMessage.ModifyAsync(_guildSetup.RedTeam.BoardMessageContents);
                await _guildSetup.YellowTeam.BoardMessage.ModifyAsync(_guildSetup.YellowTeam.BoardMessageContents);
            }



        }

        public async Task PlayerMove(DiscordInteraction ctx, DiscordMember m, string ID)
        {
            if((m.Roles.Contains(_guildSetup.RedTeam.Role) && turnFlag != PlayerType.RED)
                || m.Roles.Contains(_guildSetup.YellowTeam.Role) && turnFlag != PlayerType.YELLOW)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("It is not your turn yet!").AsEphemeral());
                return;
            }

            int intId = int.Parse(ID.Substring(1, ID.Length - 1));
            if(!_game.CanDropAtSlot(intId))
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("You can't drop coins anymore in slot " + intId + "!").AsEphemeral());
                return;
            }

            _game.DropCoin(intId, turnFlag);
            //change turn
            if (turnFlag == PlayerType.RED)
                turnFlag = PlayerType.YELLOW;
            else
                turnFlag = PlayerType.RED;

            await UpdateBoard();
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("Successfully dropped a coin!").AsEphemeral());
            await ctx.DeleteFollowupMessageAsync(ctx.Id);
            

        }

        public async Task ChangeTeam(InteractionContext ctx, DiscordMember member, long teamCode)
        {
            await ChangeTeamResponse(ctx.Interaction, member, teamCode);
        }

        public static async Task ButtonHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e)
        {
            DiscordMember m = await e.Guild.GetMemberAsync(e.User.Id);

            //roles handler
            if (e.Id == GameInstance.Game._guildSetup.RedTeam.Channel.Id.ToString())
                await GameInstance.Game.AssignTeam(e.Interaction, m, 0);
            else if (e.Id == GameInstance.Game._guildSetup.YellowTeam.Channel.Id.ToString())
                await GameInstance.Game.AssignTeam(e.Interaction, m, 1);

            if(e.Id.StartsWith("_"))
                await GameInstance.Game.PlayerMove(e.Interaction, m, e.Id);


        }
    }
}
