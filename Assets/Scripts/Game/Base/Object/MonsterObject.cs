using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    public class MonsterObject : BattleObject
    {
        /// <summary>
        /// 怪物信息
        /// </summary>
        public MonsterInfo MonsterInfo { get; private set; }

        public override void BaseInit(int id)
        {
            base.BaseInit(id);

            MonsterInfo = BinaryDataMgr.Instance.GetConfig<MonsterInfoContainer>(E_ConfigLoadType.Editor).dataDic[id];
            // 初始化与战斗无关的属性
            Name = MonsterInfo.f_name;
        }

        public override void BattleInit(int monsterId, IBattleContext context)
        {
            base.BattleInit(monsterId, context);
            // 添加战斗相关组件
            AddComponents(TextUtility.SplitToIntArr(MonsterInfo.f_comIds, 2));
        }

        public override IEnumerator ExecuteAction()
        {
            throw new System.NotImplementedException();
        }

        public override int GetSpeed()
        {
            throw new System.NotImplementedException();
        }
    }
}
