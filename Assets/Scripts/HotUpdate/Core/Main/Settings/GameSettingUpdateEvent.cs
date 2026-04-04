using Core.GlobalEvent;

namespace HotUpdate.Core.Main.Settings
{
    /// <summary>
    /// 游戏设置更新事件
    /// </summary>
    public class GameSettingUpdateEvent : Event
    {
        public GameSettings GameSettings { get; set; }
    }
}
