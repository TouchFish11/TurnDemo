using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 状态被添加事件
    /// 用于显示界面状态（Buff）文本
    /// </summary>
    public class StatusAddedEvent : BattleEvent
    {
        public IStatus NewStatus { get; }

        public StatusAddedEvent(IBattleContext context, IStatus newStatus) : base(context)
        {
            NewStatus = newStatus;
        }
    }
}
