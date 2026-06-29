using System;
using System.Collections.Generic;
using Core.DI;
using Core.Utility;
using HotUpdate.Common.Config.ExcelInfo.Info;
using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.Object.StateMeachine;
using HotUpdate.Game.Battle.Property;
using HotUpdate.Game.Battle.ResponsibilityChain.DamageChain;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Component;
using HotUpdate.Game.Battle.Skill.Conditions;
using HotUpdate.Game.Battle.TargetSelect;
using HotUpdate.Game.Battle.TargetSelect.Strategys;

namespace HotUpdate.Game.Battle.Object.Role
{
    /// <summary>
    /// 角色对象
    /// </summary>
    public abstract class PlayerObject : BattleObject, IPlayerObject
    {
        private readonly Dictionary<EActPhase, ITurnState> _turnStates = new();
        // 当前状态
        private ITurnState _currentState;
        
        public RoleInfo RoleInfo { get; private set; }

        public override ISkillFactory SkillFactory { get; protected set; }
        
        public override ICastSkillCondition DefaultCastCondition { get; protected set; }
        
        public override ITargetSelectStrategy DefaultTargetSelectStrategy { get; protected set; }

        public void RoleBattleInit(RoleBattleInitData initData)
        {
            BattleInit(initData);
            
            RoleInfo = initData.RoleInfo;
            AddState(EActPhase.SettlementBuff);
            AddState(EActPhase.TurnStart);
            AddState(EActPhase.Operator);
            AddState(EActPhase.TurnEnd);

            // 初始化伤害链
            damageChain = DamageChainBuilder.GetRoleDamageChain();
            
            SkillFactory = GetSkillFactory();
            DefaultCastCondition = GetSkillCondition();
            DefaultTargetSelectStrategy = GetTargetSelectStrategy();
            
            // 添加组件
            AddComponents(TextUtility.Split(RoleInfo.f_comNames, 2));
        }
        
        protected abstract ISkillFactory GetSkillFactory();
        
        protected virtual ICastSkillCondition GetSkillCondition()
        {
            return castSkillConditionFactory.GetCastSkillCondition<PlayerDefaultCastSkillCondition>();
        }

        protected virtual ITargetSelectStrategy GetTargetSelectStrategy()
        {
            return targetSelectStrategyFactory.GetTargetSelectStrategy<PlayerBaseTargetSelectStrategy>();
        }
        
        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="eActPhase"></param>
        public void ChangeState(EActPhase eActPhase)
        {
            _currentState?.Exit();
            _currentState = _turnStates[eActPhase];
            _currentState.Enter();
        }
        
        /// <summary>
        /// 添加状态方法
        /// </summary>
        /// <param name="phase"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void AddState(EActPhase phase)
        {
            switch (phase)
            {
                case EActPhase.SettlementBuff:
                    _turnStates.TryAdd(EActPhase.SettlementBuff, DIContainer.Create<SettlementBuffState>(parameterValues: this));
                    break;
                case EActPhase.TurnStart:
                    _turnStates.TryAdd(EActPhase.TurnStart, DIContainer.Create<TurnStartState>(parameterValues: this));
                    break;
                case EActPhase.Operator:
                    _turnStates.TryAdd(EActPhase.Operator, DIContainer.Create<OperatorState>(parameterValues: this));
                    break;
                case EActPhase.TurnEnd:
                    _turnStates.TryAdd(EActPhase.TurnEnd, DIContainer.Create<TurnEndState>(parameterValues: this));
                    break;
                case EActPhase.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        public override void ExecuteAction()
        {
            base.ExecuteAction();
            ChangeState(EActPhase.SettlementBuff);
        }

        public override void CastSkill(int skillId)
        {
            var skillComponent = GetComponent<PlayerSkillComponent>();
            // 能否释放
            if (!skillComponent.CanCast(skillId))
            {
                return;
            }
            
            // 获取技能数据
            var skill = skillComponent.GetSkill(skillId);
            // 若是终结技，则重置标识
            if (skill.SkillContext.SkillInfo.f_SkillType == (byte)E_SkillType.UltimateSkill)
            {
                skillComponent.IsTrigger = true;
                skillComponent.IsRelease = false;
            }
            
            var skillCommand = commandfactory.GetSkillCommand(skill);
            // 发送指令
            Context.GetEventBus().TriggerEvent(new InsertCommandEvent(Context, skillCommand));
        }

        public void RecoverUltimate(int value)
        {
            var propertyComponent = GetComponent<PropertyComponent>();
            var current = propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentEnergy);
            var newValue = current + value;
            if (newValue > RoleInfo.f_maxEnergy)
            {
                newValue = RoleInfo.f_maxEnergy;
            }
            
            propertyComponent.SetPropertyValue(E_DynamicPropertyType.CurrentEnergy, newValue);
        }
    }
}
