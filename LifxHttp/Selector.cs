using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxHttp
{
    /// <summary>
    /// Used for addressing one or many Lights belonging to the authenticated account.
    /// </summary>
    public class Selector
    {
        private const string TYPE_ALL = "all";
        private const string TYPE_RANDOM = "random";
        private const string TYPE_LIGHT_ID = "id";
        private const string TYPE_GROUP_ID = "group_id";
        private const string TYPE_GROUP_LABEL = "group";
        private const string TYPE_LOCATION_ID = "location_id";
        private const string TYPE_LOCATION_LABEL = "location";

        /// <summary>
        /// All lights belonging to the authenticated account.
        /// </summary>
        public static readonly Selector All = new Selector(TYPE_ALL);
        /// <summary>
        /// One randomly selected light belonging to the authenticated account.
        /// </summary>
        private static readonly Selector Random = new Selector(TYPE_RANDOM) { IsSingle = true };

        private readonly string selector;
        internal bool IsSingle { get; private set; }

        private Selector(string selector) { this.selector = selector; }

        private Selector(string type, string criteria) : this(string.Format("{0}:{1}", type, criteria)) { }

        public override string ToString()
        {
            return selector;
        }

        /// <summary>
        /// Only the light with the given ID.
        /// </summary>
        public class LightId : Selector
        {
            public LightId(string id) : base(TYPE_LIGHT_ID, id) { IsSingle = true; }
        }

        /// <summary>
        /// The first light that matches the label.
        /// </summary>
        public class LightLabel : Selector
        {
            public LightLabel(string label) : base(label) { IsSingle = true; }
        }

        /// <summary>
        /// Only the lights belonging to the group with the given ID.
        /// </summary>
        public class GroupId : Selector
        {
            public GroupId(string id) : base(TYPE_GROUP_ID, id) { }
        }

        /// <summary>
        /// Only the lights belonging to the groups matching the given label.
        /// </summary>
        public class GroupLabel : Selector
        {
            public GroupLabel(string label) : base(TYPE_GROUP_LABEL, label) { }
        }


        /// <summary>
        /// OOnly the lights belonging to the location matching the given ID.
        /// </summary>
        public class LocationId : Selector
        {
            public LocationId(string id) : base(TYPE_LOCATION_ID, id) { }
        }

        /// <summary>
        /// Only the lights belonging to the locations matching the given label.
        /// </summary>
        public class LocationLabel : Selector
        {
            public LocationLabel(string label) : base(TYPE_LOCATION_LABEL, label) { }
        }

        public static explicit operator Selector(string selector)
        {
            switch (selector)
            {
                case TYPE_ALL: return All;
                case TYPE_RANDOM: return Random;
                default:
                    int criteria = selector.IndexOf(':');
                    if (0 <= criteria && criteria < selector.Length - 1)
                    {
                        string remainder = selector.Substring(criteria + 1);
                        switch (selector.Substring(0, criteria))
                        {
                            case TYPE_LIGHT_ID: return new LightId(remainder);
                            case TYPE_GROUP_ID: return new GroupId(remainder);
                            case TYPE_GROUP_LABEL: return new GroupLabel(remainder);
                            case TYPE_LOCATION_ID: return new LocationId(remainder);
                            case TYPE_LOCATION_LABEL: return new LocationLabel(remainder);
                            default: return new LightLabel(selector);
                        }
                    }
                    else
                    {
                        return new LightLabel(selector);
                    }
            }
        }
    }
}
