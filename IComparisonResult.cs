using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EntityFramework.DbValidator
{
    public interface IComparisonResult {
        
    }
    public interface IColumnComparisonResult : IComparisonResult {
        string GetFix();
    }
    public interface ITableComparisonResult : IComparisonResult {
        string[] GetFix();
        string TableName { get; }
    }
}