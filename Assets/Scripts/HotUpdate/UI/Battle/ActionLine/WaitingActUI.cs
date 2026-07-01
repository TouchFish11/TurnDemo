using Core.DI;
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

        public void Init(Sprite icon)
        {
            imgIcon.sprite = icon;
        }
    }
}
