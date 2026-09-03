using HotUpdate.Game.Battle.Object.Role;

namespace HotUpdate.Game.Battle.Object
{
    /// <summary>
    /// 玩家输入驱动：打开操作后等待玩家选择（输入开启由 TurnStartEvent 处理器负责）。
    /// </summary>
    public class PlayerInputDriver : ITurnActionDriver
    {
        private readonly IRoleObject _roleObject;

        public PlayerInputDriver(IRoleObject roleObject)
        {
            _roleObject = roleObject;
        }

        public void OnOperationOpened()
        {
            // 玩家输入由 OnTurnStartDispatch 开启并等待；这里留空，作为自动战斗等替换驱动的扩展点。
        }
    }
}