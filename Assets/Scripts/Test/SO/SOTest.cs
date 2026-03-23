using Core.Serialize.Json;
using Test.Config;
using UnityEngine;

namespace Test.SO
{
    public class SOTest : MonoBehaviour
    {
        public TextAsset so;
    
        // Start is called before the first frame update
        void Start()
        {
            var weaponConfig = JsonManager.Instance.FromJson<WeaponConfig>(so.text, settings: Core.Utility.NewtonsoftJsonUtility.SerializerSettings);
            Debug.Log($"{weaponConfig.id}，{weaponConfig.name},{weaponConfig.description}");
            foreach (var weaponConfigBonusData in weaponConfig.bonusDatas)
            {
                Debug.Log($"{weaponConfigBonusData.StatType},{weaponConfigBonusData.BuildValue},{weaponConfigBonusData.PercentValue}");
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
