using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxHttp
{
    internal interface ILifxApi
    {
        [Get("/lights/{selector}")]
        Task<List<Light>> ListLights([Header("Authorization")] string auth, string selector);

        [Get("/scenes")]
        Task<List<Scene>> ListScenes([Header("Authorization")] string auth);

        [Put("/lights/{selector}/state")]
        [Headers("Content-Type: application/x-www-form-urlencoded")]
        Task<ApiResults> SetState([Header("Authorization")] string auth, string selector, [Body] string args);

        [Put("/lights/states")]
        [Headers("Content-Type: application/json")]
        Task<ApiResults> SetStates([Header("Authorization")] string auth, [Body] LifxClient.SetStatesSpec args);

        [Put("/scenes/scene_id:{sceneUUID}/activate")]
        [Headers("Content-Type: application/x-www-form-urlencoded")]
        Task<ApiResults> ActivateScene([Header("Authorization")] string auth, string sceneUUID, [Body] string args);

        [Post("/lights/{selector}/toggle")]
        [Headers("Content-Type: application/x-www-form-urlencoded")]
        Task<ApiResults> TogglePower([Header("Authorization")] string auth, string selector, [Body] string args);

        [Post("/lights/{selector}/effects/pulse")]
        [Headers("Content-Type: application/x-www-form-urlencoded")]
        Task<ApiResults> PulseEffect([Header("Authorization")] string auth, string selector, [Body] string args);

        [Post("/lights/{selector}/effects/breathe")]
        [Headers("Content-Type: application/x-www-form-urlencoded")]
        Task<ApiResults> BreatheEffect([Header("Authorization")] string auth, string selector, [Body] string args);

        [Post("/lights/{selector}/cycle")]
        [Headers("Content-Type: application/json")]
        Task<ApiResults> Cycle([Header("Authorization")] string auth, string selector, [Body] LifxClient.SetStateSpec args);
    }
}
