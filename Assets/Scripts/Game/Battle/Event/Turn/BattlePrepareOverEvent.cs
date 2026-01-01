using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 战斗准备完毕事件
    /// </summary>
    public class BattlePrepareOverEvent : BattleEvent
    {
        public IBattleEntityObject GoFirstBattleEntity { get; private set; }

        public BattlePrepareOverEvent(IBattleContext context, IBattleEntityObject goFirstBattleEntity) : base(context)
        {
            GoFirstBattleEntity = goFirstBattleEntity;
        }
    }
}
