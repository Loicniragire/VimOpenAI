using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace VirtualPaymentService.Model.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// Converts IEnumerable to DataTable, supports null values.
        /// </summary>
        /// <returns><see cref="DataTable"/></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> elements)
        {
            var dataTable = new DataTable();

            if (elements == null) return dataTable;

            PropertyInfo[] columns = null;

            foreach (T element in elements)
            {
                // Use reflection to get property names to create table, only executes on first row.
                if (columns == null)
                {
                    columns = element.GetType().GetProperties();
                    foreach (PropertyInfo column in columns)
                    {
                        Type colType = column.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dataTable.Columns.Add(new DataColumn(column.Name, colType));
                    }
                }

                DataRow dataRow = dataTable.NewRow();

                foreach (PropertyInfo column in columns)
                {
                    dataRow[column.Name] = column.GetValue(element, null) ?? DBNull.Value;
                }

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }
    }
}
