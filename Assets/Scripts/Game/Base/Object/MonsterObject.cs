using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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

        protected override IEnumerator OnExceuteAction()
        {
            // 恢复韧性
            yield return RestoreToughness();

            // 怪物AI逻辑，随机选择一个技能释放
            int skillId = skillIds[Random.Range(0, skillIds.Count)];
            // 触发技能选择事件，更新目标管理器的缓存目标内容，释放技能时能获取到这些内容
            Context.GetEventBus().TriggerEvent(new SelectSkillEvent(Context, skillId, this));
            // 更新相关UI
            var target = ServiceLocator.Get<ITargetSelectManager>().GetMainTarget();
            LogManager.Log($"怪物目标：{target}");
            BattleUIScheduler.Instance.UpdateCameraAndHideMarkerAndMonsterUI(Context, target);
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

        protected override void Die()
        {
            base.Die();

            // 怪物仅改变标识，当前指令结束后，再统一移除
        }
    }
}
