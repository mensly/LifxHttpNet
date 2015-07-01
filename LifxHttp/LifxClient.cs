using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxHttp
{
    /// <summary>
    /// Main class used to abstract functionality of the Lifx HTTP API.
    /// </summary>
    public class LifxClient
    {
        internal const float DEFAULT_DURATION = 1f;
        internal const bool DEFAULT_POWER_ON = true;
        private ILifxApi lifxApi;
        protected readonly string auth;

        /// <summary>
        /// Create a new LifxClient that can perform actions over the HTTP API.
        /// </summary>
        /// <param name="token">User token as generated from https://cloud.lifx.com/settings </param>
        public LifxClient(string token)
        {
            auth = "Bearer " + token;
            lifxApi = Refit.RestService.For<ILifxApi>("https://api.lifx.com/v1beta1");
            
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
            if (selector.IsSingle)
            {
                Light light = await lifxApi.GetLight(auth, selector.ToString());
                lights = new List<Light>();
                if (light != null)
                {
                    lights.Add(light);
                }
            }
            else
            {
                lights = await lifxApi.ListLights(auth, selector.ToString());
            }
            foreach (var l in lights)
            {
                // Attach this client to lights
                l.Client = this;
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
        /// Turn off lights if they are on, or turn them on if they are off. 
        /// Physically powered off lights are ignored.
        /// </summary>
        /// <param name="selector">Filter for which lights are targetted</param>
        /// <returns>Result indicating success of operation</returns>
        public async Task<List<ApiResult>> TogglePower(Selector selector)
        {
            if (selector == null) { selector = Selector.All; }
            if (selector.IsSingle)
            {
                ApiResult result;
                try
                {
                    result = await lifxApi.TogglePowerSingle(auth, selector.ToString());
                }
                catch (Refit.ApiException e)
                {
                    result = e.GetContentAs<ApiResult>();
                }
                return new List<ApiResult>() { result };
            }
            else
            {
                try
                {
                    return await lifxApi.TogglePower(auth, selector.ToString());
                }
                catch (Refit.ApiException e)
                {
                    return e.GetContentAs<List<ApiResult>>();
                }
            }
        }
        /// <summary>
        /// Turn lights on, or turn lights off.
        /// </summary>
        /// <param name="powerState">True for on, false for off</param>
        /// <param name="duration">Optionally set a duration which will fade on (or off) over the given duration in seconds.</param>
        /// <param name="selector">Filter for which lights are targetted</param>
        /// <returns>Result indicating success of operation</returns>
        public async Task<List<ApiResult>> SetPower(Selector selector, bool powerState, float duration = DEFAULT_DURATION)
        {
            return await SetPower(selector, powerState ? PowerState.On : PowerState.Off, duration);
        }
        /// <summary>
        /// Turn lights on, or turn lights off.
        /// </summary>
        /// <param name="powerState">Desired power state for lights</param>
        /// <param name="duration">Optionally set a duration which will fade on (or off) over the given duration in seconds.</param>
        /// <param name="selector">Filter for which lights are targetted</param>
        /// <returns>Result indicating success of operation</returns>
        public async Task<List<ApiResult>> SetPower(Selector selector, PowerState powerState, float duration = DEFAULT_DURATION)
        {
            if (selector == null) { selector = Selector.All; }
            string args = string.Format("state={0}&duration={1}", powerState.ToString().ToLowerInvariant(), duration);
            if (selector.IsSingle)
            {
                ApiResult result;
                try
                {
                    result = await lifxApi.SetPowerSingle(auth, selector.ToString(), args);
                }
                catch (Refit.ApiException e)
                {
                    result = e.GetContentAs<ApiResult>();
                }
                return new List<ApiResult>() { result };
            }
            else 
            {
                try
                {
                    return await lifxApi.SetPower(auth, selector.ToString(), args);
                }
                catch (Refit.ApiException e)
                {
                    return e.GetContentAs<List<ApiResult>>();
                }
            }
        }

        public async Task<List<ApiResult>> SetColor(Selector selector, LifxColor color, float duration = DEFAULT_DURATION, bool powerOn = DEFAULT_POWER_ON)
        {
            if (selector == null) { selector = Selector.All; }
            string args = string.Format("color={0}&duration={1}&power_on={2}", color, duration, powerOn);
            if (selector.IsSingle)
            {
                ApiResult result;
                try
                {
                    result = await lifxApi.SetColorSingle(auth, selector.ToString(), args);
                }
                catch (Refit.ApiException e)
                {
                    result = e.GetContentAs<ApiResult>();
                }
                return new List<ApiResult>() { result };
            }
            else
            {
                try
                {
                    return await lifxApi.SetColor(auth, selector.ToString(), args);
                }
                catch (Refit.ApiException e)
                {
                    return e.GetContentAs<List<ApiResult>>();
                }
            }
        }
        
    }
}
