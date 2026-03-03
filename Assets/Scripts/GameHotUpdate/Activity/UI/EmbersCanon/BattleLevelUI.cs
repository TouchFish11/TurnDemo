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
        private EmbersCanonLevelEntryData levelDataEntryData;

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
        /// <param name="levelDataEntryData"></param>
        /// <param name="configEntry"></param>
        public void Init(string levelName, Sprite finishedIcon, EmbersCanonLevelEntryData levelDataEntryData, BattleConfigEntry configEntry)
        {
            txtName.text = levelName;
            imgIsFinished.sprite = finishedIcon;
            this.levelDataEntryData =  levelDataEntryData;
            _configEntry = configEntry;
        }

        protected override void OnButtonClick(string btnName)
        {
            switch (btnName)
            {
                case nameof(btnEnter):
                if (!levelDataEntryData.isComplete)
                {
                    EnterBattle();
                }
                else
                {
                    _eventCenter.TriggerEvent(new GlobalMessageEvent {Message = "该关卡已完成"});
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
                await _battleManager.EnterBattle(turnData, async () =>
                {
                    // 清理场景内容缓存
                    HotfixGameMain.ClearScene();
                    // 隐藏活动界面
                    await _uiManager.SetViewActive(_uiManager.GetController<ActivityController>(), false);
                }, async () =>
                {
                    await ChangedScene();
                    // 更新当前活动数据，标记为完成
                    levelDataEntryData.isComplete = true;
                    // 激活活动界面
                    await _uiManager.SetViewActive(_uiManager.GetController<ActivityController>(), true);
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
        private async Task ChangedScene()
        {
            // 切换到指定场景场景
            await _sceneManager.LoadSceneAsync(ResKeyCollection.MainScene, UnityEngine.SceneManagement.LoadSceneMode.Single, null);
            await HotfixGameMain.InitScene();
        }
    }
}
