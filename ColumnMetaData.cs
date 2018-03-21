using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EntityFramework.DbValidator
{
    public class ColumnMetaData
    {
        #region Private Stuff
        private string columnName;
        #endregion
        public string DataType { get; set; }
        public string ColumnName { get { return columnName; } set { columnName = value.ToLower(); } }
        public bool IsNullable { get; set; }
        public int? CharacterMaximumLength { get; set; }

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