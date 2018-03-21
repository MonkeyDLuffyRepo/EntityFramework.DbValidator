using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EntityFramework.DbValidator
{
    public class TableData
    {
        public string TableName { get; set; }
        public List<ColumnData> Columns { get; set; }
    }
}