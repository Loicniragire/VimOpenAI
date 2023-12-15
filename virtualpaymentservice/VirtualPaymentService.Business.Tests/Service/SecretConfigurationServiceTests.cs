using Moq;
using NUnit.Framework;
using ProgLeasing.Platform.SecretConfiguration;
using ProgLeasing.System.Logging.Contract;
using System;
using System.Configuration;
using VirtualPaymentService.Business.Configuration;
using VirtualPaymentService.Business.Service;

namespace VirtualPaymentService.Business.Tests.Service
{
    [TestFixture]
    public class SecretConfigurationServiceTests
    {
        #region Field
        private Mock<ILogger<SecretConfigurationService>> _mockLogger;
        private Mock<ISecretManager> _mockSecretManager;
        private Mock<ISecretResult> _mockSecretResult;
        private AppSettings _appsettings;
        private SecretConfigurationService _secretConfigurationService;
        #endregion Field

        #region Setup
        [SetUp]
        public void Initialize()
        {
            _mockLogger = new Mock<ILogger<SecretConfigurationService>>();
            _mockSecretResult = new Mock<ISecretResult>();
            _mockSecretManager = new Mock<ISecretManager>();

            _appsettings = new AppSettings();
            _appsettings.CardProviderSettings.Add(new CardProviderSetting { ApiUserKeyName = "TestUserName1", ApiPasswordKeyName = "TestPassword1" });
            _appsettings.CardProviderSettings.Add(new CardProviderSetting { ApiUserKeyName = "TestUserName2", ApiPasswordKeyName = "TestPassword2" });
        }
        #endregion Setup

        #region Constructor Test
        [TestCase("TestUser3", "")]
        [TestCase("TestUser3", null)]
        [TestCase("", "TestPassword3")]
        [TestCase(null, "TestPassword3")]
        [TestCase("", "")]
        [TestCase(null, null)]
        public void Constructor_WhenKeyNameNotExistsInAppSettings_ThenConfigurationErrorsExceptionReturned(string userKeyName, string passwordKeyName)
        {
            // Assemble
            _mockSecretResult.Setup(res => res.Success).Returns(true);
            _mockSecretResult.Setup(res => res.Value).Returns("RandomSecretValue");
            _mockSecretManager.Setup(sm => sm.GetSecret(It.IsAny<string>())).Returns(_mockSecretResult.Object);

            // Add a CardProviderSetting that is missing required properties to be set.
            _appsettings.CardProviderSettings.Add(new CardProviderSetting
            {
                ApiUserKeyName = userKeyName,
                ApiPasswordKeyName = passwordKeyName,
                CardProvider = Model.Enums.VirtualCardProviderNetwork.Marqeta,
                ProductType = Model.Enums.ProductType.Commercial
            });

            // Act
            void actual() => _secretConfigurationService = new SecretConfigurationService(_mockSecretManager.Object, _appsettings, _mockLogger.Object);

            // Assert
            Assert.Throws<ConfigurationErrorsException>(actual);
            _mockLogger.Verify(logger => logger.Log(LogLevel.Error, It.IsAny<string>(), It.IsAny<Exception>(), null), Times.Once);
        }

        [TestCase("Error message returned from secret server.", "Error message returned from secret server.")]
        [TestCase("Unknown error when trying to retrieve secret value for key TestUserName1.", null)]
        [TestCase("Unknown error when trying to retrieve secret value for key TestUserName1.", "   ")]
        [TestCase("Unknown error when trying to retrieve secret value for key TestUserName1.", "")]
        public void Constructor_WhenArgumentExceptionThrown_ThenSameErrorFromSecretServerReturnedIfPresent(string expected, string errorFromSecretServer)
        {
            // Assemble
            _mockSecretResult.Setup(res => res.Success).Returns(false);
            _mockSecretResult.Setup(res => res.ErrorMessage).Returns(errorFromSecretServer);
            _mockSecretManager.Setup(sm => sm.GetSecret(It.IsAny<string>())).Returns(_mockSecretResult.Object);

            // Act
            void actual() => new SecretConfigurationService(_mockSecretManager.Object, _appsettings, _mockLogger.Object);

            // Assert
            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo(expected), delegate { actual(); });
        }

        [Test]
        public void Constructor_WhenFinalExceptionThrown_ThenLogContainsTextToKeySplunkAlertFrom()
        {
            // Assemble
            string expected = "Failure to retrieve value from secrets store after all configured retries!";

            _appsettings.SecretServerGetSecretRetryLimit = 2;

            _mockSecretResult.Setup(res => res.Success).Returns(false);
            _mockSecretResult.Setup(res => res.ErrorMessage).Returns("Random error!");
            _mockSecretManager.Setup(sm => sm.GetSecret(It.IsAny<string>())).Returns(_mockSecretResult.Object);

            // Act
            void actual() => new SecretConfigurationService(_mockSecretManager.Object, _appsettings, _mockLogger.Object);

            // Assert
            Assert.Catch(actual);
            // Validate the error logged starts with the text the Splunk alert will be based on
            _mockLogger.Verify(logger => logger.Log(LogLevel.Error, It.Is<string>(str => str.StartsWith(expected)), It.IsAny<Exception>(), null), Times.Exactly(1));
        }

        [Test]
        public void Constructor_WhenSecretServerGetSecretRetryLimitGreaterThanZero_ThenRetrySettingHonored()
        {
            // Assemble
            int expected = 2;

            _mockSecretResult.Setup(res => res.Success).Returns(false);
            _mockSecretResult.Setup(res => res.ErrorMessage).Returns("Random Error");
            _mockSecretManager.Setup(sm => sm.GetSecret(It.IsAny<string>())).Returns(_mockSecretResult.Object);

            // Set the retry to the number of time we expect the retry to execute
            _appsettings.SecretServerGetSecretRetryLimit = expected;

            // Act
            void actual() => new SecretConfigurationService(_mockSecretManager.Object, _appsettings, _mockLogger.Object);

            // Assert
            Assert.Throws<ArgumentException>(actual);
            // Validating the number of times we logged a warning, this would equal the number of times we retried the call
            _mockLogger.Verify(logger => logger.Log(LogLevel.Warn, It.IsAny<string>(), It.IsAny<Exception>(), null), Times.Exactly(expected));
        }
        #endregion Constructor Test

        #region GetSecretValueForKey Test
        [Test]
        public void GetSecretValueForKey_WhenKeyNameExistsInAppSettings_ThenKeyValueReturned()
        {
            // Assemble
            var userKeyName = "TestUserName3";
            var expected = "SecretMarqetaUserName";

            _mockSecretResult.Setup(res => res.Success).Returns(true);
            _mockSecretResult.Setup(res => res.Value).Returns(expected);
            _mockSecretManager.Setup(sm => sm.GetSecret(It.IsAny<string>())).Returns(_mockSecretResult.Object);
            _appsettings.CardProviderSettings.Add(new CardProviderSetting { ApiUserKeyName = userKeyName, ApiPasswordKeyName = "TestPassword3" });
            _secretConfigurationService = new SecretConfigurationService(_mockSecretManager.Object, _appsettings, _mockLogger.Object);

            // Act
            var actual = _secretConfigurationService.GetSecretValueForKey(userKeyName);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetSecretValueForKey_WhenKeyNameNotExistsInAppSettings_ThenEmptyStringReturned()
        {
            // Assemble
            var keyNameNotInAppSettings = "RandomKeyName";
            var expected = string.Empty;

            _mockSecretResult.Setup(res => res.Success).Returns(true);
            _mockSecretResult.Setup(res => res.Value).Returns(expected);
            _mockSecretManager.Setup(sm => sm.GetSecret(It.IsAny<string>())).Returns(_mockSecretResult.Object);
            _secretConfigurationService = new SecretConfigurationService(_mockSecretManager.Object, _appsettings, _mockLogger.Object);

            // Act
            var actual = _secretConfigurationService.GetSecretValueForKey(keyNameNotInAppSettings);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetSecretValueForKey_WhenRetrySucceedsAfterFailure_ThenValueReturned()
        {
            // Assemble
            var expected = "SecretMarqetaUserName";

            var successResult = new Mock<ISecretResult>();
            successResult.Setup(res => res.Success).Returns(true);
            successResult.Setup(res => res.Value).Returns(expected);

            var errorResult = new Mock<ISecretResult>();
            errorResult.Setup(res => res.Success).Returns(false);
            errorResult.Setup(res => res.ErrorMessage).Returns("Random Error");

            // Returns an error first, then success on retry.
            // NOTE: 3rd-5th is for remaining properties set in appsettings during test setup.
            _mockSecretManager.SetupSequence(sm => sm.GetSecret(It.IsAny<string>()))
                .Returns(errorResult.Object)
                .Returns(successResult.Object)
                .Returns(successResult.Object)
                .Returns(successResult.Object)
                .Returns(successResult.Object);

            _appsettings.SecretServerGetSecretRetryLimit = 2;

            // This is the first 
            var keyNameToSearch = _appsettings.CardProviderSettings[0].ApiUserKeyName;

            _secretConfigurationService = new SecretConfigurationService(_mockSecretManager.Object, _appsettings, _mockLogger.Object);

            // Act
            var actual = _secretConfigurationService.GetSecretValueForKey(keyNameToSearch);

            // Assert
            Assert.AreEqual(expected, actual);
            // Warning is logged once with first error.
            _mockLogger.Verify(logger => logger.Log(LogLevel.Warn, It.IsAny<string>(), It.IsAny<Exception>(), null), Times.Exactly(1));
            // Error not logged since second call was successful.
            _mockLogger.Verify(logger => logger.Log(LogLevel.Error, It.IsAny<string>(), It.IsAny<Exception>(), null), Times.Exactly(0));
        }
        #endregion GetSecretValueForKey Test
    }
}