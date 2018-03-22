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
            return ColumnMetadatas.Where(c => c.ColumnName.ToLower() == columnName.ToLower()).FirstOrDefault();
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
            var columns = entityType.Properties
                .Select(p => new ColumnMetaData(p.Name, p.TypeUsage.EdmType.Name, p.Nullable, p.MaxLength));
            return new TableMetaData(entityType.Name, columns.ToImmutableList());
        }

        public ColumnsMessingResult CheckForMessingColumns(TableMetaData refTable)
        {
            var columnResults = refTable.ColumnMetadatas
                .Select(CheckColumn)
                .Where(result => result != null)
                .ToList();

            return columnResults.Count == 0 ? null :
                new ColumnsMessingResult(refTable.TableName, columnResults.ToImmutableList());
        }
    }
}