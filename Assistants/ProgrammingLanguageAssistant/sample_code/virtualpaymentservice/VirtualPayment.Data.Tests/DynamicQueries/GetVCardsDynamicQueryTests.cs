using NUnit.Framework;
using System.Reflection;
using VirtualPaymentService.Data.DynamicQueries;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Data.Tests.DynamicQueries
{
    [TestFixture]
    public class GetVCardsDynamicQueryTests
    {
        private GetVCardsDynamicQuery vCardQuery = new GetVCardsDynamicQuery();

        [SetUp]
        public void SetUp()
        {
            vCardQuery = new GetVCardsDynamicQuery();
        }

        [TestCaseSource(nameof(TestCases))]
        public void Setters_Behave_As_Expected(DynamicQueryCase testCase)
        {
            // Arrange
            PropertyInfo prop = typeof(GetVCardsDynamicQuery).GetProperty(testCase.PropertyName ?? string.Empty) ??
                throw new Exception($"No property by the name of {testCase.PropertyName} was found on type {nameof(GetVCardsDynamicQuery)}");

            var expectedSql = $" WHERE {testCase.ColumnName} {testCase.ConditionalOperator} @{testCase.ParamName};";

            // Act
            prop.SetValue(vCardQuery, testCase.PropertyValue);
            var dynamicQuery = vCardQuery.BuildParameterizedQuery(string.Empty);

            // Assert
            Assert.Multiple(() =>
            {
                if (testCase.ShouldUpdateQuery)
                {
                    Assert.That(dynamicQuery.RawSql, Is.EqualTo(expectedSql));
                    Assert.That(dynamicQuery.QueryParams[testCase.ParamName], Is.EqualTo(testCase.ExpectedValue ?? testCase.PropertyValue));
                }
                else
                {
                    Assert.That(dynamicQuery.RawSql, Is.EqualTo(";"));
                    Assert.That(dynamicQuery.QueryParams.ContainsKey(testCase.ParamName), Is.False);
                }
            });
        }

        private static IEnumerable<DynamicQueryCase[]> TestCases()
        {
            var dateOffset = DateTimeOffset.Now;
            var mstDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateOffset, "Mountain Standard Time").DateTime;
            var utcDateTime = dateOffset.UtcDateTime;
            var testCases = new List<DynamicQueryCaseParams>
            {
                // List cases
                new DynamicQueryCaseParams {
                     new DynamicQueryCase
                     {
                        PropertyName = nameof(GetVCardsDynamicQuery.LeaseIds),
                        PropertyValue = new List<long>() { { 1234 } },
                        ColumnName = nameof(VCard.LeaseId),
                        ParamName = nameof(VCard.LeaseId),
                        ConditionalOperator = ConditionalOperator.IN,
                        ShouldUpdateQuery = true,
                     }
                },
                new DynamicQueryCaseParams {
                     new DynamicQueryCase
                     {
                        PropertyName = nameof(GetVCardsDynamicQuery.ProviderIds),
                        PropertyValue = new List<VPaymentProvider>() { { VPaymentProvider.Marqeta } },
                        ColumnName = nameof(VCard.VCardProviderId),
                        ParamName = nameof(VCard.VCardProviderId),
                        ConditionalOperator = ConditionalOperator.IN,
                        ShouldUpdateQuery = true,
                     }
                },
                new DynamicQueryCaseParams {
                     new DynamicQueryCase
                     {
                        PropertyName = nameof(GetVCardsDynamicQuery.VCardStatusIds),
                        PropertyValue = new List<CardStatus>() { { CardStatus.Authorized } },
                        ColumnName = nameof(VCard.VCardStatusId),
                        ParamName = nameof(VCard.VCardStatusId),
                        ConditionalOperator = ConditionalOperator.IN,
                        ShouldUpdateQuery = true,
                     }
                },
                // Date cases
                new DynamicQueryCaseParams {
                     new DynamicQueryCase
                     {
                        PropertyName = nameof(GetVCardsDynamicQuery.MinActiveToDate),
                        PropertyValue = dateOffset,
                        ExpectedValue = mstDateTime,
                        ColumnName = nameof(VCard.ActiveToDate),
                        ParamName = nameof(VCard.ActiveToDate),
                        ConditionalOperator = ConditionalOperator.GTEQ,
                        ShouldUpdateQuery = true,
                     }
                },
                new DynamicQueryCaseParams {
                     new DynamicQueryCase
                     {
                        PropertyName = nameof(GetVCardsDynamicQuery.MinExpirationDate),
                        PropertyValue = dateOffset,
                        ExpectedValue = utcDateTime,
                        ColumnName = nameof(VCard.ExpirationDate),
                        ParamName = nameof(VCard.ExpirationDate),
                        ConditionalOperator = ConditionalOperator.GTEQ,
                        ShouldUpdateQuery = true,
                     }
                },
                new DynamicQueryCaseParams {
                     new DynamicQueryCase
                     {
                        PropertyName = nameof(GetVCardsDynamicQuery.MinUpdatedDate),
                        PropertyValue = dateOffset,
                        ExpectedValue = mstDateTime,
                        ColumnName = nameof(VCard.UpdatedDate),
                        ParamName = nameof(VCard.UpdatedDate),
                        ConditionalOperator = ConditionalOperator.GTEQ,
                        ShouldUpdateQuery = true,
                     }
                },
                new DynamicQueryCaseParams {
                     new DynamicQueryCase
                     {
                        PropertyName = nameof(GetVCardsDynamicQuery.MaxUpdatedDate),
                        PropertyValue = dateOffset,
                        ExpectedValue = mstDateTime,
                        ColumnName = nameof(VCard.UpdatedDate),
                        ParamName = nameof(GetVCardsDynamicQuery.MaxUpdatedDate),
                        ConditionalOperator = ConditionalOperator.LTEQ,
                        ShouldUpdateQuery = true,
                     }
                },
                new DynamicQueryCaseParams {
                     new DynamicQueryCase
                     {
                        PropertyName = nameof(GetVCardsDynamicQuery.MaxExpirationDate),
                        PropertyValue = dateOffset,
                        ExpectedValue = utcDateTime,
                        ColumnName = nameof(VCard.ExpirationDate),
                        ParamName = nameof(GetVCardsDynamicQuery.MaxExpirationDate),
                        ConditionalOperator = ConditionalOperator.LTEQ,
                        ShouldUpdateQuery = true,
                     }
                },
                new DynamicQueryCaseParams {
                     new DynamicQueryCase
                     {
                        PropertyName = nameof(GetVCardsDynamicQuery.MaxActiveToDate),
                        PropertyValue = dateOffset,
                        ExpectedValue = mstDateTime,
                        ColumnName = nameof(VCard.ActiveToDate),
                        ParamName = nameof(GetVCardsDynamicQuery.MaxActiveToDate),
                        ConditionalOperator = ConditionalOperator.LTEQ,
                        ShouldUpdateQuery = true,
                     }
                },
            };

            foreach (var caseParams in testCases)
            {
                // Create empty list test cases
                if (caseParams[0].ConditionalOperator == ConditionalOperator.IN)
                {
                    // Create empty enumerable
                    var newCase = caseParams[0].Clone();
                    newCase.PropertyValue = newCase.PropertyValue is null ? null : Activator.CreateInstance(newCase.PropertyValue.GetType());
                    newCase.ShouldUpdateQuery = false;

                    yield return new DynamicQueryCase[] { newCase };
                }
                else if (caseParams[0].PropertyValue?.GetType() == typeof(DateTimeOffset))
                {
                    var newCase = caseParams[0].Clone();
                    newCase.PropertyValue = null;
                    newCase.ShouldUpdateQuery = false;

                    yield return new DynamicQueryCase[] { newCase };
                }

                // Convert DynamicQueryCaseParams to DynamicQueryCase[]
                yield return caseParams.ToArray();
            }
        }

        /// <summary>
        /// Just used for code clarity
        /// </summary>
        public class DynamicQueryCaseParams : List<DynamicQueryCase> { }

        /// <summary>
        /// Just used for code clarity
        /// </summary>
        public class DynamicQueryCase
        {
            public string? PropertyName { get; set; }

            public object? PropertyValue { get; set; }

            public object? ExpectedValue { get; set; }

            public string? ColumnName { get; set; }

            public string? ParamName { get; set; }

            public string? ConditionalOperator { get; set; }

            public bool ShouldUpdateQuery { get; set; }

            public DynamicQueryCase Clone()
            {
                return (DynamicQueryCase)MemberwiseClone();
            }
        }
    }
}
