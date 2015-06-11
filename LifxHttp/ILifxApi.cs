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
        Task<Light> GetLight([Header("Authorization")] string auth, string selector);

        [Get("/lights/{selector}")]
        Task<List<Light>> ListLights([Header("Authorization")] string auth, string selector);

        [Post("/lights/{selector}/toggle")]
        Task<List<ApiResult>> TogglePower([Header("Authorization")] string auth, string selector);

        [Post("/lights/{selector}/toggle")]
        Task<ApiResult> TogglePowerSingle([Header("Authorization")] string auth, string selector);

        [Put("/lights/{selector}/power")]
        [Headers("Content-Type: application/x-www-form-urlencoded")]
        Task<List<ApiResult>> SetPower([Header("Authorization")] string auth, string selector, [Body] string args);

        [Put("/lights/{selector}/power")]
        [Headers("Content-Type: application/x-www-form-urlencoded")]
        Task<ApiResult> SetPowerSingle([Header("Authorization")] string auth, string selector, [Body] string args);

        [Put("/lights/{selector}/color")]
        [Headers("Content-Type: application/x-www-form-urlencoded")]
        Task<List<ApiResult>> SetColor([Header("Authorization")] string auth, string selector, [Body] string args);
        [Put("/lights/{selector}/color")]
        [Headers("Content-Type: application/x-www-form-urlencoded")]
        Task<ApiResult> SetColorSingle([Header("Authorization")] string auth, string selector, [Body] string args);
    }
}
