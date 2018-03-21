using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EntityFramework.DbValidator
{
    public class ColumnData
    {
        public string DataType { get; set; }
        public string ColumnName { get; set; }
        public bool IsNullable { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            ColumnData p = (ColumnData)obj;
            return p.IsNullable == IsNullable && p.ColumnName == ColumnName && p.DataType == DataType;
        }

        public override int GetHashCode()
        {
            return IsNullable.GetHashCode() ^ ColumnName.GetHashCode() ^ DataType.GetHashCode();
        }
    }
}