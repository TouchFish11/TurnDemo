using Core.Service;
using Core.UI;
using Game.Battle;
using Game.Dialogue;
using Game.FloatingText;
using Game.Input;
using Game.Main;
using Game.Manager;
using Game.Objects;
using Game.Tasks;
using Game.VFX;
using GameHotUpdate.Cameras;
using GameHotUpdate.Dialogue;
using GameHotUpdate.FloatingText;
using GameHotUpdate.Input;
using GameHotUpdate.Main;
using GameHotUpdate.Tasks;
using GameHotUpdate.UI;

namespace GameHotUpdate.Manager
{
    public class GameServiceManger : IGameServiceManger
    {
        public void InitService()
        {
            // 测试
            ServiceLocator.Register<IObjectBuilder>(new ObjectBuilder()); 
            ServiceLocator.Register<IUIManager>(UIManager.Instance);
            
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
