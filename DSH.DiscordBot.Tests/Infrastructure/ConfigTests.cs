using System;
using System.Collections.Specialized;
using System.Linq;
using DSH.DiscordBot.Infrastructure.Configuration;
using Moq;
using NUnit.Framework;

namespace DSH.DiscordBot.Tests.Infrastructure
{
    [TestFixture]
    public sealed class ConfigTests
    {
        private IConfig _config;

        private Mock<IAppSettings> _appSettingsMock;

        [SetUp]
        public void Init()
        {
            _appSettingsMock = new Mock<IAppSettings>(MockBehavior.Loose);
            _appSettingsMock.Setup(_ => _.Get())
                .Returns(new NameValueCollection()
                {
                    {"Token", "TestToken"},
                    {"DbConnectionString", "TestDb"},
                    {"AdminName", "TestAdmin"},
                    {"CommandPrefix", "!"},
                    {"Sources", "Source1|Source2"}
                });

            _config = CreateConfig();
        }

        [Test]
        public void Throws_If_IAppSettings_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _config = CreateConfig(isIAppSettingsNull: true);
            });
        }

        [Test]
        public void Can_Obtain_Token()
        {
            Assert.AreEqual("TestToken", _config.Token);
        }

        [Test]
        public void Can_Obtain_DbConnectionString()
        {
            Assert.AreEqual("TestDb", _config.DbConnectionString);
        }

        [Test]
        public void Can_Obtain_CommandPrefix()
        {
            Assert.AreEqual("!", _config.CommandPrefix);
        }

        [Test]
        public void Can_Obtain_Sources()
        {
            Assert.IsNotNull(_config.Sources);
            Assert.IsNotEmpty(_config.Sources);
            Assert.AreEqual(2, _config.Sources.Count());
            Assert.AreEqual("Source1", _config.Sources.FirstOrDefault());
            Assert.AreEqual("Source2", _config.Sources.LastOrDefault());
        }

        [Test]
        public void Returns_Null_Sources_If_Config_Field_IS_Empty()
        {
            _appSettingsMock.Setup(_ => _.Get())
                .Returns(new NameValueCollection()
                {
                    {"Token", "TestToken"},
                    {"DbConnectionString", "TestDb"},
                    {"AdminName", "TestAdmin"},
                    {"CommandPrefix", "TestPrefix"},
                    {"Sources", ""}
                });

            Assert.IsNull(_config.Sources);
        }

        private IConfig CreateConfig(
            bool isIAppSettingsNull = false)
        {
            return new Config(
                isIAppSettingsNull ? null : new Lazy<IAppSettings>(() => _appSettingsMock.Object));
        }
    }
}