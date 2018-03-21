using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;

namespace EntityFramework.DbValidator
{
    public class Comparer
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
        private IEnumerable<TableData> GetContextTables()
        {
            return (from meta in objectContext.MetadataWorkspace.GetItems(DataSpace.SSpace)
                                     .Where(m => m.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                    let properties = meta is EntityType ? (meta as EntityType).Properties : null
                    select new TableData
                    {
                        TableName = (meta as EntityType).Name,
                        Columns = (from p in properties
                                   select new ColumnData
                                   {
                                       ColumnName = p.Name,
                                       DataType = p.TypeUsage.EdmType.Name,
                                       IsNullable = p.Nullable
                                   }).ToList()
                    }).ToList();
        }
        private IEnumerable<TableData> GetDbTables()
        {
            var sql = @"SELECT COLUMNS.TABLE_NAME as TableName, COLUMNS.COLUMN_NAME as ColumnName , COLUMNS.IS_NULLABLE as IsNullable, COLUMNS.DATA_TYPE as DataType 
FROM INFORMATION_SCHEMA.TABLES AS TABLES 
LEFT OUTER JOIN INFORMATION_SCHEMA.COLUMNS AS COLUMNS 
ON TABLES.TABLE_NAME = COLUMNS.TABLE_NAME 
WHERE (TABLES.TABLE_TYPE = 'BASE TABLE') AND (TABLES.TABLE_CATALOG = N'Saham') AND (TABLES.TABLE_SCHEMA = N'dbo')";
            var dbTables = objectContext.ExecuteStoreQuery<Result>(sql).ToList().GroupBy(r => r.TableName)
                .Select(gr => new TableData
                {
                    TableName = gr.Key,
                    Columns = gr.Select(r =>
                    new ColumnData
                    {
                        ColumnName = r.ColumnName,
                        IsNullable = r.IsNullable == "YES",
                        DataType = r.DataType
                    }).ToList()
                });
            return dbTables;
        }
        private static IColumnComparisonResult CheckColumn(string tableName, TableData dbTable, ColumnData ctxColumn)
        {
            var dbColumn = dbTable.Columns.Where(d => d.ColumnName == ctxColumn.ColumnName).FirstOrDefault();
            if (dbColumn != null)
            {
                if (!dbColumn.Equals(ctxColumn))
                {
                    return new ColumnMismatchResult { Column = ctxColumn, TableName = tableName };
                }
                return null;
            }
            else
            {
                return new MessingColumnResult { TableName = tableName, Column = ctxColumn };
            }
        }
        private static ITableComparisonResult CheckTable(IEnumerable<TableData> dbTables, TableData ctxTable)
        {
            var dbTable = dbTables.Where(e => e.TableName == ctxTable.TableName).FirstOrDefault();
            if (dbTable != null)
            {
                var tableCheckResult = new MessingColumnsResult(ctxTable.TableName);
                foreach (var ctxColumn in ctxTable.Columns)
                {
                    var result = CheckColumn(ctxTable.TableName, dbTable, ctxColumn);
                    if (result != null)
                        tableCheckResult.ColumnComparisonResults.Add(result);
                }
                return tableCheckResult;
            }
            else
            {
                return new MessingTableResult { Table = ctxTable };
            }
        }
        #endregion

        public Comparer(ObjectContext objectContext)
        {
            this.objectContext = objectContext;
        }

        public List<ITableComparisonResult> Compare()
        {
            var ctxTables = GetContextTables();
            var dbTables = GetDbTables();
            return ctxTables
                .Select(ctxTable => CheckTable(dbTables, ctxTable))
                .ToList();
        }
    }
}