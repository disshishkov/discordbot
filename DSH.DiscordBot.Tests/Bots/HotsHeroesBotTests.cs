using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DSH.DiscordBot.Bots;
using DSH.DiscordBot.Contract.Dto;
using DSH.DiscordBot.Infrastructure.Logging;
using DSH.DiscordBot.Infrastructure.Serialization;
using DSH.DiscordBot.Storage;
using Moq;
using NUnit.Framework;

namespace DSH.DiscordBot.Tests.Bots
{
    [TestFixture]
    public sealed class HotsHeroesBotTests
    {
        private IHotsHeroesBot _bot;

        private Mock<ILog> _logMock;
        private Mock<ISerializer> _serializerMock;
        private Mock<IStorage> _storageMock;

        [SetUp]
        public void Init()
        {
            _logMock = new Mock<ILog>(MockBehavior.Loose);

            _serializerMock = new Mock<ISerializer>(MockBehavior.Loose);
            _serializerMock.Setup(_ => _.Serialize(It.IsAny<object>()))
                .Returns("TestSerializer");

            var hero = new Hero()
            {
                Name = "Test",
                Id = "TEST",
                Aliases = new[] {"t"},
                Builds = new[]
                {
                    new Build()
                    {
                        Title = "TestBuild",
                        Source = "TestSource",
                        Url = new Uri("http://test.ru")
                    }
                }
            };

            _storageMock = new Mock<IStorage>(MockBehavior.Loose);
            _storageMock.Setup(_ => _.Fetch(It.IsAny<Expression<Func<Hero, bool>>>()))
                .Returns(() => new List<Hero>() { hero });
            _storageMock.Setup(_ => _.All<Hero>())
                .Returns(() => new List<Hero>() { hero });

            _bot = CreateBot();
        }

        [Test]
        public void Throws_If_ILog_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _bot = CreateBot(isILogNull: true);
            });
        }

        [Test]
        public void Throws_If_ISerializer_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _bot = CreateBot(isISerializerNull: true);
            });
        }

        [Test]
        public void Throws_If_IStorage_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _bot = CreateBot(isIStorageNull: true);
            });
        }

        [Test]
        public void GetHero_Throws_If_Name_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _bot.GetHero(null);
            });
        }

        [Test]
        public void GetHero_Throws_If_Name_Is_Empty()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _bot.GetHero(string.Empty);
            });
        }

        [Test]
        public void GetHero_Throws_If_Name_Is_WhiteSpace()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _bot.GetHero("    ");
            });
        }

        [Test]
        public void GetHero_Can_Returns_Hero()
        {
            var hero = _bot.GetHero("Test");
            Assert.IsNotNull(hero);
            Assert.AreEqual("Test", hero.Name);
        }
        
        [Test]
        public void GetHeroByAlias_Throws_If_Alias_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _bot.GetHeroByAlias(null);
            });
        }

        [Test]
        public void GetHeroByAlias_Throws_If_Alias_Is_Empty()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _bot.GetHeroByAlias(string.Empty);
            });
        }

        [Test]
        public void GetHeroByAlias_Throws_If_Alias_Is_WhiteSpace()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _bot.GetHeroByAlias("    ");
            });
        }
        
        [Test]
        public void GetHeroByAlias_Can_Returns_Hero_By_Name()
        {
            var hero = _bot.GetHeroByAlias("Test");
            Assert.IsNotNull(hero);
            Assert.AreEqual("Test", hero.Name);
        }
        
        [Test]
        public void GetHeroByAlias_Can_Returns_Hero_By_Alias()
        {
            var hero = _bot.GetHeroByAlias("t");
            Assert.IsNotNull(hero);
            Assert.AreEqual("Test", hero.Name);
        }

        [Test]
        public void GetHeroes_Can_Returns_Heroes()
        {
            var heroes = _bot.GetHeroes();
            Assert.IsNotNull(heroes);
            Assert.IsNotEmpty(heroes);
        }
        
        [Test]
        public void DeleteHero_Throws_If_Name_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _bot.DeleteHero(null);
            });
        }

        [Test]
        public void DeleteHero_Throws_If_Name_Is_Empty()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _bot.DeleteHero(string.Empty);
            });
        }

        [Test]
        public void DeleteHero_Throws_If_Name_Is_WhiteSpace()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _bot.DeleteHero("    ");
            });
        }

        [Test]
        public void DeleteHero_Can_Delete()
        {
            _bot.DeleteHero("Test");
        }

        [Test]
        public void ParseBuild_Return_Default_If_String_Is_Null()
        {
            var build = _bot.ParseBuild(null);
            Assert.IsNotNull(build);
            Assert.AreEqual("Default", build.Title);
        }

        [Test]
        public void ParseBuild_Return_Default_If_String_Is_Empty()
        {
            var build = _bot.ParseBuild(string.Empty);
            Assert.IsNotNull(build);
            Assert.AreEqual("Default", build.Title);
            Assert.IsNull(build.Url);
            Assert.IsNull(build.Source);
        }

        [Test]
        public void ParseBuild_Can_Parse_Url()
        {
            const string url = "http://test.ru/";

            var build = _bot.ParseBuild(url);
            Assert.IsNotNull(build);
            Assert.AreEqual("Default", build.Title);
            Assert.AreEqual(url, build.Url.AbsoluteUri);
        }

        [Test]
        public void ParseBuild_Throws_If_Incorrect_Url()
        {
            Assert.Throws<UriFormatException>(() =>
            {
                _bot.ParseBuild("test");
            });
        }

        [Test]
        public void ParseBuild_Can_Parse_Title()
        {
            var build = _bot.ParseBuild("Test|http://test.ru/");
            Assert.IsNotNull(build);
            Assert.AreEqual("Test", build.Title);
            Assert.AreEqual("http://test.ru/", build.Url.AbsoluteUri);
        }

        [Test]
        public void ParseBuild_Can_Parse_Source()
        {
            var build = _bot.ParseBuild("Test|http://test.ru/|Source1");
            Assert.IsNotNull(build);
            Assert.AreEqual("Test", build.Title);
            Assert.AreEqual("http://test.ru/", build.Url.AbsoluteUri);
            Assert.AreEqual("Source1", build.Source);
        }

        [Test]
        public void SaveAlias_Throws_If_HeroName_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _bot.SaveAlias(null, "alias");
            });
        }

        [Test]
        public void SaveAlias_Throws_If_HeroName_Is_Empty()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _bot.SaveAlias(string.Empty, "alias");
            });
        }

        [Test]
        public void SaveAlias_Throws_If_Alias_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _bot.SaveAlias("name", null);
            });
        }

        [Test]
        public void SaveAlias_Throws_If_Alias_Is_Empty()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _bot.SaveAlias("name", string.Empty);
            });
        }

        [Test]
        public void SaveAlias_Can_Save_Alias()
        {
            _bot.SaveAlias("name", "alias");
        }

        [Test]
        public void SaveHeroes_Throws_If_Heroes_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _bot.SaveHeroes(null);
            });
        }

        [Test]
        public void SaveHeroes_Can_Save_Heroes()
        {
            _bot.SaveHeroes(new []{new Hero()});
        }

        [Test]
        public void SaveBuild_Throws_If_HeroName_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _bot.SaveBuild(null, new Build()
                {
                    Url = new Uri("http://test.ru/"),
                    Title = "Test"
                });
            });
        }

        [Test]
        public void SaveBuild_Throws_If_HeroName_Is_Empty()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _bot.SaveBuild(string.Empty, new Build()
                {
                    Url = new Uri("http://test.ru/"),
                    Title = "Test"
                });
            });
        }

        [Test]
        public void SaveBuild_Throws_If_Build_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _bot.SaveBuild("name", null);
            });
        }

        [Test]
        public void SaveBuild_Throws_If_Build_Url_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _bot.SaveBuild("name", new Build()
                {
                    Url = null
                });
            });
        }

        [Test]
        public void SaveBuild_Throws_If_Build_Title_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _bot.SaveBuild("name", new Build()
                {
                    Url = new Uri("http://test.ru/"),
                    Title = null
                });
            });
        }

        [Test]
        public void SaveBuild_Throws_If_Build_Title_Is_Empty()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _bot.SaveBuild("name", new Build()
                {
                    Url = new Uri("http://test.ru/"),
                    Title = string.Empty
                });
            });
        }

        [Test]
        public void SaveBuild_Can_Save_Build()
        {
            var heroName = _bot.SaveBuild("Test", new Build()
            {
                Url = new Uri("http://test.ru/"),
                Title = "Test"
            });
            Assert.AreEqual("Test", heroName);
        }

        private IHotsHeroesBot CreateBot(
            bool isILogNull = false,
            bool isISerializerNull = false,
            bool isIStorageNull = false)
        {
            return new HotsHeroesBot(
                isILogNull ? null : new Lazy<ILog>(() => _logMock.Object),
                isISerializerNull ? null : new Lazy<ISerializer>(() => _serializerMock.Object),
                isIStorageNull ? null : new Lazy<IStorage>(() => _storageMock.Object));
        }
    }
}