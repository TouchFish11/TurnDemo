using Config.ActivityConfigSO;
using Core.Log;
using Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameHotUpdate.Activity.UI.EmbersCanon
{
    /// <summary>
    /// 活动战斗关卡UI
    /// </summary>
    public class BattleLevelUI : UIBehaviourBase
    {
        [Inject] private Button btnEnter;
        [Inject] private TextMeshProUGUI txtName;
        [Inject] private Image imgIsFinished;
        
        private BattleConfigEntry _configEntry;

        /// <summary>
        /// 初始化UI
        /// </summary>
        /// <param name="levelName"></param>
        /// <param name="finishedIcon"></param>
        /// <param name="configEntry"></param>
        public void Init(string levelName, Sprite finishedIcon, BattleConfigEntry configEntry)
        {
            txtName.text = levelName;
            imgIsFinished.sprite = finishedIcon;
            _configEntry = configEntry;
        }

        protected override void OnButtonClick(string btnName)
        {
            switch (btnName)
            {
                case nameof(btnEnter):
                    // 进入战斗
                    LogManager.Log($"进入战斗，{_configEntry.levelName}");
                break;
            }
        }
    }
}
