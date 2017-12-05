using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSH.DiscordBot.Bots;
using DSH.DiscordBot.Bots.Converters;

namespace DSH.DiscordBot.Clients.Commands
{
    public sealed class HeroesCommands
    {
        private readonly Lazy<IHotsHeroesBot> _hotsHeroesBot;
        private readonly Lazy<IHeroTextConverter> _heroesConverter;
        
        public HeroesCommands(Lazy<IHotsHeroesBot> hotsHeroesBot, Lazy<IHeroTextConverter> heroesConverter)
        {
            _hotsHeroesBot = hotsHeroesBot ?? throw new ArgumentNullException(nameof(hotsHeroesBot));
            _heroesConverter = heroesConverter ?? throw new ArgumentNullException(nameof(heroesConverter));
        }
        
        [Command("list")]
        [Description("Provides the list of available builds")]
        [Aliases("l", "tierlist", "л")]
        public async Task List(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var answer = _heroesConverter.Value.Convert(_hotsHeroesBot.Value.GetHeroes());

            foreach (var msg in answer)
            {
                await ctx.RespondAsync(msg);
            }
        }
        
        [Command("build")]
        [Description("Gets build for a specific hero")]
        [Aliases("b", "б")]
        public async Task Build(CommandContext ctx, string alias)
        {
            await ctx.TriggerTypingAsync();

            await ctx.RespondAsync(_heroesConverter.Value.Convert(
                _hotsHeroesBot.Value.GetHeroByAlias(alias)));
        }
    }
}