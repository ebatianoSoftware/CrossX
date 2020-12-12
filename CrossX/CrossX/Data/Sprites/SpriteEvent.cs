using System.Collections.Generic;

namespace CrossX.Data.Sprites
{
    public class SpriteEvent
    {
        private readonly Dictionary<string, object> parameters;
        public string Id { get; }

        public SpriteEvent(string id)
        {
            Id = id;
        }

        public SpriteEvent(string id, Dictionary<string, object> parameters): this(id)
        {
            this.parameters = parameters;
        }

        public T GetParameter<T>(string name)
        {
            if (parameters == null) return default;
            if (!parameters.TryGetValue(name, out var parameter)) return default;

            if (parameter is T) return (T)parameter;
            return default;
        }
    }
}