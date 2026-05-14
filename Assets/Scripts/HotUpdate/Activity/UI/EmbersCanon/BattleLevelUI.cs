using System;
using System.Collections.Generic;
using Core.DI;
using Core.GlobalEvent;
using Core.GlobalEvent.Events;
using Core.Scene;
using Core.UI;
using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Turn;
using HotUpdate.Base.Main;
using HotUpdate.Base.Scene;
using HotUpdate.Common;
using HotUpdate.Common.Config.Activity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = Core.Log.Logger;

namespace HotUpdate.Activity.UI.EmbersCanon
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 活动战斗关卡UI
    /// </summary>
    public class BattleLevelUI : UIBehaviourBase
    {
        [InjectUI] private Button btnEnter;
        [InjectUI] private TextMeshProUGUI txtName;
        [InjectUI] private Image imgIsFinished;
        
        [Inject] private IUIManager _uiManager;
        [Inject] private IBattleManager _battleManager;
        [Inject] private IEventCenter _eventCenter;
        [Inject] private ISceneManager _sceneManager;
        
        private bool _isComplete;
        
        // 只暴露事件，不处理业务逻辑
        public event Action OnEnterBattle;
        
        private BattleConfigEntry _configEntry;
        private EmbersCanonLevelEntryData levelDataEntryData;
        private int _activeControllerId;

        protected override void Awake()
        {
            DIContainer.InjectIntoInstance(this);
            base.Awake();
        }

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
                OnEnterBattle?.Invoke();
            else
                _eventCenter.TriggerEvent(new GlobalMessageEvent { Message = "该关卡已完成" });
        }
    }
}
