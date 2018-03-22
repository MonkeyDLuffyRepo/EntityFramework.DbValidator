using System.Collections.Generic;
using System.Linq;

namespace EntityFramework.DbValidator
{
    public class TableMetaData
    {
        public string TableName { get; set; }
        public List<ColumnMetaData> ColumnMetadatas { get; set; }

        public ColumnMetaData GetColumnMetaData(string columnName)
        {
            return ColumnMetadatas.Where(c => c.ColumnName == columnName).FirstOrDefault();
        }
    }
}