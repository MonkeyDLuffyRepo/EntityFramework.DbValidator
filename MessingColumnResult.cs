namespace EntityFramework.DbValidator
{
    public class ColumnMismatchResult : IColumnComparisonResult
    {
        public string Description = "ColumnMismatch";
        public string TableName { get; set; }
        public ColumnMetaData Column { get; set; }

        public string GetFix()
        {
            var datatype = Column.DataType + ((DataTypes.IsNvarchar(Column.DataType) ? $"({Column.CharacterMaximumLength})" : ""));
            return $"ALTER TABLE {TableName} ALTER COLUMN {Column.ColumnName} {datatype} {(Column.IsNullable ? "NULL" : "NOT NULL")}; ";
        }
    }

    public class MessingColumnResult : IColumnComparisonResult
    {
        public string TableName { get; set; }
        public ColumnMetaData Column { get; set; }

        public string GetFix()
        {
            var datatype = Column.DataType + ((DataTypes.IsNvarchar(Column.DataType) ? $"({Column.CharacterMaximumLength})" : ""));
            return $"ALTER TABLE {TableName} ADD {Column.ColumnName} {datatype} {(Column.IsNullable ? "NULL" : "NOT NULL")}; ";
        }
    }


}