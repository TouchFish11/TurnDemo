using Core.SO;
using UnityEngine;

namespace HotUpdate.Core.Main.Settings
{
    [CreateAssetMenu(fileName = "GameSettingsConfigSO", menuName = "Settings/GameSettingsConfigSO")]
    public class GameSettingsConfigSO : SOBase
    {
        public GameSettingsConfig SettingsConfig;

        private void OnValidate()
        {
            target = SettingsConfig;
        }
    }
}
