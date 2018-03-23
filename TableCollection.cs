using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace EntityFramework.DbValidator
{
    public class TableCollection
    {
        #region Private Stuff
        private IEnumerable<TableMetaData> Tables;
        private TableComparisonResult CompareTableTo(TableMetaData refTable)
        {
            var dbTable = Tables.Where(e => e.TableName == refTable.TableName).FirstOrDefault();
            if (dbTable == null) return new TableMessingResult(refTable);
            var result = dbTable.CompareColumns(refTable);
            if (result == null) return new TableMatchResult(refTable.TableName);
            return new ColumnsMessingResult(refTable.TableName, result);
        }
        #endregion
        public TableCollection(IEnumerable<TableMetaData> tables)
        {
            Tables = tables;
        }
        public List<TableComparisonResult> CompareWith(TableCollection refCollection, string[] tableNames)
        {
            var refTables = refCollection.Tables;
            if (tableNames.Count() != 0)
                refTables = refTables.Where(t => tableNames.Contains(t.TableName));
            return refTables.Select(CompareTableTo).ToList();
        }
    }
}
