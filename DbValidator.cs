using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

namespace EntityFramework.DbValidator
{
    public class DbValidator
    {
        #region Private Stuff
        private readonly ObjectContext ObjectContext;
        private readonly string DatabaseName;
        private class SqlQueryResult
        {
            public string TableName { get; set; }
            public string ColumnName { get; set; }
            public string IsNullable { get; set; }
            public string DataType { get; set; }
            public int? CharacterMaximumLength { get; set; }
        }
        private TableCollection GetStorageModelTables()
        {
            var tables= (from meta in ObjectContext.MetadataWorkspace.GetItems(DataSpace.SSpace)
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
                                               IsNullable = p.Nullable,
                                               CharacterMaximumLength = p.MaxLength
                                           }).ToList()
                    }).ToList();
            return new TableCollection(tables);
        }
        private TableCollection GetDbTables()
        {
            var sql = $@"SELECT COLUMNS.TABLE_NAME as TableName, COLUMNS.COLUMN_NAME as ColumnName ,
COLUMNS.IS_NULLABLE as IsNullable, COLUMNS.DATA_TYPE as DataType, COLUMNS.CHARACTER_MAXIMUM_LENGTH as CharacterMaximumLength 
FROM INFORMATION_SCHEMA.TABLES AS TABLES 
LEFT OUTER JOIN INFORMATION_SCHEMA.COLUMNS AS COLUMNS 
ON TABLES.TABLE_NAME = COLUMNS.TABLE_NAME 
WHERE (TABLES.TABLE_TYPE = 'BASE TABLE') AND (TABLES.TABLE_CATALOG = N'{DatabaseName}') AND (TABLES.TABLE_SCHEMA = N'dbo')";
            var dbTables = ObjectContext.ExecuteStoreQuery<SqlQueryResult>(sql).ToList().GroupBy(r => r.TableName)
                .Select(gr => new TableMetaData
                {
                    TableName = gr.Key,
                    ColumnMetadatas = gr.Select(r =>
                    new ColumnMetaData
                    {
                        ColumnName = r.ColumnName,
                        IsNullable = r.IsNullable == "YES",
                        DataType = r.DataType,
                        CharacterMaximumLength = r.CharacterMaximumLength
                    }).ToList()
                });
            return new TableCollection(dbTables.ToList());
        }
        #endregion

        public DbValidator(DbContext context)
        {
            ObjectContext = ((IObjectContextAdapter)context).ObjectContext;
            DatabaseName = context.Database.Connection.Database;
        }
        public List<TableComparisonResult> Validate()
        {
            var storageModelTables = GetStorageModelTables();
            var dbTables = GetDbTables();
            return storageModelTables.Tables
                .Select(storageModelTable => dbTables.CheckForTable(storageModelTable))
                .ToList();
        }
        public string GetUpgradeSqlScript(string[] tableNames, string delimiter = null)
        {
            var results = Validate().Where(r => tableNames.Contains(r.TableName)).SelectMany(r => r.UpgradeScript.Value);
            return string.Join(delimiter, results);
        }
    }
}