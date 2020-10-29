using System;

namespace CrossX.Core
{
    public interface IApp
    {
        void LoadContent();
        void Update(TimeSpan frameTime);
        void Draw(TimeSpan frameTime);
    }
}
