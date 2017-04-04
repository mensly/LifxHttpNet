using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LifxHttp.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Direction
    {
       [EnumMember(Value = "forward")]
       Forward,
       [EnumMember(Value = "backward")]
       Backward
    }
}
