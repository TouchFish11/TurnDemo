using Core.Utility;
using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.ResponsibilityChain.DamageChain;
using HotUpdate.Game.Battle.Skill.Component;
using HotUpdate.Game.Battle.Skill.Conditions;
using HotUpdate.Game.Battle.Skill.Factory;
using HotUpdate.Game.Battle.TargetSelect;
using HotUpdate.Game.Battle.TargetSelect.Strategys;

namespace HotUpdate.Game.Battle.Object.Monster
{
    /// <summary>
    /// 怪物战斗对象
    /// 继承自BattleObject，封装了怪物的基础属性、战斗行为等核心逻辑
    /// </summary>
    public abstract class MonsterObject : BattleObject, IMonsterObject
    {
        public MonsterInfo MonsterInfo { get; private set; }

        public override ISkillFactory SkillFactory { get; protected set; }
        
        public override ICastSkillCondition DefaultCastCondition { get; protected set; }
        
        public override ITargetSelectStrategy DefaultTargetSelectStrategy { get; protected set; }
        
        /// <summary>
        /// 战斗初始化方法
        /// 初始化怪物的技能列表和战斗组件，为进入战斗做最终准备
        /// </summary>
        public void MonsterBattleInit(BattleParameterObject parameter)
        {
            BattleInit(parameter);
            SetMonsterInfo((MonsterInfo)parameter.BattleInfo);
            // 初始化伤害链
            damageChain = DamageChainBuilder.GetMonsterDamageChain();
            // 根据配置的组件名称列表，为怪物添加对应的战斗组件（如韧性组件、动画组件等）
            AddComponents(TextUtility.Split(MonsterInfo.f_comNames, 2));
        }
        
        public void SetMonsterInfo(MonsterInfo monsterInfo)
        {
            MonsterInfo = monsterInfo;
        }

        protected override ICastSkillCondition GetSkillCondition()
        {
            return castSkillConditionFactory.GetCastSkillCondition<MonsterDefaultCastSkillCondition>();
        }

        protected override ITargetSelectStrategy GetTargetSelectStrategy()
        {
            return targetSelectStrategyFactory.GetTargetSelectStrategy<MonsterBaseTargetSelectStrategy>();
        }
        
        public abstract int SelectSkill();
        
        public override void CastSkill(int skillId)
        {
            var skillComponent = GetComponent<MonsterSkillComponent>();
            // 能否释放
            if (!skillComponent.CanCast(skillId))
                return;
            
            // 先消耗，默认只能行动一次
            ConsumeAction();
            // 获取技能数据
            var skill = skillComponent.GetSkill(skillId);
            var skillCommand = commandfactory.GetSkillCommand(skill);
            var toughnessRecoveryCommand = commandfactory.GetMonsterActionCommand(skillCommand);
            // 插入指令（同步读 CanAct，已更新）
            Context.EventBus.TriggerEvent(new InsertCommandEvent(Context, toughnessRecoveryCommand));
            // 怪物技能固定消耗行动预算
            BeginActing();
        }
    }
}