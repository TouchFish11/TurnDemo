
namespace GameLogic.BattleMoudule.Summon
{
    /// <summary>
    /// 召唤组件接口
    /// </summary>
    public interface ISummonComponent : IComponent
    {
        void Init(IBattleEntity owner);
    }
}
