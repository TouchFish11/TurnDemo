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
            AddComponents(TextUtility.SplitToIntArr(MonsterInfo.f_comIds, 2));
        }

        protected override IEnumerator OnExceuteAction()
        {
            // 怪物AI逻辑，随机选择一个技能释放
            int skillId = skillIds[Random.Range(0, skillIds.Count)];
            // 模拟怪物行动的延迟
            yield return new WaitForSeconds(1.0f);
            CastSkill(skillId);
        }

        public override int GetSpeed()
        {
            throw new System.NotImplementedException();
        }
    }
}
