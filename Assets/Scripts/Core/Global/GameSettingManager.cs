using System.Threading.Tasks;
using Core.GlobalEvent;
using Core.GlobalEvent.Events;
using Core.Log;
using Core.Mono;
using Core.Serialize.Json;
using Core.Service;
using Core.Singleton;
using Core.Utility;

namespace Core.Global
{
    /// <summary>
    /// 游戏设置管理器
    /// </summary>
    public class GameSettingManager : SingletonBase<GameSettingManager>, IGameSettingManager, IApplicationExitNotify
    {
        public override int InitPriority => 1;
        public int QuitPriority => 0;
        
        private IJsonManager _jsonManager;
        private IMonoAdapter _monoAdapter;
        private IEventCenter _eventCenter;

        // 游戏设置
        public GameSettings GameSettings { get; private set; }
        
        private GameSettingManager(){}
        
        public override async Task InitAsync()
        {
            _jsonManager = ServiceLocator.Get<IJsonManager>();
            _monoAdapter = ServiceLocator.Get<IMonoAdapter>();
            _eventCenter = ServiceLocator.Get<IEventCenter>();
            
            GameSettings = await _jsonManager.FromJsonAsync<GameSettings>($"{PathUtility.GetUserDataLocalSavePath(FileUtility.GameSettingFileName)}");
            GameSettings.enableTypewriter = true;
        }
        
        /// <summary>
        /// 设置启用打印机效果
        /// </summary>
        /// <param name="enable"></param>
        public void SetEnableTypewriter(bool enable)
        {
            GameSettings.enableTypewriter = enable;
            _eventCenter.TriggerEvent(new GameSettingUpdateEvent {GameSettings = GameSettings});
        }
        
        public void OnAppQuit()
        {
            _jsonManager.SaveToJson(GameSettings, $"{PathUtility.GetUserDataLocalSavePath(FileUtility.GameSettingFileName)}");
            LogManager.Log($"{nameof(GameSettingManager)}.{nameof(OnAppQuit)}:游戏设置数据保存成功，{GameSettings}");
        }
    }
}
