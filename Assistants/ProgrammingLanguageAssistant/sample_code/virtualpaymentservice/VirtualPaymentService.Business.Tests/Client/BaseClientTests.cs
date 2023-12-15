using Moq;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Text;
using VirtualPaymentService.Business.Client;
using VirtualPaymentService.Business.Configuration;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Business.Tests.Client
{
    [TestFixture]
    public class BaseClientTests
    {
        #region TestBaseClient class
        private class TestBaseClient : BaseClient
        {
            #region Field
            public HttpClient fakeClient;
            #endregion Field

            #region Constructor
            public TestBaseClient(HttpClient client, AppSettings appSettings) : base(client, appSettings)
            {
                fakeClient = client;
            }

            public TestBaseClient(
                HttpClient client,
                AppSettings appSettings,
                ISecretConfigurationService secretConfig) : base(client, appSettings, secretConfig)
            {
                fakeClient = client;
            }
            #endregion Constructor

            #region Property
            public bool PassKeyIsRead { get; set; } = false;
            public bool UserKeyIsRead { get; set; } = false;
            protected override VirtualCardProviderNetwork CardProvider => VirtualCardProviderNetwork.Marqeta;
            protected override ProductType CardProductType => ProductType.Commercial;
            #endregion
        }
        #endregion TestBaseClient class

        #region Field
        public static string userKey;
        public static string passKey;
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpClient _client;
        private AppSettings _appsettings;
        private Mock<ISecretConfigurationService> _mockSecretConfigurationService;
        #endregion Field

        #region Setup
        [SetUp]
        public void SetUp()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _client = new HttpClient(_mockHttpMessageHandler.Object);

            _appsettings = new AppSettings();
            _appsettings.CardProviderSettings.Add(new CardProviderSetting
            {
                CardProvider = VirtualCardProviderNetwork.Marqeta,
                ProductType = ProductType.Commercial,
                BaseUrl = "https://www.espn.com",
                ApiUserKeyName = "MarqetaApiUsername",
                ApiPasswordKeyName = "MarqetaApiPassword"
            });

            _mockSecretConfigurationService = new Mock<ISecretConfigurationService>();
        }
        #endregion Setup

        #region BaseClient InitializeClient Tests
        [Test]
        public void InitializeClient_WhenSecretConfigurationServiceNull_ThenAuthHeaderNotSet()
        {
            // Assemble
            var testClient = new TestBaseClient(_client, _appsettings);

            // Act
            var actual = testClient.fakeClient.DefaultRequestHeaders.Authorization;

            // Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void InitializeClient_WhenSecretConfigurationServicePresent_ThenAuthHeaderSet()
        {
            // Assemble
            string userName = "USERNAME";
            string password = "PASSWORD";

            _mockSecretConfigurationService.Setup(x => x.GetSecretValueForKey("MarqetaApiUsername")).Returns(userName);
            _mockSecretConfigurationService.Setup(x => x.GetSecretValueForKey("MarqetaApiPassword")).Returns(password);

            var expected = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{password}"));

            // Act
            var client = new TestBaseClient(_client, _appsettings, _mockSecretConfigurationService.Object);
            var authHeader = client.fakeClient.DefaultRequestHeaders.Authorization;

            // Assert
            Assert.AreEqual(expected, authHeader.Parameter);
        }
        #endregion BaseClient InitializeClient Tests
    }
}