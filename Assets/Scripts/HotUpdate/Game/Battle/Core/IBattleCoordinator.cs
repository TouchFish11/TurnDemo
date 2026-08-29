using System.Collections;
using System.Threading.Tasks;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.TargetSelect;

namespace HotUpdate.Game.Battle.Core
{
    public interface IBattleCoordinator
    {
        OperationState OperationState { get; }
        
        /// <summary>
        /// 初始化战斗协调器
        /// </summary>
        /// <param name="battleContext"></param>
        /// <param name="operationState"></param>
        void Init(IBattleContext battleContext, OperationState operationState);

        /// <summary>
        /// 重置管理器状态
        /// </summary>
        void Reset();

        /// <summary>
        /// 初始化技能目标
        /// </summary>
        /// <param name="skill"></param>
        void InitSkillTarget(ISkill skill);

        /// <summary>
        /// 设置技能信息缓存
        /// </summary>
        /// <param name="skillInfo"></param>
        void SetSelectSkillInfo(SkillInfo skillInfo);

        /// <summary>
        /// 根据技能信息自动重新计算主目标和范围内的目标，触发目标选择事件更新UI显示,在选择目标前要先设置SetSelectSkillInfo
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="targetSelectStrategy"></param>
        void SelectTargets(IBattleEntityObject caster, ITargetSelectStrategy targetSelectStrategy);

        /// <summary>
        /// 更新相机看向,看向怪物或玩家角色
        /// </summary>
        /// <param name="skillTargetType"></param>
        /// <param name="roleObject"></param>
        Task UpdateCamera(E_SkillTargetType skillTargetType, RoleObject roleObject);

        /// <summary>
        /// 执行玩家角色终结技释放前逻辑
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skillInfo"></param>
        IEnumerator ExecutePreUltimateCast(IBattleEntityObject caster, SkillInfo skillInfo);

        /// <summary>
        /// 传入玩家角色对象，更新相机的位置和渲染
        /// </summary>
        /// <param name="roleObject"></param>
        Task UpdateCamera(RoleObject roleObject);
    }
}
