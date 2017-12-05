using System;
using System.Linq;
using DSH.DiscordBot.Contract.Dto;
using DSH.DiscordBot.Infrastructure.Logging;
using DSH.DiscordBot.Infrastructure.Serialization;
using DSH.DiscordBot.Infrastructure.Web;
using DSH.DiscordBot.Sources;
using DSH.DiscordBot.Sources.Api.Entities;
using Moq;
using NUnit.Framework;
using Build = DSH.DiscordBot.Sources.Api.Entities.Build;
using Hero = DSH.DiscordBot.Sources.Api.Entities.Hero;

namespace DSH.DiscordBot.Tests.Sources
{
    [TestFixture]
    public sealed class ApiSourceTests
    {
        private ISource _source;
        
        private Mock<ILog> _logMock;
        private Mock<ISerializer> _serializerMock;
        private Mock<IClient> _clientMock;
        
        [SetUp]
        public void Init()
        {
            _logMock = new Mock<ILog>(MockBehavior.Loose);

            _serializerMock = new Mock<ISerializer>(MockBehavior.Loose);
            _serializerMock.Setup(_ => _.Serialize(It.IsAny<object>()))
                .Returns("TestSerializer");
            _serializerMock.Setup(_ => _.Deserialize<Heroes>(It.IsAny<string>()))
                .Returns(() => new Heroes()
                {
                    Abathur = new Hero()
                    {
                        Id = "Aba",
                        Name = "aba",
                        Builds = new[]
                        {
                            new Build()
                            {
                                Name = "TestBuild",
                                Description = "Blah blah blah",
                                Url = "http://testbuild.com/"
                            }
                        }
                    }
                });
            
            _clientMock = new Mock<IClient>(MockBehavior.Loose);
            _clientMock.Setup(_ => _.GetString(It.IsAny<string>()))
                .ReturnsAsync("{\"Chromie\":{\"id\":\"Chromie\",\"name\":\"chromie\",\"builds\":[{\"name\":\"Sand Blast + Sands\",\"desc\":\"The standard build for Chromie. Timew\",\"url\":\"https://psionic-storm.com/en/calculateur-de-talents/chromie/3-1-2-1-3-1-1/\",\"updated\":1512193210773},{\"name\":\"Safety\",\"desc\":\"A good Chromie should be quite safe with her enormous reach & range, but respond in peeling for you. \",\"url\":\"http://bit.ly/2tSfEAC\",\"updated\":1512193210773}],\"alt\":[\"gnome\",\"bronze\",\"dragon\",\"cromie\",\"chrome\",\"chromi\"],\"class\":\"\",\"points\":\"1\"}}");

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
        public void Returns_Empty_List_If_Source_Type_Is_Not_Api()
        {
            var heroes = _source.GetHeroes(new[] {
                new Source()
                {
                    Type = SourceType.Scraping, 
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
                    Type = SourceType.Api, 
                    Url = new Uri("http://TestSource.com")
                } 
            });

            Assert.IsNotNull(heroes);

            var collection = heroes as Contract.Dto.Hero[] ?? heroes.ToArray();
            Assert.IsNotEmpty(collection);

            var hero = collection.FirstOrDefault();

            Assert.IsNotNull(hero);
            Assert.AreEqual("aba", hero.Name);

            Assert.IsNotEmpty(hero.Builds);
            Assert.IsNotEmpty(hero.Builds);

            var build = hero.Builds.FirstOrDefault();

            Assert.IsNotNull(build);
            Assert.AreEqual("TestBuild", build.Title);
            Assert.AreEqual("http://testbuild.com/", build.Url.AbsoluteUri);
        }
        
        private ISource CreateSource(
            bool isILogNull = false,
            bool isISerializerNull = false,
            bool isIClientNull = false)
        {
            return new ApiSource(
                isILogNull ? null : new Lazy<ILog>(() => _logMock.Object),
                isISerializerNull ? null : new Lazy<ISerializer>(() => _serializerMock.Object),
                isIClientNull ? null : new Lazy<IClient>(() => _clientMock.Object));
        }
    }
}