using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
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

            var hero = _hotsHeroesBot.Value.GetHeroByAlias(alias);

            if (hero == null)
            {
                await ctx.RespondAsync("Hero is not exist");
            }
            else
            {
                var screens = new List<(string title, byte[] data)>();
                var embed = new DiscordEmbedBuilder
                {
                    Title = hero.Name,
                    ThumbnailUrl = hero.ImageUrl?.AbsoluteUri,
                    Color = DiscordColor.Gray
                };

                if (!(hero.Builds?.Any() ?? false))
                {
                    embed.Description = "No one build was added";
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var builds in hero.Builds.GroupBy(_ => _.Source))
                    {
                        sb.AppendLine("");
                        sb.AppendLine($"**{builds.Key?.ToLowerInvariant()}**");
                        foreach (var build in builds)
                        {
                            sb.AppendLine($"{build.Title} - {build.Url}");
                            if (build.Screen != null)
                            {
                                screens.Add((build.Title, build.Screen));
                            }
                        }
                    }
                    embed.Description = sb.ToString();
                }

                await ctx.RespondAsync(embed: embed);
                
                foreach (var screen in screens)
                {
                    await ctx.RespondAsync($"`{screen.title}`");
                    using (var ms = new MemoryStream(screen.data))
                    {
                        await ctx.RespondWithFileAsync(ms, $"{Guid.NewGuid()}.jpeg");
                    }
                }
            }
        }
    }
}