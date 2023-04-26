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
        [SlashCommand("join", "Join a Connect4 Game.")]
        public async Task JoinGame(InteractionContext ctx)
        {
            await GameInstance.Game.JoinGame(ctx);
        }

        [SlashCommand("start", "Starts a Connect4 Game.")]
        public async Task StartGame(InteractionContext ctx)
        {
            await GameInstance.Game.StartGame(ctx);
        }

        [SlashCommand("delrole", "role deleter")]
        public async Task TestRoleDeleter(InteractionContext ctx)
        {
            await GameInstance.Game.DisposeGame(ctx);
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
