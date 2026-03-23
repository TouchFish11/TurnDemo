using Test.Config;
using UnityEngine;

namespace Test.SO
{
    /// <summary>
    /// 头盔SO
    /// </summary>
    [CreateAssetMenu(fileName = "HelmetSO", menuName = "Equipment/HelmetSO")]
    public class HelmetSO : EquipmentSO
    {
        public HelmetConfig HelmetConfig;

        private void Awake()
        {
            target = HelmetConfig;
        }
    }
}
