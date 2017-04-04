using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LifxHttp.Enums;
using LifxHttp.Helpers;

namespace LifxHttp
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LightState
    {
        /// <summary>
        /// Selector used for Set States.
        /// </summary>
        [JsonProperty("selector", NullValueHandling = NullValueHandling.Ignore)]
        public Selector Selector { get; set; }
        [JsonProperty("power", NullValueHandling = NullValueHandling.Ignore)]
        public PowerState PowerState { get; set; }
        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        public LifxColor Color { get; set; }
        private double? _brightness;
        [JsonProperty("brightness", NullValueHandling = NullValueHandling.Ignore)]
        public double? Brightness { get { return _brightness; }
            set {
                if (value.HasValue)
                    _brightness = value < LifxClient.MIN_BRIGHTNESS ? LifxClient.MIN_BRIGHTNESS : value > LifxClient.MAX_BRIGHTNESS ? LifxClient.MAX_BRIGHTNESS : value;
                else _brightness = value;
            }
        }
        private double? _duration;
        [JsonProperty("duration", NullValueHandling = NullValueHandling.Ignore)]
        public double? Duration { get { return _duration; }
            set {
                if (value.HasValue)
                    _duration = value < LifxClient.MIN_DURATION ? LifxClient.MIN_DURATION : value > LifxClient.MAX_DURATION ? LifxClient.MAX_DURATION : value;
                else _duration = value;
            }
        }
        [JsonProperty("infrared", NullValueHandling = NullValueHandling.Ignore)]
        public double? Infrared { get; set; }

        public LightState(PowerState powerState, LifxColor color, double? brightness = null, double? duration = null, double? infrared = null)
        {
            PowerState = powerState;
            Color = color;
            Brightness = brightness;
            Duration = duration;
            Infrared = infrared;
        }

        public LightState(Selector selector, PowerState powerState, LifxColor color, double? brightness = null, double? duration = null, double? infrared = null)
        {
            Selector = selector;
            PowerState = powerState;
            Color = color;
            Brightness = brightness;
            Duration = duration;
            Infrared = infrared;
        }

        public LightState() { }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            if(Selector != null)
            {
                result.Append(string.Format("Selector: {0} ", Selector));
            }
            result.Append(string.Format("Power State: {0} ", PowerState));
            if (Color != null)
            {
                result.Append(string.Format("Color: {0} ", Color));
            }
            if (Brightness != null)
            {
                result.Append(string.Format("Brightness: {0} ", Brightness));
            }
            if (Duration != null)
            {
                result.Append(string.Format("Duration: {0} ", Duration));
            }
            if (Infrared != null)
            {
                result.Append(string.Format("Infrared: {0} ", Infrared));
            }
            return result.ToString();
        }
    }
}
