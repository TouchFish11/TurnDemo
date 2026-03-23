using Test.Config;
using UnityEngine;

namespace Test.SO
{
    /// <summary>
    /// 武器ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponSO", menuName = "Equipment/WeaponSO")]
    public class WeaponSO : EquipmentSO
    {
        public WeaponConfig weaponConfig;

        private void OnValidate()
        {
            target = weaponConfig;
        }
    }
}
