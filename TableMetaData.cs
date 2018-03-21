using System.Collections.Generic;

namespace EntityFramework.DbValidator
{
    public class TableMetaData
    {
        public string TableName { get; set; }
        public List<ColumnMetaData> ColumnMetadatas { get; set; }
    }
}