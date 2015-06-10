using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LifxHttp
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PowerState
    {
       [EnumMember(Value = "on")]
       On,
       [EnumMember(Value = "off")]
       Off
    }
}
