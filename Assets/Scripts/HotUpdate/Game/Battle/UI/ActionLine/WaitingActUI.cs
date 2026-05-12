using Core.DI;
using Core.UI;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Game.Battle.UI.ActionLine
{
    /// <summary>
    /// 等待行动UI
    /// </summary>
    public class WaitingActUI : UIBehaviourBase
    {
        [Inject] private Image imgIcon;

        public void Init(Sprite icon)
        {
            imgIcon.sprite = icon;
        }
    }
}
