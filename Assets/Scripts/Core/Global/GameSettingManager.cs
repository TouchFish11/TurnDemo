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
    public class GameSettingManager : SingletonBase<GameSettingManager>, IGameSettingManager
    {
        public override int Priority => 1;
        private IJsonManager _jsonManager;
        private IMonoAdapter _monoAdapter;
        private IEventCenter _eventCenter;
        // 游戏设置
        public GameSetting GameSetting { get; private set; }
        
        private GameSettingManager(){}
        
        public override async Task InitAsync()
        {
            _jsonManager = ServiceLocator.Get<IJsonManager>();
            _monoAdapter = ServiceLocator.Get<IMonoAdapter>();
            _eventCenter = ServiceLocator.Get<IEventCenter>();
            
            _monoAdapter.OnAppQuit += OnOnAppQuit;
            
            GameSetting = await _jsonManager.FromJsonAsync<GameSetting>($"{PathUtility.GetUserDataLocalSavePath(FileUtility.GameSettingFileName)}");
            GameSetting.enableTypewriter = true;
        }
        
        /// <summary>
        /// 设置启用打印机效果
        /// </summary>
        /// <param name="enable"></param>
        public void SetEnableTypewriter(bool enable)
        {
            GameSetting.enableTypewriter = enable;
            _eventCenter.TriggerEvent(new GameSettingUpdateEvent {GameSetting = GameSetting});
        }
        
        private async Task OnOnAppQuit()
        {
            await _jsonManager.SaveToJsonAsync(GameSetting, $"{PathUtility.GetUserDataLocalSavePath(FileUtility.GameSettingFileName)}");
            LogManager.Log($"{nameof(GameSettingManager)}.{nameof(OnOnAppQuit)}：游戏数据保存成功，{GameSetting}");
        }
    }
}
