using System.Collections.Generic;

namespace CrossX.Core
{
    public interface IFeaturesFlags
    {
        IReadOnlyCollection<string> Flags { get; }
        bool Check(string flag);
        void Add(string flag);
    }
}
