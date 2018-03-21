using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace EntityFramework.DbValidator
{
    public class DbValidator
    {
        #region Private Stuff
        private ObjectContext objectContext;
        private class Result
        {
            public string TableName { get; set; }
            public string ColumnName { get; set; }
            public string IsNullable { get; set; }
            public string DataType { get; set; }
        }
        private IEnumerable<TableMetaData> GetStorageModelTables()
        {
            return (from meta in objectContext.MetadataWorkspace.GetItems(DataSpace.SSpace)
                                     .Where(m => m.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                    let properties = meta is EntityType ? (meta as EntityType).Properties : null
                    select new TableMetaData
                    {
                        TableName = (meta as EntityType).Name,
                        ColumnMetadatas = (from p in properties
                                   select new ColumnMetaData
                                   {
                                       ColumnName = p.Name,
                                       DataType = p.TypeUsage.EdmType.Name,
                                       IsNullable = p.Nullable
                                   }).ToList()
                    }).ToList();
        }
        private IEnumerable<TableMetaData> GetDbTables()
        {
            var sql = $@"SELECT COLUMNS.TABLE_NAME as TableName, COLUMNS.COLUMN_NAME as ColumnName , COLUMNS.IS_NULLABLE as IsNullable, COLUMNS.DATA_TYPE as DataType 
FROM INFORMATION_SCHEMA.TABLES AS TABLES 
LEFT OUTER JOIN INFORMATION_SCHEMA.COLUMNS AS COLUMNS 
ON TABLES.TABLE_NAME = COLUMNS.TABLE_NAME 
WHERE (TABLES.TABLE_TYPE = 'BASE TABLE') AND (TABLES.TABLE_CATALOG = N'{objectContext.Connection.Database}') AND (TABLES.TABLE_SCHEMA = N'dbo')";
            var dbTables = objectContext.ExecuteStoreQuery<Result>(sql).ToList().GroupBy(r => r.TableName)
                .Select(gr => new TableMetaData
                {
                    TableName = gr.Key,
                    ColumnMetadatas = gr.Select(r =>
                    new ColumnMetaData
                    {
                        ColumnName = r.ColumnName,
                        IsNullable = r.IsNullable == "YES",
                        DataType = r.DataType
                    }).ToList()
                });
            return dbTables;
        }
        private static IColumnComparisonResult CheckColumn(string tableName, TableMetaData dbTable, ColumnMetaData storageModelColumn)
        {
            var dbColumn = dbTable.ColumnMetadatas.Where(c => c.ColumnName == storageModelColumn.ColumnName).FirstOrDefault();
            if (dbColumn != null)
            {
                if (!dbColumn.Equals(storageModelColumn))
                {
                    return new ColumnMismatchResult { Column = storageModelColumn, TableName = tableName };
                }
                return null;
            }
            else
            {
                return new MessingColumnResult { TableName = tableName, Column = storageModelColumn };
            }
        }
        private static ITableComparisonResult CheckTable(IEnumerable<TableMetaData> dbTables, TableMetaData storageModelTable)
        {
            var dbTable = dbTables.Where(e => e.TableName == storageModelTable.TableName).FirstOrDefault();
            if (dbTable != null)
            {
                var tableCheckResult = new MessingColumnsResult(storageModelTable.TableName);
                foreach (var ctxColumn in storageModelTable.ColumnMetadatas)
                {
                    var result = CheckColumn(storageModelTable.TableName, dbTable, ctxColumn);
                    if (result != null)
                        tableCheckResult.ColumnComparisonResults.Add(result);
                }
                return tableCheckResult;
            }
            else
            {
                return new MessingTableResult { Table = storageModelTable };
            }
        }
        #endregion

        public DbValidator(ObjectContext objectContext)
        {
            this.objectContext = objectContext;
        }

        public List<ITableComparisonResult> Validate()
        {
            var ctxTables = GetStorageModelTables();
            var dbTables = GetDbTables();
            return ctxTables
                .Select(ctxTable => CheckTable(dbTables, ctxTable))
                .ToList();
        }
    }
}