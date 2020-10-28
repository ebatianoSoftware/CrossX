using CrossX.IoC;
using System.Threading.Tasks;

namespace CrossX.Core
{
    public interface IApp
    {
        void LoadContent();
        void Update(double time);
        void Draw(double time);
    }
}
