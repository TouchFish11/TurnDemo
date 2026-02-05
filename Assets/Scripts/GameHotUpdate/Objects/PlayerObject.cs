using System.Collections;
using Core.DataPersistence.Binary;
using Core.Service;
using Core.Utility;
using Game.Animation;
using Game.Battle.Context;
using Game.Battle.Objects;
using GameHotUpdate.Animation;
using GameHotUpdate.Battle.ResponsibilityChain.DamageChain;

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
            RoleInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<RoleInfoContainer>(EConfigLoadType.Editor).dataDic[id];
        }

        public override void BattleInit(int battleEntityId, IBattleContext context)
        {
            base.BattleInit(battleEntityId, context);
            
            // 添加组件
            AddComponents(TextUtility.Split(RoleInfo.f_comNames, 2));
            // 初始化伤害链
            damageChain = DamageChainBuilder.GetRolrDamageChain();
            // 添加状态
            AddState(EActPhase.SettlementBuff);
            AddState(EActPhase.TurnStart);
            AddState(EActPhase.Operator);
            AddState(EActPhase.TurnEnd);
        }
        
        public override IEnumerator Die()
        {
            // 
            yield return ServiceLocator.Get<IAnimationPlayManager>().WaitForAnimOver(GetComponent<BattleAnimationComponent>(), AnimationComponent.Battle_Layer_Name, E_AnimationType.Death);
        }
    }
}
