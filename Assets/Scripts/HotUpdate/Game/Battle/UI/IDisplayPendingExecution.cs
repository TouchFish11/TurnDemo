using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.UI
{
    /// <summary>
    /// 显示待执行逻辑接口，用于战斗中等待列表中的内容UI显示
    /// </summary>
    public interface IDisplayPendingExecution
    {
        IBattleEntityObject BattleEntity { get; }
    }
}
