using Connect4_house.Commands.GameCommandsModule.Structures;
using Connect4_house.GameLogic;
using Connect4_house.GameLogic.Structures;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;

namespace Connect4_house.Commands.GameCommandsModule
{
    internal class Connect4DiscordGame
    {
        public Connect4GuildSetup GuildSetup { get; set; }
        private DiscordGuild _guild;
        private Connect4Game _game;
        public bool started {get; private set;} = false;
        private DiscordButtonComponent[] optionsBtns;
        private PlayerType turnFlag;
        private DiscordMember _creator;

        private async Task AssignTeam(DiscordInteraction ctx, DiscordMember member, long teamCode)
        {
            DiscordRole chosenRole = teamCode == 0 ? GuildSetup.RedTeam.Role : GuildSetup.YellowTeam.Role;
            if (!(member.Roles.Contains(GuildSetup.YellowTeam.Role) && member.Roles.Contains(GuildSetup.RedTeam.Role)))
            {
                await member.GrantRoleAsync(chosenRole);
                await ctx.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent($"You have joined the {chosenRole.Name}!").AsEphemeral());
            }
            else
                await ctx.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent($"You are already in the {chosenRole.Name}!").AsEphemeral());
        }

        private async Task ChangeTeamResponse(DiscordInteraction ctx, DiscordMember member, long teamCode)
        {
            DiscordRole chosenRole = teamCode == 0 ? GuildSetup.RedTeam.Role : GuildSetup.YellowTeam.Role;
            if (member.Roles.Contains(GuildSetup.RedTeam.Role) && chosenRole == GuildSetup.YellowTeam.Role)
            {
                await member.RevokeRoleAsync(GuildSetup.RedTeam.Role);
                await member.GrantRoleAsync(chosenRole);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Successfully changed role from {GuildSetup.RedTeam.Role} to {chosenRole.Name}!").AsEphemeral());
            }
            else if (member.Roles.Contains(GuildSetup.YellowTeam.Role) && chosenRole == GuildSetup.RedTeam.Role)
            {
                await member.RevokeRoleAsync(GuildSetup.YellowTeam.Role);
                await member.GrantRoleAsync(chosenRole);
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent($"Successfully changed role from {GuildSetup.YellowTeam.Role} to {chosenRole.Name}!").AsEphemeral());
            }
        }

        private string GetBoardUpdateMessage(PlayerType p)
        {
            string message = _game.GetDiscordBoard().ToString();
            if (turnFlag == p)
                return "It is now your turn.\r\n" + message;
            else
                return "Waiting for turn...\r\n" + message;
        }

        private async Task UpdateDraw()
        {
            //update message after a single message is created
            GuildSetup.RedTeam.BoardMessageContents.ClearComponents();
            GuildSetup.YellowTeam.BoardMessageContents.ClearComponents();
            GuildSetup.RedTeam.BoardMessageContents.Content = "The Game has ended with draw.\n" + _game.GetDiscordBoard().ToString();
            GuildSetup.YellowTeam.BoardMessageContents.Content = "The Game has ended with draw.\n" + _game.GetDiscordBoard().ToString();

            await GuildSetup.RedTeam.BoardMessage.ModifyAsync(GuildSetup.RedTeam.BoardMessageContents);
            await GuildSetup.YellowTeam.BoardMessage.ModifyAsync(GuildSetup.YellowTeam.BoardMessageContents);
        }

        private async Task UpdateWinner()
        {
            DiscordConnect4Team teamWinner, teamLoser;
            if (turnFlag == PlayerType.RED)
            {
                teamWinner = GuildSetup.RedTeam;
                teamLoser = GuildSetup.YellowTeam;
            }
            else
            {
                teamWinner = GuildSetup.YellowTeam;
                teamLoser = GuildSetup.RedTeam;
            }

            //update message after a single message is created
            teamWinner.BoardMessageContents.ClearComponents();
            teamLoser.BoardMessageContents.ClearComponents();
            teamWinner.BoardMessageContents.Content = "Your team won!\n" + _game.GetDiscordBoard().ToString();
            teamLoser.BoardMessageContents.Content = "Your team lost! Better luck next time <:coolmen:591614014698684417>\n" + _game.GetDiscordBoard().ToString();

            await GuildSetup.RedTeam.BoardMessage.ModifyAsync(GuildSetup.RedTeam.BoardMessageContents);
            await GuildSetup.YellowTeam.BoardMessage.ModifyAsync(GuildSetup.YellowTeam.BoardMessageContents);

        }

        private async Task UpdateBoard()
        {
            if (GuildSetup.RedTeam.BoardMessageContents == null && GuildSetup.YellowTeam.BoardMessageContents == null)
            {
                //create a message that will be used for later updates
                DiscordMessageBuilder msgBuilderRed = new DiscordMessageBuilder();
                msgBuilderRed.Content = GetBoardUpdateMessage(PlayerType.RED);
                msgBuilderRed.AddComponents(optionsBtns.Take(3));
                msgBuilderRed.AddComponents(optionsBtns.Skip(3).Take(4));
                GuildSetup.RedTeam.BoardMessage = await GuildSetup.RedTeam.Channel.SendMessageAsync(msgBuilderRed);
                GuildSetup.RedTeam.BoardMessageContents = msgBuilderRed;

                DiscordMessageBuilder msgBuilderYellow = new DiscordMessageBuilder();
                msgBuilderYellow.Content = GetBoardUpdateMessage(PlayerType.YELLOW);
                msgBuilderYellow.AddComponents(optionsBtns.Take(3));
                msgBuilderYellow.AddComponents(optionsBtns.Skip(3).Take(4));
                GuildSetup.YellowTeam.BoardMessage = await GuildSetup.YellowTeam.Channel.SendMessageAsync(msgBuilderYellow);
                GuildSetup.YellowTeam.BoardMessageContents = msgBuilderYellow;
            }
            else
            {
                //update message after a single message is created
                GuildSetup.RedTeam.BoardMessageContents.Content = GetBoardUpdateMessage(PlayerType.RED);
                GuildSetup.YellowTeam.BoardMessageContents.Content = GetBoardUpdateMessage(PlayerType.YELLOW);

                await GuildSetup.RedTeam.BoardMessage.ModifyAsync(GuildSetup.RedTeam.BoardMessageContents);
                await GuildSetup.YellowTeam.BoardMessage.ModifyAsync(GuildSetup.YellowTeam.BoardMessageContents);
            }

        }

        public async Task InitializeGame(InteractionContext i, DiscordMember creator, bool isNew = true)
        {
            if(isNew)
                await i.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Successfully Created Game!").AsEphemeral());
            started = false;
            _guild = i.Guild;
            _creator = creator;
            _game = new Connect4Game();
            optionsBtns = new DiscordButtonComponent[7];
            optionsBtns[0] = new DiscordButtonComponent(ButtonStyle.Secondary, "_1", "", emoji: new DiscordComponentEmoji("1️⃣"));
            optionsBtns[1] = new DiscordButtonComponent(ButtonStyle.Secondary, "_2", "", emoji: new DiscordComponentEmoji("2️⃣"));
            optionsBtns[2] = new DiscordButtonComponent(ButtonStyle.Secondary, "_3", "", emoji: new DiscordComponentEmoji("3️⃣"));
            optionsBtns[3] = new DiscordButtonComponent(ButtonStyle.Secondary, "_4", "", emoji: new DiscordComponentEmoji("4️⃣"));
            optionsBtns[4] = new DiscordButtonComponent(ButtonStyle.Secondary, "_5", "", emoji: new DiscordComponentEmoji("5️⃣"));
            optionsBtns[5] = new DiscordButtonComponent(ButtonStyle.Secondary, "_6", "", emoji: new DiscordComponentEmoji("6️⃣"));
            optionsBtns[6] = new DiscordButtonComponent(ButtonStyle.Secondary, "_7", "", emoji: new DiscordComponentEmoji("7️⃣"));
            if (isNew)
            {
                GuildSetup = new Connect4GuildSetup(_guild);
                await GuildSetup.InitializeTeam(creator);
            }

            turnFlag = PlayerType.RED;
        }

        public async Task DisposeGame(InteractionContext ctx)
        {
            await GuildSetup.Dispose();
        }

        public async Task JoinGame(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
                new DiscordInteractionResponseBuilder().WithContent("Select your role: ").AddComponents(
                    new DiscordComponent[]
                    {
                        GuildSetup.RedTeam.buttonComponent,
                        GuildSetup.YellowTeam.buttonComponent,
                    }).AsEphemeral());
        }

        public async Task StartGame(InteractionContext ctx, bool reset = false)
        {
            if(reset)
            {
                GuildSetup.RedTeam.BoardMessageContents.AddComponents(optionsBtns.Take(3));
                GuildSetup.RedTeam.BoardMessageContents.AddComponents(optionsBtns.Skip(3).Take(4));
                GuildSetup.YellowTeam.BoardMessageContents.AddComponents(optionsBtns.Take(3));
                GuildSetup.YellowTeam.BoardMessageContents.AddComponents(optionsBtns.Skip(3).Take(4));
            }
            started = true;
            await UpdateBoard();
            if(!reset)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("The game has started!").AsEphemeral());
            }
        }

        internal async Task PlayerMove(DiscordInteraction ctx, DiscordMember m, string ID)
        {
            //check player role (A player can't have both red and yellow team roles)
            if((m.Roles.Contains(GuildSetup.RedTeam.Role) && turnFlag != PlayerType.RED)
                || m.Roles.Contains(GuildSetup.YellowTeam.Role) && turnFlag != PlayerType.YELLOW)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("It is not your turn yet!").AsEphemeral());
                return;
            }

            //parse the ID of selected button
            int intId = int.Parse(ID.Substring(1, ID.Length - 1));
            if(!_game.CanDropAtSlot(intId))
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("You can't drop coins anymore in slot " + intId + "!").AsEphemeral());
                return;
            }

            _game.DropCoin(intId, turnFlag);

            if (_game.FindConsecutive4(turnFlag))
            {
                await UpdateWinner();
                await ctx.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                return;
            }

            if (_game.IsDraw())
            {
                await UpdateDraw();
                await ctx.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                return;
            }

            //change turn
            if (turnFlag == PlayerType.RED)
                turnFlag = PlayerType.YELLOW;
            else
                turnFlag = PlayerType.RED;

            await UpdateBoard();
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
        }

        internal async Task ChangeTeam(InteractionContext ctx, DiscordMember member, long teamCode)
        {
            await ChangeTeamResponse(ctx.Interaction, member, teamCode);
        }

        public static async Task ButtonHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e)
        {
            DiscordMember m = await e.Guild.GetMemberAsync(e.User.Id);

            if(e.Id.Contains("|"))
            {
                string[] splitData = e.Id.Split('|');
                ulong userId = ulong.Parse(splitData[1]);
                string actualBtnId = splitData[0];
                DiscordMember gameCreator = await e.Guild.GetMemberAsync(userId);
                if (!GameManager.GameInstances.ContainsKey(gameCreator))
                    throw new NotSupportedException(); //unexpected error: game creator must be always in the game instance
                                                       //roles handler

                if (actualBtnId == GameManager.GameInstances[gameCreator].GuildSetup.RedTeam.Channel.Id.ToString())
                    await GameManager.GameInstances[gameCreator].AssignTeam(e.Interaction, m, 0);
                else if (actualBtnId == GameManager.GameInstances[gameCreator].GuildSetup.YellowTeam.Channel.Id.ToString())
                    await GameManager.GameInstances[gameCreator].AssignTeam(e.Interaction, m, 1);
            }

            //player move handler
            if(e.Id.StartsWith("_"))
            {
                if (m.VoiceState == null || m.VoiceState.Channel == null || !GameManager.ChannelLookup.ContainsKey(m.VoiceState.Channel.Id))
                {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("You need to be in a game voice chat!").AsEphemeral());
                    return;
                }

                DiscordMember gameCreator = GameManager.ChannelLookup[m.VoiceState.Channel.Id];
                if (!GameManager.GameInstances.ContainsKey(gameCreator))
                    throw new NotSupportedException(); //unexpected error: game creator must be always in the game instance
                                                       //roles handler

                await GameManager.GameInstances[gameCreator].PlayerMove(e.Interaction, m, e.Id);
            }
        }
    }
}
