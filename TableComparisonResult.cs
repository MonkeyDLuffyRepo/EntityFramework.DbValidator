using System;
using System.Collections.Generic;
using System.Linq;

namespace EntityFramework.DbValidator
{
    public class TableComparisonResult
    {
        public Lazy<string[]> UpgradeScript;
        public string TableName;
    }

    public class MessingColumnsResult : TableComparisonResult
    {
        #region Private Stuff
        private string[] GetUpgradeScript()
        {
            return ColumnComparisonResults.Select(c => c.UpgradeScript.Value).ToArray();
        }
        #endregion

        public MessingColumnsResult(string tableName)
        {
            TableName = tableName;
            ColumnComparisonResults = new List<ColumnComparisonResult>();
            UpgradeScript = new Lazy<string[]>(GetUpgradeScript);
        }
        public List<ColumnComparisonResult> ColumnComparisonResults { get; set; }
    }

    public class MessingTableResult : TableComparisonResult
    {
        public TableMetaData Table { get; set; }
        public MessingTableResult()
        {
            UpgradeScript = new Lazy<string[]>(GetUpgradeScript);
        }
        #region Private Stuff
        private string[] GetUpgradeScript()
        {
            var columns = string.Join(", ", Table.ColumnMetadatas.Select(c => c.ColumnName + " " + c.DataType + " " + (c.IsNullable ? "NULL" : "NOT NULL")));
            return new string[] { $"CREATE TABLE {Table.TableName} ({columns});" };
        } 
        #endregion
    }
}