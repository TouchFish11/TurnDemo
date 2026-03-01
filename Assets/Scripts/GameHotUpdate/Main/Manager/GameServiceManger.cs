using Core.Service;
using GameHotUpdate.Battle.Core;
using GameHotUpdate.Dialogue;
using GameHotUpdate.Input;
using GameHotUpdate.Main.FloatingText;
using GameHotUpdate.Main.Object;
using GameHotUpdate.Task.Core;
using GameHotUpdate.VFX;

namespace GameHotUpdate.Main.Manager
{
    public class GameServiceManger : IGameServiceManger
    {
        public void InitService()
        {
            // 测试
            ServiceLocator.Register<IObjectBuilder>(new ObjectBuilder()); 
            
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
