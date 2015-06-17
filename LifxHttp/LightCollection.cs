using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxHttp
{
    public abstract class LightCollection : IEnumerable<Light>, ILightTarget<List<ApiResult>>
    {
        public string Id { get; private set; }
        public string Label { get; private set; }

        public bool IsOn { get { return lights.Any(l => l.IsOn); } }

        private List<Light> lights;
        private LifxClient client;

        internal LightCollection(LifxClient client, string id, string label, List<Light> lights)
        {
            this.client = client;
            Id = id;
            Label = label;
            this.lights = lights;
        }

        public IEnumerator<Light> GetEnumerator()
        {
            return lights.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return lights.GetEnumerator();
        }
        public async Task<List<ApiResult>> TogglePower()
        {
            if (client == null) { return new List<ApiResult>(); }
            return await client.TogglePower(this);
        }

        public async Task<List<ApiResult>> SetPower(bool powerState, float duration = LifxClient.DEFAULT_DURATION)
        {
            if (client == null) { return new List<ApiResult>(); }
            return await client.SetPower(this, powerState, duration);
        }

        public async Task<List<ApiResult>> SetColor(LifxColor color, float duration = LifxClient.DEFAULT_DURATION, bool powerOn = LifxClient.DEFAULT_POWER_ON)
        {
            if (client == null) { return new List<ApiResult>(); }
            return await client.SetColor(this, color, duration, powerOn);
        }

        public async Task Refresh()
        {
            if (client != null)
            {
                lights = await client.ListLights(this);
            }
        }

        public abstract Selector ToSelector();

        public override string ToString()
        {
            return Label;
        }

        public static implicit operator Selector(LightCollection lightCollection)
        {
            return lightCollection.ToSelector();
        }
    }
}
