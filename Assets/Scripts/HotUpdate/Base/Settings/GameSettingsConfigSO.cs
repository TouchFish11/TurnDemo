using Core.SO;
using UnityEngine;

namespace HotUpdate.Base.Settings
{
    [CreateAssetMenu(fileName = "GameSettingsConfigSO", menuName = "Settings/GameSettingsConfigSO")]
    public class GameSettingsConfigSO : SOBase
    {
        public GameSettingsConfig SettingsConfig;

        private void OnValidate()
        {
            target = SettingsConfig;
        }

        protected override void OnAwake()
        {
            
        }
    }
}
