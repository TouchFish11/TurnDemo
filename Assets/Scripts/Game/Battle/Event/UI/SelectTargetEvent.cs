using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 选择目标事件
    /// </summary>
    public class SelectTargetEvent : BattleEvent
    {
        /// <summary>
        /// 目标列表（包含主目标）
        /// </summary>
        public List<IBattleEntityObject> SelectedTargets { get; private set; }
        /// <summary>
        /// 主目标
        /// </summary>
        public IBattleEntityObject MainTarget { get; private set; }


        public SelectTargetEvent(IBattleContext context, IBattleEntityObject mainTarget, List<IBattleEntityObject> selectedTargets) : base(context)
        {
            this.MainTarget = mainTarget;
            this.SelectedTargets = selectedTargets;
        }
    }
}
