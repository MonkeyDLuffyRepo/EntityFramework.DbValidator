using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
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
            var tables = ObjectContext.MetadataWorkspace.GetItems(DataSpace.SSpace)
                .Where(m => m.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                .Cast<EntityType>()
                .Select(TableMetaData.FromEntityType);
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
                .Select(gr => new TableMetaData(gr.Key,
                    gr.Select(r =>
                        new ColumnMetaData(r.ColumnName, r.DataType, r.IsNullable == "YES", r.CharacterMaximumLength)
                    ).ToList().ToImmutableList())
                );
            return new TableCollection(dbTables.ToList());
        }
        #endregion

        public DbValidator(DbContext context)
        {
            ObjectContext = ((IObjectContextAdapter)context).ObjectContext;
            DatabaseName = context.Database.Connection.Database;
        }
        public List<TableComparisonResult> Validate(string[] includedTables = null)
        {
            var storageModelTables = GetStorageModelTables();
            var dbTables = GetDbTables();
            return dbTables.CompareWith(storageModelTables, includedTables);
        }
        public string GetUpgradeSqlScript(string[] includedTables = null, string delimiter = null)
        {
            var results = Validate(includedTables).SelectMany(r => r.UpgradeScript.Value);
            return string.Join(delimiter, results);
        }
    }
}