using System.Threading.Tasks;
using HotUpdate.Game.Battle.Object.Role;

namespace HotUpdate.Game.Battle.Object
{
    /// <summary>
    /// 玩家输入驱动
    /// </summary>
    public class PlayerInputDriver : ITurnActionDriver
    {
        private readonly IRoleObject _roleObject;

        public PlayerInputDriver(IRoleObject roleObject)
        {
            _roleObject = roleObject;
        }
        
        public async Task WaitForOperation()
        {
            while (_roleObject.CanAct || _roleObject.Acting)
            {
                await Task.Yield();
            }
        }
    }
}
