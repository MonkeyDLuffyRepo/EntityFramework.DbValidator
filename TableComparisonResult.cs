using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace EntityFramework.DbValidator
{
    public class TableComparisonResult
    {
        public Lazy<string[]> UpgradeScript;
        public string TableName;
    }

    public class ColumnsMessingResult : TableComparisonResult
    {
        #region Private Stuff
        private string[] GetUpgradeScript()
        {
            return ColumnComparisonResults.Select(c => c.UpgradeScript.Value).ToArray();
        }
        #endregion

        public ColumnsMessingResult(string tableName, ImmutableList<ColumnComparisonResult> columnComparisonResults)
        {
            TableName = tableName;
            ColumnComparisonResults = columnComparisonResults;
            UpgradeScript = new Lazy<string[]>(GetUpgradeScript);
        }
        public readonly ImmutableList<ColumnComparisonResult> ColumnComparisonResults;
    }

    public class TableMessingResult : TableComparisonResult
    {
        #region Private Stuff
        private string[] GetUpgradeScript()
        {
            var columns = string.Join(", ", Table.ColumnMetadatas.Select(c => c.ColumnName + " " + c.DataType + " " + (c.IsNullable ? "NULL" : "NOT NULL")));
            return new string[] { $"CREATE TABLE {Table.TableName} ({columns});" };
        }
        #endregion
        public readonly TableMetaData Table;
        public TableMessingResult(TableMetaData table)
        {
            Table = table;
            TableName = table.TableName;
            UpgradeScript = new Lazy<string[]>(GetUpgradeScript);
        }
    }

    public class TableMatchResult : TableComparisonResult {
        public TableMatchResult(string tableName)
        {
            TableName = tableName;
            UpgradeScript = new Lazy<string[]>(() => new string[] { });
        }
    }
}