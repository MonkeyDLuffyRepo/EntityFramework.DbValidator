# EntityFramework.DbValidator
This a library created to check if the tables defined in an EDM schema are still valid in the live SQL server database and generate an upgrade script for the database if there are a missing fields or missing tables.

Database First allows you to reverse engineer a model from an existing database. The model is stored in an EDMX file. The database your application will use may have a different schema : some tables, fileds are deleted, other fileds have changed [datatype, nullable, max length...]. 

Running your application with such a database, you will run into a lot of problems. This library could help you easily detect the changes between your edmx file and your database, generate a SQL script to update the database and writing tests.

## Usage
The following example shows how to generate database update script, which is a SQL script that makes the model in EDMX file and the database in sync : 

```
using EntityFramework.DbValidator;
...
using (var ctx = new MyContext())
{
    var dbValidator = new DbValidator(ctx);
    var dbUpdateScript = dbValidator.GetUpgradeSqlScript();
    System.Console.WriteLine(dbUpdateScript);

    //launch the script
    if (!string.IsNullOrEmpty(dbUpdateScript))
    {
        ctx.Database.ExecuteSqlCommand(dbUpdateScript);
    }
}
```
Another usage of the library is to write an integration test
```
[Test]
public void ModelDatabaseTest()
{
    using (var ctx = new MyContext())
    {
        var dbValidator = new DbValidator(ctx);
        var dbValidationResults = dbValidator.Validate();
        Assert.AreEqual(0,  dbValidationResults.Count);
    }
}

```
