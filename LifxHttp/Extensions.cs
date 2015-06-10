using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxHttp
{
    public static class Extensions
    {
        public static List<Group> AsGroups(this IEnumerable<Light> lights)
        {
            Dictionary<Light.CollectionSpec, List<Light>> groups =
                new Dictionary<Light.CollectionSpec, List<Light>>();
            foreach (Light light in lights)
            {
                if (!groups.ContainsKey(light.group))
                {
                    groups[light.group] = new List<Light>();
                }
                groups[light.group].Add(light);
            }
            return groups.Select(entry => new Group(entry.Key.id, entry.Key.name, entry.Value)).ToList();
        }
        public static List<Location> AsLocations(this IEnumerable<Light> lights)
        {
            Dictionary<Light.CollectionSpec, List<Light>> groups =
                new Dictionary<Light.CollectionSpec, List<Light>>();
            foreach (Light light in lights)
            {
                if (!groups.ContainsKey(light.location))
                {
                    groups[light.location] = new List<Light>();
                }
                groups[light.location].Add(light);
            }
            return groups.Select(entry => new Location(entry.Key.id, entry.Key.name, entry.Value)).ToList();
        }
    }
}
