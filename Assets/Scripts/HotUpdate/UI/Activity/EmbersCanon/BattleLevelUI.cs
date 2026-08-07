using System;
using Core.DI;
using Core.GlobalEvent;
using Core.GlobalEvent.Events;
using Core.UI;
using HotUpdate.Common.Config.Activity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI.Activity.EmbersCanon
{
    /// <summary>
    /// 活动战斗关卡UI
    /// </summary>
    public class BattleLevelUI : UIBehaviourBase
    {
        [InjectUI] private Button btnEnter;
        [InjectUI] private TextMeshProUGUI txtName;
        [InjectUI] private Image imgIsFinished;

        [Inject] private IEventCenter _eventCenter;
        
        private bool _isComplete;
        
        public event Action OnEnterBattle;
        
        private BattleConfigEntry _configEntry;
        private EmbersCanonLevelEntryData levelDataEntryData;
        private int _activeControllerId;
        
        /// <summary>
        /// 初始化UI
        /// </summary>
        /// <param name="levelName"></param>
        /// <param name="finishedIcon"></param>
        /// <param name="isComplete"></param>
        public void Init(string levelName, Sprite finishedIcon, bool isComplete)
        {
            txtName.text = levelName;
            imgIsFinished.sprite = finishedIcon;
            _isComplete = isComplete;
        }

        protected override void OnButtonClick(string btnName)
        {
            if (!_isComplete)
            {
                OnEnterBattle?.Invoke();
            }
            else
            {
                var globalMessageEvent = EventSource.Get<GlobalMessageEvent>();
                globalMessageEvent.Message = "该关卡已完成";
                _eventCenter.TriggerEvent(globalMessageEvent);
            }
        }

        protected override void OnDisable()
        {
            OnEnterBattle = null;
        }
    }
}
