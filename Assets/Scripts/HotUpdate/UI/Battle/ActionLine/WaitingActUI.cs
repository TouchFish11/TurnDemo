using Core.UI;
using HotUpdate.Game.Battle.Object;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI.Battle.ActionLine
{
    /// <summary>
    /// 等待行动UI
    /// </summary>
    public class WaitingActUI : UIBehaviourBase
    {
        [InjectUI] private Image imgIcon;
        
        public IBattleEntityObject BattleEntity { get; private set; }
        
        public int Priority { get; private set; }

        /// <summary>
        /// 初始化图标
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="battleEntity"></param>
        /// <param name="Priority"></param>
        public void Init(Sprite icon, IBattleEntityObject battleEntity, int Priority)
        {
            imgIcon.sprite = icon;
            BattleEntity = battleEntity;
            this.Priority = Priority;
        }
    }
}
