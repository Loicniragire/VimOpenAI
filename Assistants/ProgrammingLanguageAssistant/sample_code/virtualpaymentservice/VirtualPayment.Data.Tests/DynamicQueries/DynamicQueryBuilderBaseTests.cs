using NUnit.Framework;
using VirtualPaymentService.Data.DynamicQueries;

namespace VirtualPaymentService.Data.Tests.DynamicQueries
{
    [TestFixture]
    public class DynamicQueryBuilderBaseTests
    {
        #region Helpers
        private class TestDynamicQueryBuilder : DynamicQueryBuilderBase
        {
            public new void AddParameterizedCondition(string columnName, string conditionalOperator, object value, string? paramName = null, bool overwriteParam = false)
            {
                base.AddParameterizedCondition(columnName, conditionalOperator, value, paramName, overwriteParam);
            }
        }

        private static TestDynamicQueryBuilder GetQueryBuilder()
        {
            return new TestDynamicQueryBuilder();
        }
        #endregion Helpers

        [Test]
        public void BuildParameterizedQuery_DoesNotAddWhereClause_When_NoParametersAreProvided()
        {
            // Arrange
            var baseQuery = " SELECT * FROM Progressive.VPay.VCard ";
            var expected = string.Concat(baseQuery.Trim(), ";");

            var queryBuilder = GetQueryBuilder();

            // Act
            var actual = queryBuilder.BuildParameterizedQuery(baseQuery);

            // Assert
            Assert.That(actual.RawSql, Is.EqualTo(expected));
        }

        [TestCase("TestColumn1", ConditionalOperator.EQ)]
        [TestCase("TestColumn2", ConditionalOperator.GTEQ)]
        [TestCase("TestColumn3", ConditionalOperator.LTEQ)]
        [TestCase("TestColumn4", ConditionalOperator.IN)]
        [TestCase("TestColumn5", ConditionalOperator.LIKE)]
        [TestCase("TestColumn1", ConditionalOperator.EQ, "TestParamName1")]
        [TestCase("TestColumn2", ConditionalOperator.GTEQ, "TestParamName2")]
        [TestCase("TestColumn3", ConditionalOperator.LTEQ, "TestParamName3")]
        [TestCase("TestColumn4", ConditionalOperator.IN, "TestParamName4")]
        [TestCase("TestColumn5", ConditionalOperator.LIKE, "TestParamName5")]
        public void BuildParameterizedQuery_AddsParameterizedConditions_WithAppropiateParamNames
            (
            string columnName,
            string conditionalOperator,
            string? paramName = null
            )
        {
            // Arrange
            var expected = $" WHERE {columnName} {conditionalOperator} @{paramName ?? columnName};";
            var queryBuilder = GetQueryBuilder();
            queryBuilder.AddParameterizedCondition(columnName, conditionalOperator, new { }, paramName);

            // Act
            var actual = queryBuilder.BuildParameterizedQuery(string.Empty);

            // Assert
            Assert.That(actual.RawSql, Is.EqualTo(expected));
        }

        [TestCase(false, TestName = "AddParameterizedCondition_Succeeds_OnOverwrite_When_OverwriteIsTrue")]
        [TestCase(true, TestName = "AddParameterizedCondition_Throws_OnOverwrite_When_OverwriteIsFalse")]
        public void AddParameterizedCondition_HandlesOverwriteParam(bool allowOverwrite)
        {
            // Arrange
            var paramName = "ColName";
            var queryBuilder = GetQueryBuilder();
            queryBuilder.AddParameterizedCondition(paramName, ConditionalOperator.EQ, new { });

            // Act & Assert
            if (allowOverwrite)
            {
                Assert.DoesNotThrow(
                    () => queryBuilder.AddParameterizedCondition(paramName, "=>", new { }, overwriteParam: allowOverwrite));
            }
            else
            {
                Assert.Throws<ArgumentException>(
                    () => queryBuilder.AddParameterizedCondition(paramName, "=>", new { }, overwriteParam: allowOverwrite));
            }
        }

        [Test]
        public void BuildParameterizedQuery_WorksWithMultipleParameters()
        {
            // Arrange
            var where = "WHERE";
            var condition1 = new DynamicCondition(ConditionalOperator.EQ, "someValue", "column1");
            var condition2 = new DynamicCondition(ConditionalOperator.IN, new List<int>(), "column2");
            var condition3 = new DynamicCondition(ConditionalOperator.LTEQ, 64, "column3");
            var conditions = new List<DynamicCondition>
            {
                { condition1 },
                { condition2 },
                { condition3 },
            };


            var queryBuilder = GetQueryBuilder();
            var conditionDictionary = new Dictionary<string, DynamicCondition>();
            foreach (var condition in conditions)
            {
                conditionDictionary[$"{condition.ColumnName} {condition.ConditionalOperator} @{condition.ColumnName}"] = condition;
                queryBuilder.AddParameterizedCondition(condition.ColumnName, condition.ConditionalOperator, condition.Value);
            }

            // Act
            var query = queryBuilder.BuildParameterizedQuery(string.Empty);
            var whereConditions = query.RawSql
                .AsSpan()
                .Slice(query.RawSql.IndexOf(where) + where.Length)
                .Trim(';')
                .ToString()
                .Split("AND");

            // Assert
            Assert.That(conditions.Count, Is.EqualTo(whereConditions.Length));

            foreach (var condition in whereConditions)
            {
                var dynamicCondition = conditionDictionary[condition.Trim()];
                var queryParamValue = query.QueryParams[dynamicCondition.ColumnName];

                Assert.That(dynamicCondition.Value, Is.EqualTo(queryParamValue));
            }
        }
    }
}
