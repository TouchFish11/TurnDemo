using Core.Service;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Dialogue;
using HotUpdate.Core.Input;
using HotUpdate.Core.Main;
using HotUpdate.Core.Task;
using HotUpdate.Core.VFX;
using UnityEngine.VFX;

namespace HotUpdate.Core
{
    /// <summary>
    /// 游戏服务管理器
    /// </summary>
    public class GameServiceManager
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
