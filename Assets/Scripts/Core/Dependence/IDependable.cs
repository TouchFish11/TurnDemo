using System.Threading.Tasks;

namespace Core.Dependence
{
    public interface IDependable
    {
        Task OnDependcyInited();
    }
}
