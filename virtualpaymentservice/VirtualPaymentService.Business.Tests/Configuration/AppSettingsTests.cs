namespace VirtualPaymentService.Business.Tests.Configuration
{
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;
    using System.Configuration;
    using System.IO;
    using System.Text;
    using System.Text.Json;
    using VirtualPaymentService.Business.Configuration;
    using VirtualPaymentService.Model.Enums;

    [TestFixture]
    public class AppSettingsTests
    {
        private AppSettings appSettings;

        private string Key1Value => "Knowledge is key my young";
        private string Key2Value => "Knowledge is key my elderly";

        [SetUp]
        public void SetUp()
        {
            appSettings = new AppSettings();
            appSettings.ENV = "LocalUnitTests";
            appSettings.Urls.Add("MyKey1", $"{Key1Value} url");
            appSettings.Urls.Add("MyKey2", $"{Key2Value} url");
            appSettings.Endpoints.Add("MyKey1", $"{Key1Value} endpoint");
            appSettings.Endpoints.Add("MyKey2", $"{Key2Value} endpoint");
        }

        #region GetUrl Method
        [Test]
        public void GetUrl_ReturnsCorrectValueForKey()
        {
            // Act
            var actualKey1Value = appSettings.GetUrl("MyKey1");
            var actualKey2Value = appSettings.GetUrl("MyKey2");

            // Assert
            Assert.AreEqual($"{Key1Value} url", actualKey1Value);
            Assert.AreEqual($"{Key2Value} url", actualKey2Value);
        }

        [TestCase("fake")]
        [TestCase("   ")]
        public void ReturnsEmptyStringWhenProvidedInvalidUrlKey(string key)
        {
            Assert.IsEmpty(appSettings.GetUrl(key));
        }
        #endregion GetUrl Method

        #region GetEndpoint Method
        [Test]
        public void GetEndpoint_ReturnsCorrectValueForKey()
        {
            // Act
            var actualKey1Value = appSettings.GetEndpoint("MyKey1");
            var actualKey2Value = appSettings.GetEndpoint("MyKey2");

            // Assert
            Assert.AreEqual($"{Key1Value} endpoint", actualKey1Value);
            Assert.AreEqual($"{Key2Value} endpoint", actualKey2Value);
        }

        [TestCase("")]
        [TestCase("  fake ")]
        public void ReturnsEmptyStringWhenProvidedInvalidEndpointKey(string key)
        {
            Assert.IsEmpty(appSettings.GetEndpoint(key));
        }
        #endregion GetEndpoint Method

        #region ENV Property
        /// <summary>
        /// Adding unit test to provide full unit test code coverage.
        /// </summary>
        [Test]
        public void Env_WhenPropertyAccessed_ThenCurrentValueReturned()
        {
            // Arrange
            var expected = "LocalUnitTests";

            // Act
            var actual = appSettings.ENV;

            // Assert
            Assert.AreEqual(expected, actual);
        }
        #endregion ENV Property

        #region GetSecretServerKeyName Method
        [Test]
        public void GetSecretServerKeyName_WhenKeyNamePresent_ThenKeyNameValueReturned()
        {
            // Arrange
            var expected = "SecretKeyValue";
            var keyName = "SecretKeyName";

            appSettings.SecretServerKeyNames.Add(keyName, expected);

            // Act
            var actual = appSettings.GetSecretServerKeyName(keyName);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetSecretServerKeyName_WhenKeyNameNotPresent_ThenEmptyStringReturned()
        {
            // Arrange
            var keyName = "SecretKeyName";

            appSettings.SecretServerKeyNames.Add(keyName, "RandomKeyValue");

            // Act
            var actual = appSettings.GetSecretServerKeyName("RandomKeyName");

            // Assert
            Assert.AreEqual(string.Empty, actual);
        }

        #endregion GetSecretServerKeyName Method

        #region DigitalWalletTokenCountLimit Property
        [Test]
        public void DigitalWalletTokenCountLimit_WhenMissingFromAppsettings_ThenDefaultsTo5()
        {
            // Arrange
            var settings = new
            {
                AppSettings = new { }
            };
            var json = JsonSerializer.Serialize(settings);

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            IConfiguration configuration = new ConfigurationBuilder().AddJsonStream(stream).Build();

            // Act
            var appsettings = new AppSettings();
            configuration.GetSection("AppSettings").Bind(appsettings);

            // Assert
            Assert.AreEqual(5, appsettings.DigitalWalletTokenCountLimit);
        }

        [Test]
        public void DigitalWalletTokenCountLimit_WhenSetInAppsettings_ThenReturnsProperValue()
        {
            // Arrange
            var expected = 100;
            var settings = new
            {
                AppSettings = new
                {
                    DigitalWalletTokenCountLimit = expected,
                }
            };
            var json = JsonSerializer.Serialize(settings);

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            IConfiguration configuration = new ConfigurationBuilder().AddJsonStream(stream).Build();

            // Act
            var appsettings = new AppSettings();
            configuration.GetSection("AppSettings").Bind(appsettings);

            // Assert
            Assert.AreEqual(expected, appsettings.DigitalWalletTokenCountLimit);
        }
        #endregion DigitalWalletTokenCountLimit Property

        #region GetCardProviderSetting Tests
        [Test]
        public void GetCardProviderSetting_WhenProviderProductExists_ThenSettingsReturned()
        {
            var cardProvider = VirtualCardProviderNetwork.Marqeta;
            var productType = ProductType.Commercial;


            var expected = new CardProviderSetting
            {
                ApiPasswordKeyName = "Password",
                ApiUserKeyName = "UserName",
                BaseUrl = "https://somewhere.com",
                VirtualCardProductToken = "ed526afa-c3b8-475b-a17a-c025cf47272b",
                SetInitalPin = true,
                CardProvider = cardProvider,
                ProductType = productType
            };

            // Arrange
            appSettings.CardProviderSettings.Add(expected);

            appSettings.CardProviderSettings.Add(new CardProviderSetting
            {
                ApiPasswordKeyName = "Password 1",
                ApiUserKeyName = "UserName 1",
                BaseUrl = "https://somewhere1.com",
                VirtualCardProductToken = "ed526afa-c3b8-475b-a17a-c025cf47272b 1",
                SetInitalPin = false,
                CardProvider = VirtualCardProviderNetwork.Marqeta,
                ProductType = ProductType.Consumer
            });

            // Act
            var actual = appSettings.GetCardProviderSetting(cardProvider, productType);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [TestCase(VirtualCardProviderNetwork.Marqeta, ProductType.Consumer)]
        [TestCase(null, ProductType.Consumer)]
        [TestCase(VirtualCardProviderNetwork.Marqeta, null)]
        public void GetCardProviderSetting_WhenProviderProductNotExists_ThenExceptionReturned(VirtualCardProviderNetwork provider, ProductType type)
        {
            var cardProvider = VirtualCardProviderNetwork.Marqeta;
            var productType = ProductType.Commercial;

            // Arrange
            appSettings.CardProviderSettings.Add(new CardProviderSetting
            {
                ApiPasswordKeyName = "Password",
                ApiUserKeyName = "UserName",
                BaseUrl = "https://somewhere.com",
                VirtualCardProductToken = "ed526afa-c3b8-475b-a17a-c025cf47272b 1",
                SetInitalPin = false,
                CardProvider = provider,
                ProductType = type
            });

            // Act
            ConfigurationErrorsException actual = Assert.Throws<ConfigurationErrorsException>(() => appSettings.GetCardProviderSetting(cardProvider, productType));

            // Assert
            actual.Message.Contains($"Did not locate the CardProviderSetting in CardProviderSettings of appsetting.json for provider");
        }
        #endregion GetCardProviderSetting Tests
    }
}