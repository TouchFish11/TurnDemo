using System.Collections;
using Core.DataPersistence.Binary;
using Core.Reflection;
using Core.Service;
using Core.Utility;
using Game.Animation;
using Game.Battle.Command;
using Game.Battle.Context;
using Game.Battle.Skill.Enum;
using GameHotUpdate.Animation;
using GameHotUpdate.Battle.Command;
using GameHotUpdate.Battle.Event.Turn;
using GameHotUpdate.Battle.ResponsibilityChain.DamageChain;
using GameHotUpdate.Skill.Component;

namespace GameHotUpdate.Objects
{
    /// <summary>
    /// 角色对象
    /// </summary>
    public abstract class PlayerObject : BattleObject
    {
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
            
            // 添加组件
            AddComponents(TextUtility.Split(RoleInfo.f_comNames, 2));
            // 初始化伤害链
            damageChain = DamageChainBuilder.GetRolrDamageChain();
        }

        public override void CastSkill(int skillId)
        {
            var skillComponent = this.GetComponent<PlayerSkillComponent>();
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
