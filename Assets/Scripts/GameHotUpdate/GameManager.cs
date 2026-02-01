using System.Threading.Tasks;
using Core.DataPersistence;
using Core.DataPersistence.Json;
using Core.Log;
using Core.Service;
using Core.Singleton;
using Core.UI;
using Core.Utility;
using Game.Battle;
using Game.Dialogue;
using Game.FloatingText;
using Game.Input;
using Game.Main;
using Game.Manager;
using Game.Objects;
using Game.Tasks;
using Game.VFX;
using GameHotUpdate.Dialogue;
using GameHotUpdate.FloatingText;
using GameHotUpdate.Input;
using GameHotUpdate.Main;
using GameHotUpdate.Tasks;
using GameHotUpdate.UI;

namespace GameHotUpdate
{
    /// <summary>
    /// 游戏管理器
    /// </summary>
    public class GameManager : SingletonBase<GameManager>, IGameManager
    {
        private GameManager()
        {
            ServiceLocator.Get<IGameDataManager>().OnInitData += OnInitData;
            ServiceLocator.Get<IGameDataManager>().OnSaveData += OnSaveData;
            InitGameService();
        }

        private async Task OnInitData()
        {
            // 读取任务数据
            TaskDataCollection = await JsonManager.Instance.FromJsonAsync<TaskDataCollection>(PathUtility.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
            LogManager.Log($"任务数据加载成功，{TaskDataCollection}");
            
            // ...
        }

        private async Task OnSaveData()
        {
            // 保存任务数据
            await JsonManager.Instance.SaveToJsonAsync(TaskDataCollection, PathUtility.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
            LogManager.Log($"任务数据保存成功，{TaskDataCollection}");
            
            // ...
        }

        public void InitGameService()
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
            ServiceLocator.Register<IVFXManager>(GameHotUpdate.VFX.VFXManager.Instance);
            ServiceLocator.Register<IBattleManager>(BattleManager.Instance);
        }
        
        /// <summary>
        /// 任务数据集合
        /// </summary>
        public TaskDataCollection TaskDataCollection { get; private set; }
    }
}
