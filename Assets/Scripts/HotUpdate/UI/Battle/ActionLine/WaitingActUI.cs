using Core.UI;
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
        
        /// <summary>
        /// 实体对象ID
        /// </summary>
        public int BattleEntityId { get; private set; }

        /// <summary>
        /// 初始化图标
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="battleEntityId"></param>
        public void Init(Sprite icon, int battleEntityId)
        {
            imgIcon.sprite = icon;
            BattleEntityId = battleEntityId;
        }
    }
}
