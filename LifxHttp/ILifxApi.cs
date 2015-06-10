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

        [Post("/lights/{selector}/toggle")]
        Task<List<ApiResult>> TogglePower([Header("Authorization")] string auth, string selector);

        [Put("/lights/{selector}/power")]
        Task<List<ApiResult>> SetPower([Header("Authorization")] string auth, string selector, [Body] string args);

        [Put("/lights/{selector}/color")]
        Task<List<ApiResult>> SetColor([Header("Authorization")] string auth, string selector, [Body] string args);
    }
}
