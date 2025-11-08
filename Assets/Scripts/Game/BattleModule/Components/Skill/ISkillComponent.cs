
namespace GameLogic.BattleMoudule.Skill
{
    /// <summary>
    /// 技能组件接口
    /// </summary>
    public interface ISkillComponent : IComponent
    {
        void Init(IBattleEntity owner);
    }
}
