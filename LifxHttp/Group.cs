using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxHttp
{

    public sealed class Group : LightCollection
    {
        public Group(LifxClient client, string id, string label, List<Light> lights)
            : base(client, id, label, lights) { }

        public static implicit operator Selector(Group group)
        {
            return new Selector.GroupId(group.Id);
        }
    }
}
