using System.Threading.Tasks;

namespace HotUpdate.Game.Battle.Object
{
    public interface ITurnActionDriver
    {
        Task WaitForOperation();
    }
}
