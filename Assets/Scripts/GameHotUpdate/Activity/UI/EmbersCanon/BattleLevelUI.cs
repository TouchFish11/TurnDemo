using System;
using System.Collections.Generic;
using ConfigHotUpdate;
using Core.GlobalEvent;
using Core.GlobalEvent.Events;
using Core.Log;
using Core.Scene;
using Core.Service;
using Core.UI;
using GameHotUpdate.Activity.UI.Base;
using GameHotUpdate.Battle.Core;
using GameHotUpdate.Battle.Turn;
using GameHotUpdate.Config;
using GameHotUpdate.Main;
using GameHotUpdate.Main.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameHotUpdate.Activity.UI.EmbersCanon
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 活动战斗关卡UI
    /// </summary>
    public class BattleLevelUI : UIBehaviourBase
    {
        [Inject] private Button btnEnter;
        [Inject] private TextMeshProUGUI txtName;
        [Inject] private Image imgIsFinished;
        
        private IUIManager _uiManager;
        private IBattleManager _battleManager;
        private IEventCenter _eventCenter;
        private ISceneManager _sceneManager;
        
        private BattleConfigEntry _configEntry;
        private EmbersCanonLevelEntry _levelDataEntry;

        protected override void Awake()
        {
            _uiManager = ServiceLocator.Get<IUIManager>();
            _battleManager = ServiceLocator.Get<IBattleManager>();
            _eventCenter = ServiceLocator.Get<IEventCenter>();
            _sceneManager = ServiceLocator.Get<ISceneManager>();
            base.Awake();
        }

        /// <summary>
        /// 初始化UI
        /// </summary>
        /// <param name="levelName"></param>
        /// <param name="finishedIcon"></param>
        /// <param name="levelDataEntry"></param>
        /// <param name="configEntry"></param>
        public void Init(string levelName, Sprite finishedIcon, EmbersCanonLevelEntry levelDataEntry, BattleConfigEntry configEntry)
        {
            txtName.text = levelName;
            imgIsFinished.sprite = finishedIcon;
            _levelDataEntry =  levelDataEntry;
            _configEntry = configEntry;
        }

        protected override void OnButtonClick(string btnName)
        {
            switch (btnName)
            {
                case nameof(btnEnter):
                if (!_levelDataEntry.isComplete)
                {
                    EnterBattle();
                }
                else
                {
                    _eventCenter.TriggerEvent(new GlobalMessageEvent("该关卡已完成"));
                }
                break;
            }
        }

        /// <summary>
        /// 进入战斗
        /// </summary>
        private async void EnterBattle()
        {
            try
            {
                var turnData = new TurnData
                {
                    TotalTurnNumber = _configEntry.battleWave,
                    Waves = new List<List<int>>
                    {
                        _configEntry.monsterIds
                    }
                };
            
                // 通过BattleManager启动战斗，传入当前控制器上下文
                await _battleManager.EnterBattle(turnData, () =>
                {
                    // 清理场景内容缓存
                    HotfixGameMain.ClearScene();
                    // 隐藏活动界面
                    _uiManager.SetViewActive(_uiManager.GetController<ActivityController>(), false);
                    return Task.CompletedTask;
                }, async () =>
                {
                    await ChangedScene();
                    // 显示主界面
                    _uiManager.SetViewActive(_uiManager.GetController<MainController>(), true);
                    // 更新当前活动数据，标记为完成
                    _levelDataEntry.isComplete = true;
                    // 激活活动界面
                    _uiManager.SetViewActive(_uiManager.GetController<ActivityController>(), true);
                });
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(BattleLevelUI)}.{nameof(EnterBattle)}：{e.Message}，{e.StackTrace}");
            }
        }

        /// <summary>
        /// 切换场景
        /// </summary>
        /// <returns></returns>
        private Task ChangedScene()
        {
            // 切换到指定场景场景
            return _sceneManager.LoadSceneAsync(ResKeyCollection.MainScene, UnityEngine.SceneManagement.LoadSceneMode.Single, null);
        }
    }
}
