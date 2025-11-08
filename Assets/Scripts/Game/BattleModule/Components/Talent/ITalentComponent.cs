
namespace GameLogic.BattleMoudule.Talent
{
    /// <summary>
    /// 天赋组件接口
    /// </summary>
    public interface ITalentComponent : IComponent
    {
        void Init(IBattleEntity owner);
    }
}
