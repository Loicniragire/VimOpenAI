using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using VirtualPaymentService.Business.Common.AutoMapper;
using VirtualPaymentService.Model.Responses;
using VirtualPaymentService.Model.Responses.Marqeta;

namespace VirtualPaymentService.Business.Tests.Common.AutoMapper
{
    [TestFixture]
    public class VirtualCardProfileTests
    {
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<VirtualCardProfile>();
            });
            _mapper = mapperConfig.CreateMapper();
        }

        [Test]
        public void VirtualCardProfile_IsValid()
        {
            // Assemble
            var config = new MapperConfiguration(cfg => cfg.AddProfile<VirtualCardProfile>());

            // Assert
            Assert.DoesNotThrow(() => config.AssertConfigurationIsValid());
        }

        #region MarqetaUserResponseMapToCardUserUpdateResponse Tests
        /// <summary>
        /// This test validates that we handle the zip code value returned from Marqeta correctly.
        /// In production the <see cref="MarqetaUserResponse"/> returns the zip code in the "zip" named property
        /// but in lower lanes (sandbox) this value is returned in the "postal_code" property.
        /// If zip is present we will use that value if not then we will look at the postal_code for that value.
        /// </summary>
        [TestCase("84095", "", "84095")]
        [TestCase("84095", null, "84095")]
        [TestCase("", "91423", "91423")]
        [TestCase(null, "91423", "91423")]
        [TestCase("84095", "91423", "84095")]
        [TestCase("", "", "")]
        [TestCase(null, null, null)]
        public void MarqetaUserResponseMapToCardUserUpdateResponse_WhenZipCodeReturned_ThenCorrectSourceIsChosen(string zipCode, string postalCode, string expectedValue)
        {
            // Assemble
            var marqetaUserResponse = new MarqetaUserResponse
            {
                PostalCode = postalCode,
                Zip = zipCode,
                FirstName = "John",
                LastName = "Smith",
                Address1 = "123 Main St",
                Address2 = "#123",
                City = "South Jordan",
                State = "UT",
                Active = true,
                CreatedTime = System.DateTime.Now,
                LastModifiedTime = System.DateTime.Now,
                Token = "123456",
                Phone = "+18015551212"
            };

            var expectedCardUserUpdateResponse = new CardUserUpdateResponse
            {
                PostalCode = expectedValue,
                FirstName = marqetaUserResponse.FirstName,
                LastName = marqetaUserResponse.LastName,
                Address1 = marqetaUserResponse.Address1,
                Address2 = marqetaUserResponse.Address2,
                City = marqetaUserResponse.City,
                State = marqetaUserResponse.State,
                Active = marqetaUserResponse.Active,
                CreatedTime = marqetaUserResponse.CreatedTime,
                LastModifiedTime = marqetaUserResponse.LastModifiedTime,
                PhoneNumber = marqetaUserResponse.Phone
            };

            // Act
            var actual = _mapper.Map<CardUserUpdateResponse>(marqetaUserResponse);

            // Assert
            actual.Should().BeEquivalentTo(expectedCardUserUpdateResponse);
        }

        #endregion MarqetaUserResponseMapToCardUserUpdateResponse Tests
    }
}