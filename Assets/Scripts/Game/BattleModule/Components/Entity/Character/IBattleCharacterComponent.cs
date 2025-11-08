
namespace GameLogic.BattleMoudule.Entity
{
    /// <summary>
    /// 战斗角色组件
    /// </summary>
    public interface IBattleCharacterComponent : IComponent
    {
        void Init(IBattleEntity owner);
    }
}
