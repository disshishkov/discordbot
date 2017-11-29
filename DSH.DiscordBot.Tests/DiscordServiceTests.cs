using System;
using DSH.DiscordBot.Clients;
using DSH.DiscordBot.Host.Service;
using DSH.DiscordBot.Infrastructure.Logging;
using Moq;
using NUnit.Framework;

namespace DSH.DiscordBot.Tests
{
    [TestFixture]
    public sealed class DiscordServiceTests
    {
        private IService _service;
        
        private Mock<ILog> _logMock;
        private Mock<IDiscordClient> _discordClientMock;
        
        [SetUp]
        public void Init()
        {
            _logMock = new Mock<ILog>(MockBehavior.Loose);
            _discordClientMock = new Mock<IDiscordClient>(MockBehavior.Loose);

            _service = CreateService();
        }
        
        [Test]
        public void Throws_If_ILog_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _service = CreateService(isILogNull: true);
            });
        }
        
        [Test]
        public void Throws_If_IDiscordClient_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _service = CreateService(isIDiscordClientNull: true);
            });
        }

        [Test]
        public void Can_Start()
        {
            Assert.IsTrue(_service.Start());
        }
        
        [Test]
        public void Can_Stop()
        {
            Assert.IsTrue(_service.Stop());
        }

        private IService CreateService(
            bool isILogNull = false,
            bool isIDiscordClientNull = false)
        {
            return new DiscordService(
                isILogNull ? null : new Lazy<ILog>(() => _logMock.Object),
                isIDiscordClientNull ? null : new Lazy<IDiscordClient>(() => _discordClientMock.Object));
        }
    }
}