using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxHttp
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class ApiResults
    {
        [JsonProperty("results")]
        public List<ApiResult> Results { get; set; }
    }
}
