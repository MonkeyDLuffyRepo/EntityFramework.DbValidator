using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EntityFramework.DbValidator
{

    public class MessingColumnsResult : ITableComparisonResult
    {
        public MessingColumnsResult(string tableName)
        {
            TableName = tableName;
            ColumnComparisonResults = new List<IColumnComparisonResult>();
        }
        public string TableName { get; set; }
        public List<IColumnComparisonResult> ColumnComparisonResults { get; set; }

        public string[] GetFix()
        {
            return  ColumnComparisonResults.Select(c => c.GetFix()).ToArray();
        }
    }

    public class MessingTableResult : ITableComparisonResult
    {
        public TableData Table { get; set; }
        public string TableName { get => Table.TableName; }

        public string[] GetFix()
        {
            var columns = string.Join(", ", Table.Columns.Select(c => c.ColumnName + " " + c.DataType + " " + (c.IsNullable ? "NULL" : "NOT NULL")));
            return new string[] { $"CREATE TABLE {Table.TableName} ({columns});" };
        }
    }
}