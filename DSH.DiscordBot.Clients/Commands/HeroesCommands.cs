using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSH.DiscordBot.Bots;
using DSH.DiscordBot.Bots.Converters;

namespace DSH.DiscordBot.Clients.Commands
{
    public sealed class HeroesCommands
    {
        // Discord has a 2000 lenght limit for the one message.
        private const int MaxMessageLenght = 1999;
        
        private readonly Lazy<IHotsHeroesBot> _hotsHeroesBot;
        private readonly Lazy<IHeroTextConverter> _heroesConverter;
        
        public HeroesCommands(Lazy<IHotsHeroesBot> hotsHeroesBot, Lazy<IHeroTextConverter> heroesConverter)
        {
            _hotsHeroesBot = hotsHeroesBot ?? throw new ArgumentNullException(nameof(hotsHeroesBot));
            _heroesConverter = heroesConverter ?? throw new ArgumentNullException(nameof(heroesConverter));
        }
        
        [Command("list")]
        [Description("Provides the list of available builds")]
        [Aliases("l", "tierlist")]
        public async Task List(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var answer = _heroesConverter.Value.Convert(_hotsHeroesBot.Value.GetHeroes());

            foreach (var msg in ChunksUpto(answer, MaxMessageLenght))
            {
                await ctx.RespondAsync(msg);
            }
        }
        
        [Command("build")]
        [Description("Gets build for a specific hero")]
        [Aliases("b")]
        public async Task Build(CommandContext ctx, string alias)
        {
            await ctx.TriggerTypingAsync();

            await ctx.RespondAsync(_heroesConverter.Value.Convert(
                _hotsHeroesBot.Value.GetHeroByAlias(alias)));
        }
        
        private static IEnumerable<string> ChunksUpto(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length-i));
        }
    }
}