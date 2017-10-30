using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nsfttz.Common.Serialize.JsonConvert
{
    /// <summary>
    /// 限制属性分析器
    /// </summary>
    public class LimitPropertyContractResolver : DefaultContractResolver
    {
        public LimitPropertyContractResolver(PermitImplement<string> stringLimit)
        {
            StringLimit = stringLimit;
        }

        public PermitImplement<string> StringLimit { get; set; }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return
                base.CreateProperties(type, memberSerialization)
                    .Where(item => StringLimit.Permit(item.PropertyName))
                    .ToList();
        }
    }
}