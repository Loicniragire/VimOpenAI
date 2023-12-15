using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualPaymentService.Data.DynamicQueries
{
    public abstract class DynamicQueryBuilderBase
    {
        private readonly IDictionary<string, DynamicCondition> _queryParams;
        private const string Where = " WHERE ";
        private const string And = " AND ";

        protected DynamicQueryBuilderBase()
        {
            _queryParams = new Dictionary<string, DynamicCondition>();
        }

        /// <summary>
        /// Used to build a parameterized sql query, along with parameters.
        /// </summary>
        /// <param name="baseQuery">
        /// The base query to append the query conditions to. 
        /// E.g. "SELECT * FROM SomeTable"
        /// </param>
        /// <param name="containsWhereClause">
        /// Bool that is used to determine if the WHERE clause has already been created, 
        /// or if it needs to be added.
        /// </param>
        /// <returns><see cref="ParameterizedSql"/></returns>
        public ParameterizedSql BuildParameterizedQuery(string baseQuery, bool containsWhereClause = false)
        {
            var queryParams = new Dictionary<string, object>();
            var sqlQueryBuilder = new StringBuilder(baseQuery.Trim());

            foreach (var (paramName, dynamicCondition) in _queryParams)
            {
                if (!containsWhereClause)
                {
                    sqlQueryBuilder.Append(Where);
                    containsWhereClause = true;
                }
                else
                {
                    sqlQueryBuilder.Append(And);
                }

                sqlQueryBuilder.Append($"{dynamicCondition.ColumnName} {dynamicCondition.ConditionalOperator} @{paramName}");
                queryParams[paramName] = dynamicCondition.Value;
            }

            sqlQueryBuilder.Append(';');

            return new ParameterizedSql(sqlQueryBuilder.ToString(), queryParams);
        }

        /// <summary>
        /// Adds a parameterized condition to the dynamic query.
        /// </summary>
        /// <param name="columnName">The name of the column the query should base this condition on.</param>
        /// <param name="conditionalOperator">The SQL conditional operator to use. E.g. >=, IN, =, etc...</param>
        /// <param name="value">The value of the query condition.</param>
        /// <param name="paramName">
        /// Optional. 
        /// Can be used to specify the parameterized name within the dynamic query 
        /// to prevent parameter naming conflicts if the same <paramref name="columnName"/> 
        /// is being used for multiple conditions. 
        /// Default: <paramref name="columnName"/>.
        /// </param>
        /// <param name="overwriteParam">
        /// Optional.
        /// Can be used to overwrite the existing parameter value for a previously set <paramref name="paramName"/>
        /// Default: false.
        /// </param>
        protected void AddParameterizedCondition(
            string columnName,
            string conditionalOperator,
            object value,
            string paramName = null,
            bool overwriteParam = false)
        {
            paramName ??= columnName;

            if (_queryParams.ContainsKey(paramName) && !overwriteParam)
            {
                throw new ArgumentException(
                    $"A parameter with the name of {paramName} already exists. If you would like to overwrite the existing value, please set {nameof(overwriteParam)} to true",
                    nameof(paramName));
            }

            _queryParams[paramName] = new DynamicCondition(conditionalOperator, value, columnName);
        }
    }

    public class DynamicCondition
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditionalOperator"><see cref="ConditionalOperator"/></param>
        /// <param name="value"><see cref="Value"/></param>
        /// <param name="columnName"><see cref="ColumnName"/></param>
        public DynamicCondition(string conditionalOperator, object value, string columnName)
        {
            ConditionalOperator = conditionalOperator;
            Value = value;
            ColumnName = columnName;
        }

        /// <summary>
        /// The SQL conditional operator to use. E.g. >=, IN, =, etc...
        /// </summary>
        public string ConditionalOperator { get; init; }

        /// <summary>
        /// The value of the query condition.
        /// </summary>
        public object Value { get; init; }

        /// <summary>
        /// Used to specify the column used within the dynamic query.
        /// </summary>
        public string ColumnName { get; init; }
    }

    public class ParameterizedSql
    {
        public ParameterizedSql(string rawSql, IDictionary<string, object> queryParams)
        {
            RawSql = rawSql;
            QueryParams = queryParams;
        }

        /// <summary>
        /// The raw parameterized SQL string.
        /// E.g. SELECT * FROM SomeTable WHERE MyColumn = @MyColumn
        /// </summary>
        public string RawSql { get; set; }

        /// <summary>
        /// A <see cref="IDictionary{TKey, TValue}" /> of the query params.
        /// </summary>
        public IDictionary<string, object> QueryParams { get; set; }
    }
}
