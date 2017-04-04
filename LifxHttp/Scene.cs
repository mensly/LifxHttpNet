using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxHttp
{
    [JsonObject(MemberSerialization.OptIn)]
    public struct AccountSpec
    {
        [JsonProperty("uuid")]
        public string UUID { get; private set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Scene
    {
        [JsonProperty("uuid")]
        public string UUID { get; private set; }
        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonProperty("account")]
        public AccountSpec Account { get; private set; }
        [JsonProperty("states")]
        public List<LightState> States { get; set; }
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder(string.Format("UUID: {0} Name:{1} Account:{2} Created At:{3} Updated At:{4}", UUID, Name, Account.UUID, CreatedAt, UpdatedAt));
            result.AppendLine();
            result.AppendLine("States: ");
            foreach (var state in States)
            {
                result.AppendLine(state.ToString());
            }
            return result.ToString();
        }
    }


}
