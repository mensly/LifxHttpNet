using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxHttp
{

    public sealed class Location : IEnumerable<Light>
    {
        public string Id { get; private set; }
        public string Label { get; private set; }

        private List<Light> lights;

        public Location(string id, string label, List<Light> lights)
        {
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
        public override string ToString()
        {
            return Label;
        }
    }
}
