using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Script.Serialization;

namespace Nsfttz.Common.Serialize.SerializeConverter
{
    public class DataTableJavaScriptConverter : JavaScriptConverter
    {
        internal static readonly Type[] Types;

        static DataTableJavaScriptConverter()
        {
            Types = new[] { typeof(DataTable), typeof(DataSet) };
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            if (obj == null)
            {
                return null;
            }
            var type = obj.GetType();
            if (type == Types[0])
            {
                var dt = (DataTable)obj;
                return new Dictionary<string, object>
                {
                    {
                        string.IsNullOrWhiteSpace(dt.TableName) ? type.Name : dt.TableName,
                        dt.Select()
                            .Select(
                                dr =>
                                    dt.Columns.Cast<DataColumn>()
                                        .ToDictionary(dc => dc.ColumnName, dc => dr[dc.ColumnName]))
                    }
                };
            }
            if (type == Types[1])
            {
                var ds = (DataSet)obj;
                var tables = ds.Tables.Cast<DataTable>().ToArray();
                for (int i = 0; i < tables.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(tables[i].TableName))
                    {
                        tables[i].TableName = string.Format("{0}{1}", tables[i].GetType().Name, i);
                    }
                    if (tables.Select(dt => dt.TableName).Contains(tables[i].TableName))
                    {
                        tables[i].TableName = string.Format("{0}{1}", tables[i].TableName, i);
                    }
                }
                return new Dictionary<string, object>
                {
                    {
                        string.IsNullOrWhiteSpace(ds.DataSetName)?type.Name:ds.DataSetName,
                        tables
                    }
                };

            }
            return null;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return Types; }
        }
    }
}
