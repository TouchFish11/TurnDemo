
using System.Threading.Tasks;

namespace Framework
{
    public delegate void GameSettingEvent<in T>(T value);

    public class GameSettingManager : SingletonBase<GameSettingManager>
    {
        // 游戏设置数据
        private GameSettingData gameSettingData;

        /// <summary>
        /// 对话文本打字机设置变化事件
        /// </summary>
        public event GameSettingEvent<bool> OnEnableTypewriterChanged;

        private GameSettingManager()
        {

        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            gameSettingData = JsonManager.Instance.FromJson<GameSettingData>("");
        }

        /// <summary>
        /// 设置是否启用对话文本打字机效果
        /// </summary>
        /// <param name="enable"></param>
        public void SetEnableTypewriter(bool enable)
        {
            gameSettingData.enableTypewriter = enable;
            OnEnableTypewriterChanged?.Invoke(enable);
        }
    }
}
