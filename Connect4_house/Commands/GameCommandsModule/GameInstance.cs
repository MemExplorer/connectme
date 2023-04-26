using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Connect4_house.Commands.GameCommandsModule
{
    internal class GameInstance
    {
        internal static Dictionary<DiscordMember, Connect4DiscordGame> GameInstances = new Dictionary<DiscordMember, Connect4DiscordGame>();
        internal static Dictionary<ulong, DiscordMember> ChannelLookup = new Dictionary<ulong, DiscordMember>();
        public static async Task<bool> TryCreateGameInstance(InteractionContext ctx)
        {
            //get member
            DiscordMember member = ctx.Member;
            if (GameInstances.ContainsKey(member))
                return false;
              
            //game instances 
            GameInstances[member] = new Connect4DiscordGame();
            await GameInstances[member].InitializeGame(ctx, member);

            //restrict voice chat
            if (!ChannelLookup.ContainsKey(GameInstances[member].GuildSetup.RedTeam.Channel.Id) && !ChannelLookup.ContainsKey(GameInstances[member].GuildSetup.YellowTeam.Channel.Id))
            {
                ChannelLookup[GameInstances[member].GuildSetup.RedTeam.Channel.Id] = member;
                ChannelLookup[GameInstances[member].GuildSetup.YellowTeam.Channel.Id] = member;
            }
            else
                throw new NotSupportedException(); //unexpected error cuz channel ids are supposed to be unique
            return true;
        }


        public static async Task<bool> TryResetGameInstance(InteractionContext ctx)
        {
            DiscordMember member = ctx.Member;
            if (!GameInstances.ContainsKey(member))
                return false;
            try
            {
                var guildSetup = GameInstances[member].GuildSetup;
                GameInstances[member] = new Connect4DiscordGame();
                GameInstances[member].GuildSetup = guildSetup;
                await GameInstances[member].InitializeGame(ctx, member, false);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message+ Environment.NewLine + ex.StackTrace);
                return false;
            }

            return true;
        }

        public static async Task<bool> TryDeleteGameInstance(InteractionContext ctx)
        {
            DiscordMember gameOwner = ctx.Member;
            if (!GameInstances.ContainsKey(gameOwner))
                return false;

            var gameInstance = GameInstances[gameOwner];
            if(!ChannelLookup.ContainsKey(gameInstance.GuildSetup.RedTeam.Channel.Id) || !ChannelLookup.ContainsKey(gameInstance.GuildSetup.YellowTeam.Channel.Id))
                return false;


            await gameInstance.DisposeGame(ctx);
            if (!ChannelLookup.Remove(gameInstance.GuildSetup.RedTeam.Channel.Id))
                return false;
            if (!ChannelLookup.Remove(gameInstance.GuildSetup.YellowTeam.Channel.Id))
                return false;
            if(!GameInstances.Remove(gameOwner)) 
                return false;

            return true;
        }
    }
}
