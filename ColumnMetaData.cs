namespace EntityFramework.DbValidator
{
    public class ColumnMetaData
    {
        public readonly string DataType;
        public readonly string ColumnName;
        public readonly bool IsNullable;
        public readonly int? CharacterMaximumLength;

        public ColumnMetaData(string columnName, string dataType, bool nullable, int? maxLength)
        {
            ColumnName = columnName;
            DataType = dataType;
            IsNullable = nullable;
            CharacterMaximumLength = maxLength;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            ColumnMetaData p = (ColumnMetaData)obj;
            if (p.DataType.StartsWith("varbinary"))
            {
                return p.IsNullable == IsNullable &&
                p.ColumnName == ColumnName &&
                DataType.StartsWith("varbinary");
            }

            return p.IsNullable == IsNullable &&
                p.ColumnName == ColumnName &&
                p.DataType == DataType &&
                p.CharacterMaximumLength == CharacterMaximumLength;
        }
        public override int GetHashCode()
        {
            if (DataType.StartsWith("varbinary"))
            {
                return IsNullable.GetHashCode() ^
                ColumnName.GetHashCode() ^
                DataType.StartsWith("varbinary").GetHashCode();
            }

            return IsNullable.GetHashCode() ^
                ColumnName.GetHashCode() ^
                DataType.GetHashCode() ^
                CharacterMaximumLength.GetHashCode();
        }
    }
}