using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Entity.Core.Metadata.Edm;
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

        public readonly string TableName;
        public ImmutableList<ColumnMetaData> ColumnMetadatas;

        public TableMetaData(string table, ImmutableList<ColumnMetaData> columns)
        {
            TableName = table;
            ColumnMetadatas = columns;
        }

        public static TableMetaData FromEntityType(EntityType entityType)
        {
            var columns = entityType.Properties.Select(p =>
                new ColumnMetaData
                {
                    ColumnName = p.Name,
                    DataType = p.TypeUsage.EdmType.Name,
                    IsNullable = p.Nullable,
                    CharacterMaximumLength = p.MaxLength
                });
            return new TableMetaData(entityType.Name, columns.ToImmutableList());
        }

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