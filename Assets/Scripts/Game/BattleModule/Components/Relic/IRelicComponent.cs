
namespace GameLogic.BattleMoudule.Relic
{
    /// <summary>
    /// 遗器组件接口
    /// </summary>
    public interface IRelicComponent : IComponent
    {
        void Init(IBattleEntity owner);
    }
}
