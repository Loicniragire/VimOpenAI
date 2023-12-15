using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using VirtualPaymentService.Model.Extensions;
using VirtualPaymentService.Model.Requests;

namespace VirtualPaymentService.Business.Tests.Extensions
{
    [TestFixture]
    public class IEnumerableToDataTableTests
    {
        #region IEnumerableToDataTable Tests
        [Test]
        public void IEnumerableToDataTable_WhenListIsConverted_ThenResultIsDataTable()
        {
            // Arrange
            var settlementTransactions = new List<SettlementTransaction>
            {
                new SettlementTransaction { LeaseId = 123, ProviderCardId = "CardId123" },
                new SettlementTransaction { LeaseId = 456, ProviderCardId = "CardId456" }
            };

            // Act
            var actual = settlementTransactions.ToDataTable();

            // Assert
            actual.Should().BeOfType<DataTable>();
        }

        [Test]
        public void IEnumerableToDataTable_WhenListIsConverted_ThenContentMatchesSource()
        {
            // Arrange
            var settlementTransactions = new List<SettlementTransaction>
            {
                new SettlementTransaction { LeaseId = 123, ProviderCardId = "CardId123", PostedDate = DateTime.Now, TransactionDate = DateTime.Now },
                new SettlementTransaction { LeaseId = 456, ProviderCardId = "CardId456", PostedDate = DateTime.Now, TransactionDate = DateTime.Now }
            };

            // Act
            var actual = settlementTransactions.ToDataTable();

            // Assert
            actual.Rows.Count.Should().Be(2);
            actual.Columns.Count.Should().Be(typeof(SettlementTransaction).GetProperties().Length);
            actual.Rows[0]["LeaseId"].Should().Be(settlementTransactions[0].LeaseId);
            actual.Rows[0]["ProviderCardId"].Should().Be(settlementTransactions[0].ProviderCardId);
            actual.Rows[0]["PostedDate"].Should().Be(settlementTransactions[0].PostedDate);
            actual.Rows[0]["TransactionDate"].Should().Be(settlementTransactions[0].TransactionDate);
            actual.Rows[0]["StoreId"].Should().Be(DBNull.Value);
            actual.Rows[0]["ProviderTransactionIdentifier"].Should().Be(DBNull.Value);
            actual.Rows[0]["TransactionAmount"].Should().Be(DBNull.Value);
            actual.Rows[0]["TransactionType"].Should().Be(DBNull.Value);
        }

        [Test]
        public void IEnumerableToDataTable_WhenListIsNull_ThenResultIsEmptyDataTable()
        {
            // Arrange
            List<int> emptyList = null;

            // Act
            var actual = emptyList.ToDataTable();

            // Assert
            actual.Should().BeOfType<DataTable>();
            actual.Rows.Count.Should().Be(0);
            actual.Columns.Count.Should().Be(0);
        }
        #endregion IEnumerableToDataTable Tests
    }
}
