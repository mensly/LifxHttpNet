using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxHttp
{
    /// <summary>
    /// Returned from API actions
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class ApiResult
    {
        internal static bool Successful(ApiResult result)
        {
            return result.IsSuccessful;
        }

        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("label")]
        public string Label { get; private set; }

        [JsonProperty("status")]
        public string Status { get; private set; }
        
        public bool IsSuccessful { get { return Status == "ok"; } }
        
        public bool IsTimedOut { get { return Status == "timed_out"; } }

        public ApiResult()
        {
            Status = "unknown";
        }
    }
}
