﻿using System;
using System.Linq;
using Autofac.Features.Indexed;
using DSH.DiscordBot.Contract.Dto;
using DSH.DiscordBot.Infrastructure.Logging;
using DSH.DiscordBot.Infrastructure.Serialization;
using DSH.DiscordBot.Infrastructure.Web;
using DSH.DiscordBot.Sources;
using DSH.DiscordBot.Sources.Scraping;
using HtmlAgilityPack;
using Moq;
using NUnit.Framework;

namespace DSH.DiscordBot.Tests.Sources
{
    [TestFixture]
    public sealed class ScrapingSourceTests
    {
        private ISource _source;

        private Mock<ILog> _logMock;
        private Mock<ISerializer> _serializerMock;
        private Mock<IClient> _clientMock;
        private Mock<IIndex<string, IScraper>> _scraperFactory;
        private Mock<IScraper> _scraperMock;

        [SetUp]
        public void Init()
        {
            _logMock = new Mock<ILog>(MockBehavior.Loose);

            _serializerMock = new Mock<ISerializer>(MockBehavior.Loose);
            _serializerMock.Setup(_ => _.Serialize(It.IsAny<object>()))
                .Returns("TestSerializer");
            
            _scraperMock = new Mock<IScraper>(MockBehavior.Loose);
            _scraperMock.Setup(_ => _.ParseHeroes(It.IsAny<HtmlNode>(), It.IsAny<string>()))
                .Returns(() => new[]
                {
                    new Hero()
                    {
                        Name = "TestHero",
                        Builds = new[]
                        {
                            new Build()
                            {
                                Title = "TestBuild",
                                Url = new Uri("http://test.ru")
                            }
                        }
                    }
                });
            
            _scraperFactory = new Mock<IIndex<string, IScraper>>();
            _scraperFactory.Setup(_ => _[It.IsAny<string>()]).Returns(_scraperMock.Object);

            _clientMock = new Mock<IClient>(MockBehavior.Loose);
            _clientMock.Setup(_ => _.GetString(It.IsAny<string>()))
                .ReturnsAsync("<html><body><table><tbody>"
                              + "<tr><th>Hero</th><th>Build</th></tr>"
                              + "<tr><td><p></p><p>TestHero</p></td>"
                              + "<td><ul><li><span><a href='http://test.ru'>TestBuild</a></span></li><ul></td></tr>"
                              +"</tbody></table></body></html>");

            _source = CreateSource();
        }

        [Test]
        public void Throws_If_ILog_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _source = CreateSource(isILogNull: true);
            });
        }

        [Test]
        public void Throws_If_ISerializer_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _source = CreateSource(isISerializerNull: true);
            });
        }

        [Test]
        public void Throws_If_IClient_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _source = CreateSource(isIClientNull: true);
            });
        }
        
        [Test]
        public void Throws_If_IScraper_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _source = CreateSource(isIScraperNull: true);
            });
        }

        [Test]
        public void Returns_Empty_List_If_Sources_Is_Null()
        {
            var heroes = _source.GetHeroes(null);

            Assert.IsNotNull(heroes);
            Assert.IsEmpty(heroes);
        }

        [Test]
        public void Returns_Empty_List_If_Sources_Is_Empty()
        {
            var heroes = _source.GetHeroes(new[] {(Source)null });

            Assert.IsNotNull(heroes);
            Assert.IsEmpty(heroes);
        }
        
        [Test]
        public void Returns_Empty_List_If_Source_Url_Is_Null()
        {
            var heroes = _source.GetHeroes(new[] {new Source() {Url = null} });

            Assert.IsNotNull(heroes);
            Assert.IsEmpty(heroes);
        }
        
        [Test]
        public void Returns_Empty_List_If_Source_Type_Is_Not_Scraping()
        {
            var heroes = _source.GetHeroes(new[] {
                new Source()
                {
                    Type = SourceType.Api, 
                    Url = new Uri("http://TestSource.com")
                } 
            });

            Assert.IsNotNull(heroes);
            Assert.IsEmpty(heroes);
        }

        [Test]
        public void Can_Detect_Builds()
        {
            var heroes = _source.GetHeroes(new[] {
                new Source()
                {
                    Type = SourceType.Scraping, 
                    Url = new Uri("http://TestSource.com")
                } 
            });

            Assert.IsNotNull(heroes);

            var collection = heroes as Hero[] ?? heroes.ToArray();
            Assert.IsNotEmpty(collection);

            var hero = collection.FirstOrDefault();

            Assert.IsNotNull(hero);
            Assert.AreEqual("TestHero", hero.Name);

            Assert.IsNotEmpty(hero.Builds);
            Assert.IsNotEmpty(hero.Builds);

            var build = hero.Builds.FirstOrDefault();

            Assert.IsNotNull(build);
            Assert.AreEqual("TestBuild", build.Title);
            Assert.AreEqual("http://test.ru/", build.Url.AbsoluteUri);
        }

        private ISource CreateSource(
            bool isILogNull = false,
            bool isISerializerNull = false,
            bool isIClientNull = false,
            bool isIScraperNull = false)
        {
            return new ScrapingSource(
                isILogNull ? null : new Lazy<ILog>(() => _logMock.Object),
                isISerializerNull ? null : new Lazy<ISerializer>(() => _serializerMock.Object),
                isIClientNull ? null : new Lazy<IClient>(() => _clientMock.Object),
                isIScraperNull ? null : _scraperFactory.Object);
        }
    }
}