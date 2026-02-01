using System.Collections.Generic;
using Game.Battle.Context;
using Game.Battle.Event;
using Game.Battle.Objects;

namespace GameHotUpdate.Battle.Event.UI
{
    /// <summary>
    /// ѡ��Ŀ���¼�
    /// </summary>
    public class SelectTargetEvent : BattleEvent
    {
        /// <summary>
        /// Ŀ���б���������Ŀ�꣩
        /// </summary>
        public List<IBattleEntityObject> SelectedTargets { get; private set; }
        /// <summary>
        /// ��Ŀ��
        /// </summary>
        public IBattleEntityObject MainTarget { get; private set; }


        public SelectTargetEvent(IBattleContext context, IBattleEntityObject mainTarget, List<IBattleEntityObject> selectedTargets) : base(context)
        {
            MainTarget = mainTarget;
            SelectedTargets = selectedTargets;
        }
    }
}
