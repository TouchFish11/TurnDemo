using Core.Service;
using HotUpdate.Battle.Core;
using HotUpdate.Dialogue;
using HotUpdate.Input;
using HotUpdate.Main.FloatingText;
using HotUpdate.Task.Core;
using HotUpdate.VFX;

namespace HotUpdate.Main.Manager
{
    /// <summary>
    /// 游戏服务管理器
    /// </summary>
    public class GameServiceManger
    {
        public void InitService()
        {
            // 注册游戏相关服务、管理器
            ServiceLocator.Register<IMouseManager>(MouseManager.Instance);
            ServiceLocator.Register<IFloatingTextManager>(FloatingTextManager.Instance);
            ServiceLocator.Register<IDialogueManager>(DialogueManager.Instance);
            ServiceLocator.Register<ITaskManager>(TaskManager.Instance);
            ServiceLocator.Register<IPlayerManager>(PlayerManager.Instance);
            ServiceLocator.Register<IVFXManager>(VFXManager.Instance);
            ServiceLocator.Register<IBattleManager>(BattleManager.Instance);
        }
    }
}
