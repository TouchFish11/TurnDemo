using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 行动轴排序后事件
    /// 战斗界面更新行动轴显示
    /// </summary>
    public class ActionBarSortPostEvent : BattleEvent
    {
        public IEnumerable<IBattleEntityObject> battleEntities { get; }

        public ActionBarSortPostEvent(IBattleContext context, IEnumerable<IBattleEntityObject> battleEntities) : base(context)
        {
            this.battleEntities = battleEntities;
        }
    }
}
