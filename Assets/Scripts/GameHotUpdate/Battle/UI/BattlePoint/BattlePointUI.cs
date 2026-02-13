using Core.UI;
using UnityEngine.UI;

namespace GameHotUpdate.Battle.UI.BattlePoint
{
    /// <summary>
    /// ս����UI
    /// </summary>
    public class BattlePointUI : UIBehaviourBase
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
