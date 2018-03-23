using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.DbValidator
{
    public class TableCollection
    {
        public TableCollection(IEnumerable<TableMetaData> tables)
        {
            Tables = tables;
        }

        public IEnumerable<TableMetaData> Tables { get; set; }

        public TableComparisonResult CompareTableTo(TableMetaData refTable)
        {
            var dbTable = Tables.Where(e => e.TableName == refTable.TableName).FirstOrDefault();
            if (dbTable != null)
            {
                var result = dbTable.CompareColumns(refTable);
                if (result == null) return new TableMatchResult(refTable.TableName);
                return new ColumnsMessingResult(refTable.TableName, result); ;
            }
            else
                return new TableMessingResult(refTable);
        }
    }
}
