using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSH.DiscordBot.Bots;
using DSH.DiscordBot.Sources;

namespace DSH.DiscordBot.Clients.Commands
{
    [Hidden, RequireOwner]
    public sealed class AdminCommands
    {
        private readonly Lazy<IHotsHeroesBot> _hotsHeroesBot;
        private readonly Lazy<ISource> _source;
        
        public AdminCommands(Lazy<IHotsHeroesBot> hotsHeroesBot, Lazy<ISource> source)
        {
            _hotsHeroesBot = hotsHeroesBot ?? throw new ArgumentNullException(nameof(hotsHeroesBot));
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        [Command("ping")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            
            var emoji = DiscordEmoji.FromName(ctx.Client, ":ping_pong:");
            await ctx.RespondAsync($"{emoji} Pong! Ping: {ctx.Client.Ping}ms");
        }
        
        [Command("update")]
        public async Task Update(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            _hotsHeroesBot.Value.SaveHeroes(_source.Value.GetHeroes());

            await ctx.RespondAsync(DiscordEmoji.FromName(ctx.Client, ":ok:"));
        }
        
        [Command("delete")]
        public async Task Delete(CommandContext ctx, string heroName)
        {
            await ctx.TriggerTypingAsync();

            _hotsHeroesBot.Value.DeleteHero(heroName);

            await ctx.RespondAsync(DiscordEmoji.FromName(ctx.Client, ":ok:"));
        }
        
        [Command("drop")]
        public async Task Drop(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            _hotsHeroesBot.Value.DeleteAllHeroes();

            await ctx.RespondAsync(DiscordEmoji.FromName(ctx.Client, ":ok:"));
        }
        
        [Command("add_alias")]
        public async Task AddAlias(CommandContext ctx, string heroName, string alias)
        {
            await ctx.TriggerTypingAsync();

            _hotsHeroesBot.Value.SaveAlias(heroName, alias);
            
            await ctx.RespondAsync($"`{alias}` alias was succesfully added to hero `{heroName}`");
        }
        
        [Command("add_build")]
        public async Task AddBuild(CommandContext ctx, string heroName, string buildStr)
        {
            await ctx.TriggerTypingAsync();

            var build = _hotsHeroesBot.Value.ParseBuild(buildStr);
            _hotsHeroesBot.Value.SaveBuild(heroName, build);
            
            await ctx.RespondAsync($"`{build.Title}` build was succesfully added to hero `{heroName}`");
        }
    }
}