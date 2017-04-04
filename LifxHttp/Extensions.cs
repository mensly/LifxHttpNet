using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxHttp
{
    public enum MatchMode { Any, All }

    public static class Extensions
    {
        public static List<Group> AsGroups(this IEnumerable<Light> lights)
        {
            Dictionary<Light.CollectionSpec, List<Light>> groups =
                new Dictionary<Light.CollectionSpec, List<Light>>();
            foreach (Light light in lights)
            {
                if (!groups.ContainsKey(light.Group))
                {
                    groups[light.Group] = new List<Light>();
                }
                groups[light.Group].Add(light);
            }
            // Grab client from a light
            LifxClient client = (groups.Count > 0) ? groups.First().Value.First().Client : null;
            return groups.Select(entry => new Group(client, entry.Key.Id, entry.Key.Name, entry.Value)).ToList();
        }
        public static List<Location> AsLocations(this IEnumerable<Light> lights)
        {
            Dictionary<Light.CollectionSpec, List<Light>> groups =
                new Dictionary<Light.CollectionSpec, List<Light>>();
            foreach (Light light in lights)
            {
                if (!groups.ContainsKey(light.Location))
                {
                    groups[light.Location] = new List<Light>();
                }
                groups[light.Location].Add(light);
            }
            // Grab client from a light
            LifxClient client = (groups.Count > 0) ? groups.First().Value.First().Client : null;
            return groups.Select(entry => new Location(client, entry.Key.Id, entry.Key.Name, entry.Value)).ToList();
        }

        public static bool IsSuccessful(this IEnumerable<ApiResult> results, MatchMode matchMode = MatchMode.Any)
        {
            switch (matchMode)
            {
                case MatchMode.All:
                    return results.All(ApiResult.Successful);
                default:
                case MatchMode.Any:
                    return results.Any(ApiResult.Successful);
            }
        }
    }
}
