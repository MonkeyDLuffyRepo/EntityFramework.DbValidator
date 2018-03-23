using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace EntityFramework.DbValidator
{
    public class TableMetaData
    {
        #region Private Stuff
        private ColumnMismatchResult CompareColumns(ColumnMetaData refColumn, ColumnMetaData dbColumn)
        {
            return (!dbColumn.Equals(refColumn)) ?
                 new ColumnMismatchResult(refColumn, TableName) : null;
        }
        private ColumnComparisonResult CheckColumn(ColumnMetaData refColumn)
        {
            var dbColumn = GetColumnMetaData(refColumn.ColumnName);
            if (dbColumn == null) return new ColumnMissingResult(refColumn, TableName);
            return CompareColumns(refColumn, dbColumn);                
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
            TableName = table.ToLower();
            ColumnMetadatas = columns;
        }
        public static TableMetaData FromEntityType(EntityType entityType)
        {
            var columns = entityType.Properties
                .Select(p => new ColumnMetaData(p.Name, p.TypeUsage.EdmType.Name, p.Nullable, p.MaxLength));
            return new TableMetaData(entityType.Name, columns.ToImmutableList());
        }

        public List<ColumnComparisonResult> CompareColumns(TableMetaData refTable)
        {
            var columnResults = refTable.ColumnMetadatas
                .Select(CheckColumn)
                .Where(result => result != null)
                .ToList();

            return columnResults.Count == 0 ? null : columnResults;
        }
    }
}