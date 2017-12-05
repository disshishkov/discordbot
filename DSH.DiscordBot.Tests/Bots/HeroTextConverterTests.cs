using System;
using System.Collections.Generic;
using System.Linq;
using DSH.DiscordBot.Bots.Converters;
using DSH.DiscordBot.Contract.Dto;
using DSH.DiscordBot.Infrastructure.Configuration;
using Moq;
using NUnit.Framework;

namespace DSH.DiscordBot.Tests.Bots
{
    [TestFixture]
    public sealed class HeroTextConverterTests
    {
        private IHeroTextConverter _converter;

        private Mock<IConfig> _configMock;

        [SetUp]
        public void Init()
        {
            _configMock = new Mock<IConfig>(MockBehavior.Loose);
            _configMock.SetupGet(_ => _.HeroesCountInList).Returns(1);
            
            _converter = CreateConvertor();
        }
        
        [Test]
        public void Throws_If_IConfig_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _converter = CreateConvertor(isIIConfigNull: true);
            });
        }

        [Test]
        public void Convert_Hero_Returns_Specific_Message_If_Hero_Is_Null()
        {
            var value = _converter.Convert((Hero)null);
            Assert.AreEqual("Hero is not exist", value);
        }

        [Test]
        public void Convert_Hero_Returns_Specific_Message_If_Build_Is_Null()
        {
            var value = _converter.Convert(new Hero()
            {
                Name = "TestHero",
                Builds = null
            });
            Assert.AreEqual($"`TestHero`{Environment.NewLine}No one build was added{Environment.NewLine}", value);
        }

        [Test]
        public void Convert_Hero_Returns_Specific_Message_If_Build_Is_Empty()
        {
            var value = _converter.Convert(new Hero()
            {
                Name = "TestHero",
                Builds = new List<Build>()
            });
            Assert.AreEqual($"`TestHero`{Environment.NewLine}No one build was added{Environment.NewLine}", value);
        }

        [Test]
        public void Convert_Hero_Contains_Hero_Name()
        {
            var value = _converter.Convert(new Hero()
            {
                Name = "TestHero"
            });

            Assert.IsTrue(value.Contains("`TestHero`"));
        }
        
        [Test]
        public void Convert_Hero_Contains_Source()
        {
            var value = _converter.Convert(new Hero()
            {
                Name = "TestHero",
                Builds = new [] {
                    new Build()
                    {
                        Title = "TestBuild",
                        Url = new Uri("http://test.ru/"),
                        Source = "TestSource"
                    }
                }
            });

            Assert.IsTrue(value.Contains("**testsource**"));
        }

        [Test]
        public void Convert_Hero_Contains_Build()
        {
            var value = _converter.Convert(new Hero()
            {
                Name = "TestHero",
                Builds = new [] {
                    new Build()
                    {
                        Title = "TestBuild",
                        Url = new Uri("http://test.ru/")
                    }
                }
            });
            Assert.IsTrue(value.Contains("TestBuild - http://test.ru/"));
        }

        [Test]
        public void Convert_Heroes_Returns_Specific_Message_If_Hero_Is_Null()
        {
            var value = _converter.Convert((IEnumerable<Hero>)null);
            Assert.AreEqual("No one hero was added", value?.FirstOrDefault());
        }

        [Test]
        public void Convert_Heroes_Returns_Specific_Message_If_Hero_Is_Empty()
        {
            var value = _converter.Convert(new List<Hero>());
            Assert.AreEqual("No one hero was added", value?.FirstOrDefault());
        }

        [Test]
        public void Convert_Heroes_Contains_Hero_Name()
        {
            var value = _converter.Convert(new List<Hero>()
            {
                new Hero()
                {
                    Name = "TestHero"
                }
            });

            Assert.IsTrue(value?.FirstOrDefault()?.Contains("`TestHero`"));
        }

        [Test]
        public void Convert_Heroes_Contains_Commands()
        {
            var value = _converter.Convert(new List<Hero>()
            {
                new Hero()
                {
                    Name = "TestHero",
                    Aliases = new []{"TestAlias"}
                }
            });

            Assert.IsTrue(value?.FirstOrDefault()?.Contains("Commands: testalias, testhero"));
        }

        [Test]
        public void Convert_Heroes_Contains_Builds_Count()
        {
            var value = _converter.Convert(new List<Hero>()
            {
                new Hero()
                {
                    Name = "TestHero",
                    Builds = new [] {new Build()
                    {
                        Title = "TestBuild"
                    }}
                }
            });

            Assert.IsTrue(value?.FirstOrDefault()?.Contains("Builds: 1"));
        }

        [Test]
        public void Convert_Heroes_Builds_Count_Is_0_When_Builds_Is_Null()
        {
            var value = _converter.Convert(new List<Hero>()
            {
                new Hero()
                {
                    Name = "TestHero",
                    Builds = null
                }
            });

            Assert.IsTrue(value?.FirstOrDefault()?.Contains("Builds: 0"));
        }

        [Test]
        public void Convert_Heroes_Builds_Count_Is_0_When_Builds_Is_Empty()
        {
            var value = _converter.Convert(new List<Hero>()
            {
                new Hero()
                {
                    Name = "TestHero",
                    Builds = new List<Build>()
                }
            });

            Assert.IsTrue(value?.FirstOrDefault()?.Contains("Builds: 0"));
        }
        
        [Test]
        public void Convert_Heroes_Can_Split()
        {
            var value = _converter.Convert(new List<Hero>()
            {
                new Hero()
                {
                    Name = "TestHero1",
                    Builds = new List<Build>()
                },
                new Hero()
                {
                    Name = "TestHero2",
                    Builds = new List<Build>()
                }
            });
            
            var enumerable = value as string[] ?? value.ToArray();
            
            Assert.AreEqual(2, enumerable.Length);
            Assert.IsTrue(enumerable.FirstOrDefault()?.Contains("`TestHero1`"));
            Assert.IsTrue(enumerable.LastOrDefault()?.Contains("`TestHero2`"));
        }
        
        private IHeroTextConverter CreateConvertor(
            bool isIIConfigNull = false)
        {
            return new HeroTextConverter(
                isIIConfigNull ? null : new Lazy<IConfig>(() => _configMock.Object));
        }
    }
}