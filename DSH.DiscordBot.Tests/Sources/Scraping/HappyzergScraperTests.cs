using System;
using DSH.DiscordBot.Infrastructure.Logging;
using DSH.DiscordBot.Sources.Scraping;
using HtmlAgilityPack;
using Moq;
using NUnit.Framework;

namespace DSH.DiscordBot.Tests.Sources.Scraping
{
    [TestFixture]
    public sealed class HappyzergScraperTests
    {
        private IScraper _scraper;

        private Mock<ILog> _logMock;

        [SetUp]
        public void Init()
        {
            _logMock = new Mock<ILog>(MockBehavior.Loose);

            _scraper = CreateScraper();
        }
        
        [Test]
        public void Throws_If_ILog_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _scraper = CreateScraper(isILogNull: true);
            });
        }
        
        [Test]
        public void Returns_Empty_List_If_Table_Is_Missing()
        {
            var heroes = _scraper.ParseHeroes(HtmlNode.CreateNode("<html><body></body></html>"), String.Empty);

            Assert.IsNotNull(heroes);
            Assert.IsEmpty(heroes);
        }

        [Test]
        public void Returns_Empty_List_If_Name_Can_not_be_Detect()
        {
            var heroes = _scraper.ParseHeroes(HtmlNode.CreateNode("<html><body><table><tbody>"
                                                                  + "<tr><th>Hero</th><th>Build</th></tr>"
                                                                  + "<tr><td>TestHero</td><td>TestBuild</td></tr>"
                                                                  + "</tbody></table></body></html>"), String.Empty);

            Assert.IsNotNull(heroes);
            Assert.IsEmpty(heroes);
        }

        [Test]
        public void Returns_Empty_List_If_Builds_Can_not_be_Detect()
        {
            var heroes = _scraper.ParseHeroes(HtmlNode.CreateNode("<html><body><table><tbody>"
                                                                  + "<tr><th>Hero</th><th>Build</th></tr>"
                                                                  + "<tr><td><p></p><p>TestHero</p></td><td>TestBuild</td></tr>"
                                                                  +"</tbody></table></body></html>"), String.Empty);

            Assert.IsNotNull(heroes);
            Assert.IsEmpty(heroes);
        }

        [Test]
        public void Returns_Empty_List_If_Builds_Title_Can_not_be_Detect()
        {
            var heroes = _scraper.ParseHeroes(HtmlNode.CreateNode("<html><body><table><tbody>"
                                                                  + "<tr><th>Hero</th><th>Build</th></tr>"
                                                                  + "<tr><td><p></p><p>TestHero</p></td>"
                                                                  + "<td><ul><li><span><a></a></span></li><ul></td></tr>"
                                                                  +"</tbody></table></body></html>"), String.Empty);

            Assert.IsNotNull(heroes);
            Assert.IsEmpty(heroes);
        }

        [Test]
        public void Returns_Empty_List_If_Builds_Url_Can_not_be_Detect()
        {
            var heroes = _scraper.ParseHeroes(HtmlNode.CreateNode("<html><body><table><tbody>"
                                                                  + "<tr><th>Hero</th><th>Build</th></tr>"
                                                                  + "<tr><td><p></p><p>TestHero</p></td>"
                                                                  + "<td><ul><li><span><a href=''>TestBuild</a></span></li><ul></td></tr>"
                                                                  +"</tbody></table></body></html>"), String.Empty);

            Assert.IsNotNull(heroes);
            Assert.IsEmpty(heroes);
        }

        [Test]
        public void Returns_Empty_List_If_Builds_Url_Is_Not_Valid()
        {
            var heroes = _scraper.ParseHeroes(HtmlNode.CreateNode("<html><body><table><tbody>"
                                                                  + "<tr><th>Hero</th><th>Build</th></tr>"
                                                                  + "<tr><td><p></p><p>TestHero</p></td>"
                                                                  + "<td><ul><li><span><a href='test'>TestBuild</a></span></li><ul></td></tr>"
                                                                  +"</tbody></table></body></html>"), String.Empty);

            Assert.IsNotNull(heroes);
            Assert.IsEmpty(heroes);
        }
        
        private IScraper CreateScraper(
            bool isILogNull = false)
        {
            return new HappyzergScraper(
                isILogNull ? null : new Lazy<ILog>(() => _logMock.Object));
        }
    }
}