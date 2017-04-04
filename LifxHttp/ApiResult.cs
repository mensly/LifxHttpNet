using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxHttp
{
    /// <summary>
    /// Operation returned from setting multiple states. Contains selector and state properties.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class ApiOperation : LightState
    {
        [JsonProperty("selector")]
        string Selector;
    }

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

        [JsonProperty("operation")]
        public ApiOperation Operation { get; private set; }

        /// <summary>
        /// When setting multiple states, each result contains operation(s) and the results of each operation.
        /// </summary>
        [JsonProperty("results")]
        public List<ApiResult> Results { get; set; }

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
