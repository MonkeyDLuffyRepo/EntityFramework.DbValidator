using System.Collections.Generic;
using System.Linq;

namespace EntityFramework.DbValidator
{
    public class TableMetaData
    {
        #region Private Stuff
        private ColumnMismatchResult CompareColumns(ColumnMetaData storageModelColumn, ColumnMetaData dbColumn)
        {
            return (!dbColumn.Equals(storageModelColumn)) ?
                 new ColumnMismatchResult(storageModelColumn, TableName) : null;
        }
        private ColumnComparisonResult CheckColumn(ColumnMetaData storageModelColumn)
        {
            var dbColumn = GetColumnMetaData(storageModelColumn.ColumnName);
            if (dbColumn != null)
                return CompareColumns(storageModelColumn, dbColumn);
            else
                return new ColumnMessingResult(storageModelColumn, TableName);
        }
        private ColumnMetaData GetColumnMetaData(string columnName)
        {
            return ColumnMetadatas.Where(c => c.ColumnName == columnName).FirstOrDefault();
        }
        #endregion

        public string TableName { get; set; }
        public List<ColumnMetaData> ColumnMetadatas { get; set; }

        public ColumnsMessingResult CheckForMessingColumns(TableMetaData refTable)
        {
            var tableCheckResult = new ColumnsMessingResult(refTable.TableName);
            foreach (var storageModelColumn in refTable.ColumnMetadatas)
            {
                var result = CheckColumn(storageModelColumn);
                if (result != null)
                    tableCheckResult.ColumnComparisonResults.Add(result);
            }
            return tableCheckResult.ColumnComparisonResults.Count == 0 ? null : tableCheckResult;
        }
    }
}