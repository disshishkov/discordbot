using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSH.DiscordBot.Bots;
using DSH.DiscordBot.Contract.Dto;
using DSH.DiscordBot.Infrastructure.Configuration;
using DSH.DiscordBot.Sources;

namespace DSH.DiscordBot.Clients.Commands
{
    [Hidden, RequireOwner]
    public sealed class AdminCommands
    {
        private readonly Lazy<IHotsHeroesBot> _hotsHeroesBot;
        private readonly Lazy<IConfig> _config;
        private readonly Lazy<IDictionary<SourceType, ISource>> _sourceFactory;
        
        public AdminCommands(
            Lazy<IHotsHeroesBot> hotsHeroesBot,
            Lazy<IConfig> config,
            Lazy<IDictionary<SourceType, ISource>> sourceFactory)
        {
            _hotsHeroesBot = hotsHeroesBot ?? throw new ArgumentNullException(nameof(hotsHeroesBot));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _sourceFactory = sourceFactory ?? throw new ArgumentNullException(nameof(sourceFactory));
        }

        [Command("ping")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            
            var emoji = DiscordEmoji.FromName(ctx.Client, ":ping_pong:");
            await ctx.RespondAsync($"{emoji} Pong! Ping: {ctx.Client.Ping}ms");
            
            /*
            var embed = new DiscordEmbedBuilder
            {
                Title = "Aba",
                ImageUrl = "http://www.heroesfire.com/images/wikibase/icon/heroes/abathur.png"
            };
            await ctx.RespondAsync(embed: embed);*/
        }
        
        [Command("update")]
        public async Task Update(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var sources = _config.Value.Sources;
            if (sources != null)
            {
                var heroes = new List<Hero>();

                foreach (var groupedSources in sources.GroupBy(_ => _.Type))
                {
                    heroes.AddRange(_sourceFactory.Value[groupedSources.Key].GetHeroes(groupedSources));
                }
                
                _hotsHeroesBot.Value.SaveHeroes(heroes);
            }

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