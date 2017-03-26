using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LifxHttp.Enums;

namespace LifxHttp
{
    /// <summary>
    /// Model object for a Light
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class Light : ILightTarget<ApiResult>
    {
        public const string ColorCapability = "has_color";
        public const string ColorTemperatureCapability = "has_variable_color_temp";
        public const string IRCapability = "has_ir";
        public const string MultizoneCapability = "has_multizone";

        [JsonObject(MemberSerialization.OptIn)]
        internal class CollectionSpec
        {
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
            public override bool Equals(object obj)
            {
                CollectionSpec spec = obj as CollectionSpec;
                return spec != null && spec.Id == Id && spec.Name == Name;
            }

            public override int GetHashCode()
            {
                return (Id.GetHashCode() * 77) + Name.GetHashCode();
            }
        }

        [JsonObject(MemberSerialization.OptIn)]
        internal class ProductSpec
        {
            [JsonProperty("name")]
            public string ProductName { get; set; }
            [JsonProperty("company")]
            public string Company { get; set; }
            [JsonProperty("identifier")]
            public string ProductId { get; set; }
            [JsonProperty("capabilities")]
            public Dictionary<string, bool> Capabilities { get; set; }
        }

        internal LifxClient Client { get; set; }

        /// <summary>
        /// Serial number of the light
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("uuid")]
        public string UUID { get; private set; }

        [JsonProperty("label")]
        public string Label { get; private set; }

        [JsonProperty("connected")]
        public bool IsConnected { get; private set; }

        public bool IsOn { get { return PowerState == PowerState.On; } }

        [JsonProperty("power")]
        public PowerState PowerState { get; private set; }

        [JsonProperty("color")]
        public LifxColor.HSBK Color
        {
            get { return color == null ? null : color.WithBrightness(Brightness); }
            set { color = value; }
        }

        [JsonProperty("infrared")]
        public double? Infrared { get; private set; }

        [JsonProperty("brightness")]
        public double Brightness { get; private set; }

        [JsonProperty("group")]
        internal CollectionSpec Group = new CollectionSpec();
        public string GroupId { get { return Group.Id; } }
        public string GroupName { get { return Group.Name; } }

        [JsonProperty("location")]
        internal CollectionSpec Location = new CollectionSpec();
        public string LocationId { get { return Location.Id; } }
        public string LocationName { get { return Location.Name; } }
        
        [JsonProperty("last_seen")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime LastSeen { get; private set; }

        [JsonProperty("seconds_since_seen")]
        public double SecondsSinceSeen { get; private set; }

        [JsonProperty("product")]
        internal ProductSpec Product = new ProductSpec();
        public string ProductName { get { return Product.ProductName; } private set { ProductName = value; } }
        public string Company { get { return Product.Company; } private set { Company = value; } }
        public string ProductId { get { return Product.ProductId; } private set { ProductId = value; } }
        public Dictionary<string, bool> capabilities { get { return Product.Capabilities; } }

        public IEnumerable<string> Capabilities
        {
            get
            {
                if (capabilities != null)
                {
                    foreach (var entry in capabilities)
                    {
                        if (entry.Value)
                        {
                            yield return entry.Key;
                        }
                    }
                }
            }
        }

        public bool HasCapability(string capabilitity)
        {
            return capabilities != null && capabilities.ContainsKey(capabilitity) && capabilities[capabilitity];
        }

        private LifxColor.HSBK color;

        /// <summary>
        /// Toggles light power state. As documented here: https://api.developer.lifx.com/docs/toggle-power.
        /// </summary>
        /// <param name="duration">How long in seconds you want the power action to take. Range: 0.0 – 3155760000.0 (100 years).</param>
        /// <returns></returns>
        public async Task<ApiResult> TogglePower(double duration = LifxClient.DEFAULT_DURATION)
        {
            return (await Client.TogglePower(this)).Results.First();
        }

        /// <summary>
        /// Set power to an explicit state (on/off).
        /// </summary>
        /// <param name="powerState">On or off.</param>
        /// <param name="duration">How long in seconds you want the power action to take. Range: 0.0 – 3155760000.0 (100 years).</param>
        /// <returns></returns>
        public async Task<ApiResult> SetPower(PowerState powerState, double duration = LifxClient.DEFAULT_DURATION)
        {
            return (await Client.SetPower(this, powerState, duration)).Results.First();
        }

        /// <summary>
        /// Set the light to a specific color.
        /// </summary>
        /// <param name="color">The color to set the light to.</param>
        /// <param name="duration">How long in seconds you want the power action to take. Range: 0.0 – 3155760000.0 (100 years).</param>
        /// <param name="powerOn">If true, turn the bulb on if it is not already on.</param>
        /// <returns></returns>
        public async Task<ApiResult> SetColor(LifxColor color, double duration = LifxClient.DEFAULT_DURATION, bool powerOn = LifxClient.DEFAULT_POWER_ON)
        {
            return (await Client.SetColor(this, color, duration, powerOn)).Results.First();
        }

        /// <summary>
        /// Set an explicit state. As documented here: https://api.developer.lifx.com/docs/set-state.
        /// </summary>
        /// <param name="powerState">On or off.</param>
        /// <param name="color">The color to set the light to.</param>
        /// <param name="brightness">The brightness level from 0.0 to 1.0. Overrides any brightness set in color (if any).</param>
        /// <param name="duration">How long in seconds you want the power action to take. Range: 0.0 – 3155760000.0 (100 years).</param>
        /// <param name="infrared">The maximum brightness of the infrared channel.</param>
        /// <returns></returns>
        public async Task<ApiResult> SetState(PowerState powerState, LifxColor color, double brightness, double duration = LifxClient.DEFAULT_DURATION, double infrared = LifxClient.DEFAULT_INFRARED)
        {
            return (await Client.SetState(this, powerState, color, brightness, duration, infrared)).Results.First();
        }

        /// <summary>
        /// Triggers a pulsing ("strobe") effect on the light. During each cycle the light switches between two colors ("fromColor" and "color"). As documented here: https://api.developer.lifx.com/docs/pulse-effect.
        /// </summary>
        /// <param name="color">Color which the light changes to. Light alternates between this and fromColor.</param>
        /// <param name="period">The time (in seconds) to complete one cycle of the effect.</param>
        /// <param name="cycles">The number of times to repeat the effect.</param>
        /// <param name="fromColor">Color which the light begins on and changes back to at the start of the next cycle. If omitted, this will be the current color of the light. Set to LifxColor.OffState to toggle between an off state and your chosen color (which must have a brightness level set above 0... default named colors will result in the light staying off).</param>
        /// <param name="persist">If false, this sets the light back to its previous state when the effect ends. If true the light remains on the last color of the effect.</param>
        /// <param name="powerOn">If true, turn the bulb on if it is not already on.</param>
        /// <returns></returns>
        public async Task<ApiResult> PulseEffect(LifxColor color, double period, double cycles, LifxColor fromColor = LifxClient.DEFAULT_FROM_COLOR, bool persist = LifxClient.DEFAULT_PERSIST, bool powerOn = LifxClient.DEFAULT_POWER_ON)
        {
            return (await Client.PulseEffect(this, color, period, cycles, fromColor, persist, powerOn)).Results.First();
        }

        /// <summary>
        /// Triggers a breathing ("fade") effect on the light. During each cycle the light fades between two colors ("fromColor" and "color"). As documented here: https://api.developer.lifx.com/docs/breathe-effect.
        /// </summary>
        /// <param name="color">Color which the light fades to. Light alternates between this and fromColor.</param>
        /// <param name="period">The time (in seconds) to complete one cycle of the effect.</param>
        /// <param name="cycles">The number of times to repeat the effect.</param>
        /// <param name="fromColor">Color which the light begins on and changes back to at the start of the next cycle. If omitted, this will be the current color of the light. Set to LifxColor.OffState to toggle between an off state and your chosen color (which must have a brightness level set above 0... default named colors will result in the light staying "off").</param>
        /// <param name="persist">If false, this sets the light back to its previous state when the effect ends. If true the light remains on the last color of the effect.</param>
        /// <param name="powerOn">If true, turn the bulb on if it is not already on.</param>
        /// <param name="peak">Defines where in a period the target color is at its maximum. Minimum 0.0, maximum 1.0.</param>
        /// <returns></returns>
        public async Task<ApiResult> BreatheEffect(LifxColor color, double period, double cycles, LifxColor fromColor = LifxClient.DEFAULT_FROM_COLOR, bool persist = LifxClient.DEFAULT_PERSIST, bool powerOn = LifxClient.DEFAULT_POWER_ON, double peak = LifxClient.DEFAULT_PEAK)
        {
            return (await Client.BreatheEffect(this, color, period, cycles, fromColor, persist, powerOn, peak)).Results.First();
        }

        /// Triggers a state cycle effect on the light. During each cycle the light transitions between 2 - 5 predefined states that will inherit properties from the default state. As documented here: https://api.developer.lifx.com/docs/cycle.
        /// <param name="states">A list of states. Within each you can set power, color, brightness, power action duration, and infrared channel value. Must have 2 to 5 entries.</param>
        /// <param name="defaults">A state which contains default properties that are inherited by states that don't explicitely set them.</param>
        /// <param name="direction">Direction in which to cycle through the list of states.</param>
        /// <returns></returns>
        public async Task<ApiResult> Cycle(List<LightState> states, LightState defaults, Direction direction = LifxClient.DEFAULT_DIRECTION)
        {
            return (await Client.Cycle(this, states, defaults, direction)).Results.First();
        }

        /// <summary>
        /// Re-requests light information
        /// </summary>
        /// <returns>A new instance of this light returned from API</returns>
        public async Task<Light> GetRefreshed()
        {
            return (await Client.ListLights(this)).First();
        }

        /// <summary>
        /// Re-requests light information and updates all properties
        /// </summary>
        public async Task Refresh()
        {
            Light light = await GetRefreshed();
            Id = light.Id;
            UUID = light.UUID;
            Label = light.Label;
            IsConnected = light.IsConnected;
            PowerState = light.PowerState;
            Color = light.Color;
            Brightness = light.Brightness;
            Group = light.Group;
            Location = light.Location;
            LastSeen = light.LastSeen;
            SecondsSinceSeen = light.SecondsSinceSeen;
            Product = light.Product;
        }

        public override string ToString()
        {
            return Label;
        }

        public static implicit operator Selector(Light light)
        {
            return new Selector.LightId(light.Id);
        }
    }
}
