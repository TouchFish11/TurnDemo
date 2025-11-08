
namespace GameLogic.BattleMoudule.AdditionalAttack
{
    /// <summary>
    /// 追加攻击组件接口
    /// </summary>
    public interface IAdditionalAttackComponent : IComponent
    {
        void Init(IBattleEntity owner);
    }
}
