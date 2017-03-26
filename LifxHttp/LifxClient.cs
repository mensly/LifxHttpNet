using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LifxHttp.Enums;
using Newtonsoft.Json;

namespace LifxHttp
{
    /// <summary>
    /// Main class used to abstract functionality of the Lifx HTTP API.
    /// </summary>
    public class LifxClient
    {
        internal const double DEFAULT_DURATION = 1.0d;
        internal const bool DEFAULT_POWER_ON = true;
        internal const double DEFAULT_INFRARED = 0.0d;
        internal const LifxColor DEFAULT_FROM_COLOR = null;
        internal const double DEFAULT_PEAK = 0.5d;
        internal const bool DEFAULT_PERSIST = false;
        internal const Enums.Direction DEFAULT_DIRECTION = Direction.Forward;
        internal const double MIN_BRIGHTNESS = 0.0d;
        internal const double MAX_BRIGHTNESS = 1.0d;
        internal const double MIN_DURATION = 0.0d;
        internal const double MAX_DURATION = 3155760000.0d;
        internal const double MIN_PEAK = 0.0d;
        internal const double MAX_PEAK = 1.0d;

        private ILifxApi lifxApi;
        protected readonly string auth;

        /// <summary>
        /// Create a new LifxClient that can perform actions over the HTTP API.
        /// </summary>
        /// <param name="token">User token as generated from https://cloud.lifx.com/settings </param>
        public LifxClient(string token)
        {
            auth = "Bearer " + token;
            lifxApi = Refit.RestService.For<ILifxApi>("https://api.lifx.com/v1");
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class SetStateSpec
        {
            [JsonProperty("states")]
            public List<LightState> States { get; set; }
            [JsonProperty("defaults")]
            public LightState Defaults { get; set; }
            [JsonProperty("direction")]
            public Direction Direction { get; set; }
            public SetStateSpec(List<LightState> states, LightState defaults, Direction direction)
            {
                States = states;
                Defaults = defaults;
                Direction = direction;
            }
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class SetStatesSpec
        {
            [JsonProperty("states")]
            public List<LightState> States { get; set; }
            [JsonProperty("defaults")]
            public LightState Defaults { get; set; }
            public SetStatesSpec(List<LightState> states, LightState defaults)
            {
                States = states;
                Defaults = defaults;
            }
        }

        /// <summary>
        /// Gets lights belonging to the authenticated account
        /// </summary>
        /// <param name="selector">Filter for which lights are targetted</param>
        /// <returns>All matching lights</returns>
        public async Task<List<Light>> ListLights(Selector selector = null)
        {
            if (selector == null) { selector = Selector.All; }
            List<Light> lights;
            lights = await lifxApi.ListLights(auth, selector.ToString());
            foreach (var light in lights)
            {
                // Attach this client to lights
                light.Client = this;
            }
            return lights;
        }

        /// <summary>
        /// Gets light groups belonging to the authenticated account
        /// </summary>
        /// <param name="selector">Filter for which lights are targetted</param>
        /// <returns>All groups containing matching lights</returns>
        public async Task<List<Group>> ListGroups(Selector selector = null)
        {
            return (await ListLights(selector)).AsGroups();
        }

        /// <summary>
        /// Gets locations belonging to the authenticated account
        /// </summary>
        /// <param name="selector">Filter for which lights are targetted</param>
        /// <returns>All locations containing matching lights</returns>
        public async Task<List<Location>> ListLocations(Selector selector = null)
        {
            return (await ListLights(selector)).AsLocations();
        }

        /// <summary>
        /// Lists all the scenes available in the user's account. Scenes listed here can be activated with the ActivateScene. As documented here: https://api.developer.lifx.com/docs/list-scenes.
        /// </summary>
        /// <returns>All scenes available.</returns>
        public async Task<List<Scene>> ListScenes()
        {
            return await lifxApi.ListScenes(auth);
        }

        /// <summary>
        /// Toggles light power state. As documented here: https://api.developer.lifx.com/docs/toggle-power.
        /// </summary>
        /// <param name="selector">Filter for which lights are targetted</param>
        /// <param name="duration">How long in seconds you want the power action to take. Range: 0.0 – 3155760000.0 (100 years).</param>
        /// <returns></returns>
        public async Task<ApiResults> TogglePower(Selector selector, double duration = DEFAULT_DURATION)
        {
            if (selector == null) { selector = Selector.All; }
            duration = duration < MIN_DURATION ? MIN_DURATION : duration > MAX_DURATION ? MAX_DURATION : duration;
            string args = string.Format("duration={0}", duration);
            try
            {
                return await lifxApi.TogglePower(auth, selector.ToString(), args);
            }
            catch (Refit.ApiException e)
            {
                return e.GetContentAs<ApiResults>();
            }
        }

        /// <summary>
        /// Set power to an explicit state (on/off).
        /// </summary>
        /// <param name="selector">Filter for which lights are targetted</param>
        /// <param name="powerState">On or off.</param>
        /// <param name="duration">How long in seconds you want the power action to take. Range: 0.0 – 3155760000.0 (100 years).</param>
        /// <returns></returns>
        public async Task<ApiResults> SetPower(Selector selector, PowerState powerState, double duration = DEFAULT_DURATION)
        {
            if (selector == null) { selector = Selector.All; }
            string args = string.Format("power={0}&duration={1}", powerState.ToString().ToLowerInvariant(), duration);
            try
            {
                return await lifxApi.SetState(auth, selector.ToString(), args);
            }
            catch (Refit.ApiException e)
            {
                return e.GetContentAs<ApiResults>();
            }
        }

        /// <summary>
        /// Set the light to a specific color.
        /// </summary>
        /// <param name="selector">Filter for which lights are targetted</param>
        /// <param name="color">The color to set the light to.</param>
        /// <param name="duration">How long in seconds you want the power action to take. Range: 0.0 – 3155760000.0 (100 years).</param>
        /// <param name="powerOn">If true, turn the bulb on if it is not already on.</param>
        /// <returns></returns>
        public async Task<ApiResults> SetColor(Selector selector, LifxColor color, double duration = DEFAULT_DURATION, bool powerOn = DEFAULT_POWER_ON)
        {
            if (selector == null) { selector = Selector.All; }
            string args = string.Format("color={0}&duration={1}", color, duration);
            if (powerOn) { args = args + "&power=" + PowerState.On.ToString().ToLowerInvariant(); }
            try
            {
                return await lifxApi.SetState(auth, selector.ToString(), args);
            }
            catch (Refit.ApiException e)
            {
                return e.GetContentAs<ApiResults>();
            }
        }

        /// <summary>
        /// Set an explicit state. As documented here: https://api.developer.lifx.com/docs/set-state.
        /// </summary>
        /// <param name="selector">Filter for which lights are targetted</param>
        /// <param name="powerState">On or off.</param>
        /// <param name="color">The color to set the light to.</param>
        /// <param name="brightness">The brightness level from 0.0 to 1.0. Overrides any brightness set in color (if any).</param>
        /// <param name="duration">How long in seconds you want the power action to take. Range: 0.0 – 3155760000.0 (100 years).</param>
        /// <param name="infrared">The maximum brightness of the infrared channel.</param>
        /// <returns></returns>
        public async Task<ApiResults> SetState(Selector selector, PowerState powerState, LifxColor color, double brightness, double duration = LifxClient.DEFAULT_DURATION, double infrared = LifxClient.DEFAULT_INFRARED)
        {
            if (selector == null) { selector = Selector.All; }
            string args = string.Format("power={0}&color={1}&brightness={2}&duration={3}&infrared={4}", powerState.ToString().ToLowerInvariant(), color, brightness, duration, infrared);
            try
            {
                return await lifxApi.SetState(auth, selector.ToString(), args);
            }
            catch (Refit.ApiException e)
            {
                return e.GetContentAs<ApiResults>();
            }
        }

        /// <summary>
        /// Set multiple states across multiple selectors. As documented here: https://api.developer.lifx.com/docs/set-states.
        /// </summary>
        /// <param name="states">A list of states (each which may have a unique selector to chose the light(s) which will be set to said state).</param>
        /// <param name="defaults">A state which contains default properties that are inherited by states that don't explicitely set them.</param>
        /// <returns></returns>
        public async Task<ApiResults> SetStates(List<LightState> states, LightState defaults)
        {
            SetStatesSpec args = new SetStatesSpec(states, defaults);
            try
            {
                return await lifxApi.SetStates(auth, args);
            }
            catch (Refit.ApiException e)
            {
                return e.GetContentAs<ApiResults>();
            }
        }

        /// <summary>
        /// Activates a scene from the users account. As documented here: https://api.developer.lifx.com/docs/activate-scene.
        /// </summary>
        /// <param name="sceneUUID">The UUID for the scene you wish to activate.</param>
        /// <param name="duration">The time in seconds to spend performing the scene transition.</param>
        /// <returns></returns>
        public async Task<ApiResults> ActivateScene(string sceneUUID, double duration = DEFAULT_DURATION)
        {
            string args = string.Format("duration={0}", duration);
            try
            {
                return await lifxApi.ActivateScene(auth, sceneUUID, args);
            }
            catch (Refit.ApiException e)
            {
                return e.GetContentAs<ApiResults>();
            }
        }

        /// <summary>
        /// Triggers a pulsing ("strobe") effect on the light. During each cycle the light switches between two colors ("fromColor" and "color"). As documented here: https://api.developer.lifx.com/docs/pulse-effect.
        /// </summary>
        /// <param name="selector">Filter for which lights are targetted</param>
        /// <param name="color">Color which the light changes to. Light alternates between this and fromColor.</param>
        /// <param name="period">The time (in seconds) to complete one cycle of the effect.</param>
        /// <param name="cycles">The number of times to repeat the effect.</param>
        /// <param name="fromColor">Color which the light begins on and changes back to at the start of the next cycle. If omitted, this will be the current color of the light. Set to LifxColor.OffState to toggle between an off state and your chosen color (which must have a brightness level set above 0... default named colors will result in the light staying off).</param>
        /// <param name="persist">If false, this sets the light back to its previous state when the effect ends. If true the light remains on the last color of the effect.</param>
        /// <param name="powerOn">If true, turn the bulb on if it is not already on.</param>
        /// <returns></returns>
        public async Task<ApiResults> PulseEffect(Selector selector, LifxColor color, double period, double cycles, LifxColor fromColor = null, bool persist = false, bool powerOn = DEFAULT_POWER_ON)
        {
            if (selector == null) { selector = Selector.All; }
            string args = string.Format("color={0}&from_color={1}&period={2}&cycles={3}&persist={4}&power_on={5}", color, fromColor, period, cycles, persist, powerOn);
            try
            {
                return await lifxApi.PulseEffect(auth, selector.ToString(), args);
            }
            catch (Refit.ApiException e)
            {
                return e.GetContentAs<ApiResults>();
            }
        }

        /// <summary>
        /// Triggers a breathing ("fade") effect on the light. During each cycle the light fades between two colors ("fromColor" and "color"). As documented here: https://api.developer.lifx.com/docs/breathe-effect.
        /// </summary>
        /// <param name="selector">Filter for which lights are targetted</param>
        /// <param name="color">Color which the light fades to. Light alternates between this and fromColor.</param>
        /// <param name="period">The time (in seconds) to complete one cycle of the effect.</param>
        /// <param name="cycles">The number of times to repeat the effect.</param>
        /// <param name="fromColor">Color which the light begins on and changes back to at the start of the next cycle. If omitted, this will be the current color of the light. Set to LifxColor.OffState to toggle between an off state and your chosen color (which must have a brightness level set above 0... default named colors will result in the light staying "off").</param>
        /// <param name="persist">If false, this sets the light back to its previous state when the effect ends. If true the light remains on the last color of the effect.</param>
        /// <param name="powerOn">If true, turn the bulb on if it is not already on.</param>
        /// <param name="peak">Defines where in a period the target color is at its maximum. Minimum 0.0, maximum 1.0.</param>
        /// <returns></returns>
        public async Task<ApiResults> BreatheEffect(Selector selector, LifxColor color, double period, double cycles, LifxColor fromColor = null, bool persist = false, bool powerOn = DEFAULT_POWER_ON, double peak = DEFAULT_PEAK)
        {
            if (selector == null) { selector = Selector.All; }
            string args = string.Format("color={0}&from_color={1}&period={2}&cycles={3}&persist={4}&power_on={5}&peak={6}", color, fromColor, period, cycles, persist, powerOn, peak);
            try
            {
                return await lifxApi.BreatheEffect(auth, selector.ToString(), args);
            }
            catch (Refit.ApiException e)
            {
                return e.GetContentAs<ApiResults>();
            }
        }

        /// Triggers a state cycle effect on the light. During each cycle the light transitions between 2 - 5 predefined states that will inherit properties from the default state. As documented here: https://api.developer.lifx.com/docs/cycle.
        /// <param name="states">A list of states. Within each you can set power, color, brightness, power action duration, and infrared channel value. Must have 2 to 5 entries.</param>
        /// <param name="defaults">A state which contains default properties that are inherited by states that don't explicitely set them.</param>
        /// <param name="direction">Direction in which to cycle through the list of states.</param>
        /// <returns></returns>
        public async Task<ApiResults> Cycle(Selector selector, List<LightState> states, LightState defaults, Direction direction = DEFAULT_DIRECTION)
        {
            if (selector == null) { selector = Selector.All; }
            SetStateSpec args = new SetStateSpec(states, defaults, direction);
            try
            {
                return await lifxApi.Cycle(auth, selector.ToString(), args);
            }
            catch (Refit.ApiException e)
            {
                return e.GetContentAs<ApiResults>();
            }
        }
    }
}
