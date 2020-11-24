using System.Collections.Generic;

namespace CrossX.Core
{
    public class FeaturesFlags : IFeaturesFlags
    {
        public IReadOnlyCollection<string> Flags => flags;
        private readonly HashSet<string> flags = new HashSet<string>();

        public void Add(string flag)
        {
            flags.Add(flag);
        }

        public bool Check(string flag)
        {
            return flags.Contains(flag);
        }
    }
}
