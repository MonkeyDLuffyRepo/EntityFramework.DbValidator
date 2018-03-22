namespace EntityFramework.DbValidator.Helpers
{
    public class DataTypes
    {
        public static bool IsNvarchar(string datatype)
        {
            return datatype == "nvarchar";
        }
    }
}
