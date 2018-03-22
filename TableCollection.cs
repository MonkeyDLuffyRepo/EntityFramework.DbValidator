using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.DbValidator
{
    public class TableCollection
    {
        public TableCollection(List<TableMetaData> tables)
        {
            Tables = tables;
        }

        public List<TableMetaData> Tables { get; set; }

        public TableComparisonResult CheckForTable(TableMetaData refTable)
        {
            var dbTable = Tables.Where(e => e.TableName == refTable.TableName).FirstOrDefault();
            if (dbTable != null)
            {
                var result = dbTable.CheckForMessingColumns(refTable);
                if (result == null) return new TableMatchResult(refTable.TableName);
                return result;
            }
            else
                return new TableMessingResult(refTable);
        }
    }
}
