using Core.Service;
using Core.UI;
using Game.Dialogue;
using Game.FloatingText;
using Game.Input;
using Game.Main;
using Game.Tasks;
using Game.VFX;
using GameHotUpdate.Battle;
using GameHotUpdate.Dialogue;
using GameHotUpdate.FloatingText;
using GameHotUpdate.Input;
using GameHotUpdate.Main;
using GameHotUpdate.Object;
using GameHotUpdate.Tasks;

namespace GameHotUpdate.Manager
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
            ServiceLocator.Register<IVFXManager>(VFX.VFXManager.Instance);
            ServiceLocator.Register<IBattleManager>(BattleManager.Instance);
        }
    }
}
