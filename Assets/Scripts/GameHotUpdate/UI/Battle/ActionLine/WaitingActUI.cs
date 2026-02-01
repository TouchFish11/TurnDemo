using Core.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameHotUpdate.UI.Battle.ActionLine
{
    /// <summary>
    /// 等待行动UI
    /// </summary>
    public class WaitingActUI : BaseUIBehaviour
    {
        [Inject] private Image imgIcon;

        public void Init(Sprite icon)
        {
            imgIcon.sprite = icon;
        }
    }
}
