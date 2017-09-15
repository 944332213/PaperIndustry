using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Nsfttz.Common.Serialize.SerializeConverter
{
    public static class JavaScriptConverterKeymaker
    {
        public static IEnumerable<JavaScriptConverter> Get()
        {
            return new JavaScriptConverter[] { new DataTableJavaScriptConverter() };
            //return new JavaScriptConverter[] { new DataTableJavaScriptConverter(), new DictionaryJavaScriptConverter() };
        }
    }
}
