using System;

namespace EntityFramework.DbValidator
{
    public class ColumnComparisonResult
    {
        public Lazy<string> UpgradeScript { get; set; }
        public string TableName { get; set; }
        public ColumnMetaData Column { get; set; }
    }

    public class ColumnMismatchResult : ColumnComparisonResult
    {
        #region Private Stuff
        private string GetUpgradeScript()
        {
            var datatype = Column.DataType + ((Helpers.DataTypes.IsNvarchar(Column.DataType) ? $"({Column.CharacterMaximumLength})" : ""));
            return $"ALTER TABLE {TableName} ALTER COLUMN {Column.ColumnName} {datatype} {(Column.IsNullable ? "NULL" : "NOT NULL")}; ";
        }
        #endregion
        public ColumnMismatchResult()
        {
            UpgradeScript = new Lazy<string>(GetUpgradeScript);
        }
    }

    public class MessingColumnResult : ColumnComparisonResult
    {
        #region Private Stuff
        private string GetUpgradeScript()
        {
            var datatype = Column.DataType + ((Helpers.DataTypes.IsNvarchar(Column.DataType) ? $"({Column.CharacterMaximumLength})" : ""));
            return $"ALTER TABLE {TableName} ADD {Column.ColumnName} {datatype} {(Column.IsNullable ? "NULL" : "NOT NULL")}; ";
        }
        #endregion
        public MessingColumnResult()
        {
            UpgradeScript = new Lazy<string>(GetUpgradeScript);
        }
    }
}