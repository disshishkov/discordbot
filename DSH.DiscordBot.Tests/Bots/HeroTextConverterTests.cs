using System;
using System.Collections.Generic;
using DSH.DiscordBot.Bots.Converters;
using DSH.DiscordBot.Contract.Dto;
using NUnit.Framework;

namespace DSH.DiscordBot.Tests.Bots
{
    [TestFixture]
    public sealed class HeroTextConverterTests
    {
        private IHeroTextConverter _converter;

        [SetUp]
        public void Init()
        {
            _converter = new HeroTextConverter();
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

            Assert.IsTrue(value.Contains("**TestSource**"));
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
            Assert.AreEqual("No one hero was added", value);
        }

        [Test]
        public void Convert_Heroes_Returns_Specific_Message_If_Hero_Is_Empty()
        {
            var value = _converter.Convert(new List<Hero>());
            Assert.AreEqual("No one hero was added", value);
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

            Assert.IsTrue(value.Contains("`TestHero`"));
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

            Assert.IsTrue(value.Contains("Commands: testalias, testhero"));
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

            Assert.IsTrue(value.Contains("Builds: 1"));
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

            Assert.IsTrue(value.Contains("Builds: 0"));
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

            Assert.IsTrue(value.Contains("Builds: 0"));
        }
    }
}