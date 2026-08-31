using System.Collections.Generic;
using Core.DI;
using Core.Utility;
using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.Object.StateMeachine;
using HotUpdate.Game.Battle.ResponsibilityChain.DamageChain;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Component;
using HotUpdate.Game.Battle.Skill.Conditions;
using HotUpdate.Game.Battle.Skill.Factory;
using HotUpdate.Game.Battle.StatSystem;
using HotUpdate.Game.Battle.TargetSelect;
using HotUpdate.Game.Battle.TargetSelect.Strategys;

namespace HotUpdate.Game.Battle.Object.Role
{
    /// <summary>
    /// 角色对象
    /// </summary>
    public abstract class RoleObject : BattleObject, IRoleObject
    {
        public RoleInfo RoleInfo { get; private set; }

        public override ISkillFactory SkillFactory { get; protected set; }
        
        public override ICastSkillCondition DefaultCastCondition { get; protected set; }
        
        public override ITargetSelectStrategy DefaultTargetSelectStrategy { get; protected set; }

        public void RoleBattleInit(BattleParameterObject parameter)
        {
            BattleInit(parameter);
            SetRoleInfo((RoleInfo)parameter.BattleInfo);
            // 初始化伤害链
            damageChain = DamageChainBuilder.GetRoleDamageChain();
            // 添加组件
            AddComponents(TextUtility.Split(RoleInfo.f_comNames, 2));
        }

        public void SetRoleInfo(RoleInfo roleInfo)
        {
            RoleInfo = roleInfo;
        }

        protected override ICastSkillCondition GetSkillCondition()
        {
            return castSkillConditionFactory.GetCastSkillCondition<PlayerDefaultCastSkillCondition>();
        }

        protected override ITargetSelectStrategy GetTargetSelectStrategy()
        {
            return targetSelectStrategyFactory.GetTargetSelectStrategy<PlayerBaseTargetSelectStrategy>();
        }

        protected override List<ITurnStartNode> GetStartNodes()
        {
            return new List<ITurnStartNode>
            {
                DIContainer.Create<StatusSettlementNode>(),
                DIContainer.Create<DotSettlementNode>(),
            };
        }

        public override void CastSkill(int skillId)
        {
            var skillComponent = GetComponent<PlayerSkillComponent>();
            // 能否释放
            if (!skillComponent.CanCast(skillId))
                return;
            
            // 获取技能数据
            var skill = skillComponent.GetSkill(skillId);
            // 若是终结技，则重置标识
            if (skill.SkillContext.SkillInfo.f_SkillType == (byte)E_SkillType.UltimateSkill)
            {
                skillComponent.IsTrigger = true;
                skillComponent.IsRelease = false;
            }
            else
            {
                // 非终结技默认只能行动一次
                CanAct = false;
            }
            
            var skillCommand = commandfactory.GetSkillCommand(skill);
            // 发送指令
            Context.EventBus.TriggerEvent(new InsertCommandEvent(Context, skillCommand));
            // 正在行动
            Acting = true;
        }

        public void RecoverUltimate(float value)
        {
            this.GetComponent<RoleStatComponent>().GainResource(value);
        }
    }
}
