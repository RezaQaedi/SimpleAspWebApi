using System.Data;
using System.Dynamic;

namespace BehvarTestProject
{
    public static class Helpers
    {
        public static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }

        public static List<string> GetTableColumn(DataTable schemaTable)
        {
            var Columns = new List<string>();
            foreach (DataRow col in schemaTable.Rows)
            {
                // Get ColumnName
                var cName = col.Field<string>("ColumnName");

                if (cName != null)
                    Columns.Add(cName);
            }

            return Columns;
        }
    }
}
