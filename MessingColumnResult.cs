using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EntityFramework.DbValidator
{

    public class ColumnMismatchResult : IColumnComparisonResult
    {
        public string Description = "ColumnMismatch";
        public string TableName { get; set; }
        public ColumnMetaData Column { get; set; }

        public string GetFix()
        {
            return $"ALTER TABLE {TableName} ALTER COLUMN {Column.ColumnName} {Column.DataType} {(Column.IsNullable ? "NULL" : "NOT NULL")}; ";
        }
    }

    public class MessingColumnResult : IColumnComparisonResult
    {
        public string TableName { get; set; }
        public ColumnMetaData Column { get; set; }

        public string GetFix()
        {
            return $"ALTER TABLE {TableName} ADD {Column.ColumnName} {Column.DataType} {(Column.IsNullable ? "NULL" : "NOT NULL")}; ";
        }
    }


}