using System;
using System.Collections;
using System.Collections.Generic;
using Core.Reflection;
using Core.Serialize.Binary;
using Core.Service;
using Core.Utility;
using HotUpdate.Animation;
using HotUpdate.Animation.Component;
using HotUpdate.Battle.Command;
using HotUpdate.Battle.Context;
using HotUpdate.Battle.Event.Turn;
using HotUpdate.Battle.Object.StateMeachine;
using HotUpdate.Battle.ResponsibilityChain.DamageChain;
using HotUpdate.Battle.Skill.Component;
using HotUpdate.Battle.Skill.Enum;

namespace HotUpdate.Battle.Object
{
    /// <summary>
    /// 角色对象
    /// </summary>
    public abstract class PlayerObject : BattleObject
    {
        private readonly Dictionary<EActPhase, ITurnState> _turnStates = new();
        private ITurnState _currentState;
        
        /// <summary>
        /// 角色信息
        /// </summary>
        public RoleInfo RoleInfo { get; private set; }
        
        public override void BaseInit(int id)
        {
            base.BaseInit(id);
            RoleInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<RoleInfoContainer>(EConfigLoadType.Excel).dataDic[id];
        }

        public override void BattleInit(int battleEntityId, IBattleContext context)
        {
            base.BattleInit(battleEntityId, context);
            AddState(EActPhase.SettlementBuff);
            AddState(EActPhase.TurnStart);
            AddState(EActPhase.Operator);
            AddState(EActPhase.TurnEnd);
            // 添加组件
            AddComponents(TextUtility.Split(RoleInfo.f_comNames, 2));
            // 初始化伤害链
            damageChain = DamageChainBuilder.GetRoleDamageChain();
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
                    _turnStates.TryAdd(EActPhase.SettlementBuff, new SettlementBuffState(this));
                    break;
                case EActPhase.TurnStart:
                    _turnStates.TryAdd(EActPhase.TurnStart, new TurnStartState(this));
                    break;
                case EActPhase.Operator:
                    _turnStates.TryAdd(EActPhase.Operator, new OperatorState(this));
                    break;
                case EActPhase.TurnEnd:
                    _turnStates.TryAdd(EActPhase.TurnEnd, new TurnEndState(this));
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
            var skillData = skillComponent.GetSkillData(skillId);
            // 若是终结技，则重置标识
            if (skillData.Skill.SkillInfo.f_SkillType == (byte)E_SkillType.UltimateSkill)
            {
                skillComponent.IsRelease = false;
            }
            
            var skillCommand = ServiceLocator.Get<IFactoryManager>().GetFactory<ICommandFactory, CommandFactory>()
                .GetSkillCommand(skillData);
            // 发送指令
            Context.GetEventBus().TriggerEvent(new InsertCommandEvent(Context, skillCommand));
        }

        public override IEnumerator Die()
        {
            // 
            yield return ServiceLocator.Get<IAnimationPlayManager>().WaitForAnimOver(GetComponent<BattleAnimationComponent>(), AnimationComponent.Battle_Layer_Name, E_AnimationType.Death);
        }
    }
}
