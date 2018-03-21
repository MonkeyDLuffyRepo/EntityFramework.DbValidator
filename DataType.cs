using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.DbValidator
{
    public class DataTypes
    {
        public static bool IsNvarchar(string datatype)
        {
            return datatype == "nvarchar";
        }
    }
}
