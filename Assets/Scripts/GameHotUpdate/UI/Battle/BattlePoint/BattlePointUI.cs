using Core.UI;
using UnityEngine.UI;

namespace GameHotUpdate.UI.Battle.BattlePoint
{
    /// <summary>
    /// ս����UI
    /// </summary>
    public class BattlePointUI : BaseUIBehaviour
    {
        [Inject] private Image imgHas;

        /// <summary>
        /// ���õ�״̬
        /// </summary>
        /// <param name="active"></param>
        public void SetActivePoint(bool active)
        {
            imgHas.gameObject.SetActive(active);
        }
    }
}
