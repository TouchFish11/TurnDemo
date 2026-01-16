using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 怪物对象
    /// </summary>
    public class MonsterObject : BattleObject
    {
        /// <summary>
        /// 怪物信息
        /// </summary>
        public MonsterInfo MonsterInfo { get; private set; }

        private readonly List<int> skillIds = new List<int>();

        public override void BaseInit(int id)
        {
            base.BaseInit(id);
            MonsterInfo = BinaryDataManager.Instance.GetConfig<MonsterInfoContainer>(E_ConfigLoadType.Editor).dataDic[id];
        }

        public override void BattleInit(int monsterId, IBattleContext context)
        {
            base.BattleInit(monsterId, context);

            // 初始化技能列表
            skillIds.AddRange(TextUtility.SplitToIntArr(MonsterInfo.f_skillIds, 2));
            // 添加战斗相关组件
            AddComponents(TextUtility.Split(MonsterInfo.f_comNames, 2));
        }

        protected override void OnPreTakeDamage(DamageResult damageResult)
        {
            // 削减韧性
            this.GetComponent<ToughnessComponent>().ReduceToughness(damageResult.Source, damageResult.ElementType, damageResult.SkillInfo);
        }

        /// <summary>
        /// 怪物行动协程
        /// 通过怪物自身对象开启协程，不能用mono管理器开始，因为怪物会被销毁，导致韧性恢复后若死亡，仍会执行后续释放技能逻辑导致报错
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator OnExceuteAction()
        {
            // 恢复韧性
            yield return RestoreToughness();

            // 怪物AI逻辑，随机选择一个技能释放
            int skillId = skillIds[Random.Range(0, skillIds.Count)];
            CastSkill(skillId);
        }

        /// <summary>
        /// 恢复韧性
        /// </summary>
        /// <returns></returns>
        private IEnumerator RestoreToughness()
        {
            ToughnessComponent toughnessComponent = this.GetComponent<ToughnessComponent>();
            // 击破状态下才能恢复
            if (!toughnessComponent.IsToughnessBroken())
            {
                yield break;
            }
            // 封装命令
            ToughnessCommand command = ServiceLocator.Get<IFactoryManager>().GetFactory<CommandFactory>().GetToughnessCommand(toughnessComponent);
            // 放入命令
            ServiceLocator.Get<IBattleManager>().GetContext().GetTurnManager().InsertCommand(command);
            // 等待恢复完成
            yield return new WaitUntil(() => toughnessComponent.CurrentToughnessValue == toughnessComponent.MaxToughnessVaue);
        }

        public override IEnumerator Die()
        {
            var vFXInfo = new VFXInfo();
            // 显示死亡特效
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_MonsterDead, new ProjectileTrans(this.transform, false), new ProjectileData(this, null, null, null), vFXInfo);
            // 怪物播放死亡动画
            AnimationPlayManager.Instance.PlayAnimationOver(this.GetComponent<BattleAnimationComponent>(), AnimationComponent.Battle_Layer_Name, E_AnimationType.Death);
            yield return new WaitUntil(() => !vFXInfo.IsAlive);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            // 移除状态UI
            Context.GetEventBus().TriggerEvent(new MonsterDeadEvent(Context, this));
        }
    }
}
