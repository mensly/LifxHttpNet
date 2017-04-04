using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LifxHttp.Enums;

namespace LifxHttp
{
    public abstract class LightCollection : IEnumerable<Light>, ILightTarget<ApiResults>
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

        /// <summary>
        /// Toggles light power state. As documented here: https://api.developer.lifx.com/docs/toggle-power.
        /// </summary>
        /// <param name="duration">How long in seconds you want the power action to take. Range: 0.0 – 3155760000.0 (100 years).</param>
        /// <returns></returns>
        public async Task<ApiResults> TogglePower(double duration = LifxClient.DEFAULT_DURATION)
        {
            if (client == null) { return new ApiResults(); }
            return await client.TogglePower(this, duration);
        }

        /// <summary>
        /// Set power to an explicit state (on/off).
        /// </summary>
        /// <param name="powerState">On or off.</param>
        /// <param name="duration">How long in seconds you want the power action to take. Range: 0.0 – 3155760000.0 (100 years).</param>
        /// <returns></returns>
        public async Task<ApiResults> SetPower(PowerState powerState, double duration = LifxClient.DEFAULT_DURATION)
        {
            return await client.SetPower(this, powerState, duration);
        }

        /// <summary>
        /// Set the light to a specific color.
        /// </summary>
        /// <param name="color">The color to set the light to.</param>
        /// <param name="duration">How long in seconds you want the power action to take. Range: 0.0 – 3155760000.0 (100 years).</param>
        /// <param name="powerOn">If true, turn the bulb on if it is not already on.</param>
        /// <returns></returns>
        public async Task<ApiResults> SetColor(LifxColor color, double duration = LifxClient.DEFAULT_DURATION, bool powerOn = LifxClient.DEFAULT_POWER_ON)
        {
            return await client.SetColor(this, color, duration, powerOn);
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
        public async Task<ApiResults> SetState(PowerState powerState, LifxColor color, double brightness, double duration = LifxClient.DEFAULT_DURATION, double infrared = LifxClient.DEFAULT_INFRARED)
        {
            return await client.SetState(this, powerState, color, brightness, duration, infrared);
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
        public async Task<ApiResults> PulseEffect(LifxColor color, double period, double cycles, LifxColor fromColor = LifxClient.DEFAULT_FROM_COLOR, bool persist = LifxClient.DEFAULT_PERSIST, bool powerOn = LifxClient.DEFAULT_POWER_ON)
        {
            if (client == null) { return new ApiResults(); }
            return await client.PulseEffect(this, color, period, cycles, fromColor, persist, powerOn);
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
        public async Task<ApiResults> BreatheEffect(LifxColor color, double period, double cycles, LifxColor fromColor = LifxClient.DEFAULT_FROM_COLOR, bool persist = LifxClient.DEFAULT_PERSIST, bool powerOn = LifxClient.DEFAULT_POWER_ON, double peak = LifxClient.DEFAULT_PEAK)
        {
            if (client == null) { return new ApiResults(); }
            return await client.BreatheEffect(this, color, period, cycles, fromColor, persist, powerOn, peak);
        }

        /// Triggers a state cycle effect on the light. During each cycle the light transitions between 2 - 5 predefined states that will inherit properties from the default state. As documented here: https://api.developer.lifx.com/docs/cycle.
        /// <param name="states">A list of states. Within each you can set power, color, brightness, power action duration, and infrared channel value. Must have 2 to 5 entries.</param>
        /// <param name="defaults">A state which contains default properties that are inherited by states that don't explicitely set them.</param>
        /// <param name="direction">Direction in which to cycle through the list of states.</param>
        /// <returns></returns>
        public async Task<ApiResults> Cycle(List<LightState> states, LightState defaults, Direction direction = LifxClient.DEFAULT_DIRECTION)
        {
            return await client.Cycle(this, states, defaults, direction);
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
