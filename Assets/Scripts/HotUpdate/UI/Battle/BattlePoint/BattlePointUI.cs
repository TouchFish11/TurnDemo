using Core.UI;
using UnityEngine.UI;

namespace HotUpdate.UI.Battle.BattlePoint
{
    /// <summary>
    /// 战技点UI对象
    /// </summary>
    public class BattlePointUI : UIBehaviourBase
    {
        [InjectUI] private Image imgHas;

        /// <summary>
        /// 设置战技点是否激活
        /// </summary>
        /// <param name="active"></param>
        public void SetActivePoint(bool active)
        {
            imgHas.gameObject.SetActive(active);
        }
    }
}
