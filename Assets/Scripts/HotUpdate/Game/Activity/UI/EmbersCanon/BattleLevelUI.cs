using System;
using System.Collections.Generic;
using Core.DI;
using Core.GlobalEvent;
using Core.GlobalEvent.Events;
using Core.Scene;
using Core.UI;
using HotUpdate.Base.Activity;
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

namespace HotUpdate.Game.Activity.UI.EmbersCanon
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
        
        private BattleConfigEntry _configEntry;
        private IActivityData _activityData;
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
        /// <param name="levelDataEntryData"></param>
        /// <param name="configEntry"></param>
        /// <param name="activityData"></param>
        public void Init(string levelName, Sprite finishedIcon, EmbersCanonLevelEntryData levelDataEntryData, BattleConfigEntry configEntry, IActivityData activityData)
        {
            txtName.text = levelName;
            imgIsFinished.sprite = finishedIcon;
            this.levelDataEntryData =  levelDataEntryData;
            _configEntry = configEntry;
            _activityData = activityData;
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
                    SceneGeneratorHelper.GetSceneGenerator().ClearMainScene();
                    // 隐藏活动界面
                    await _uiManager.SetViewActive(_activeControllerId, false);
                }, async () =>
                {
                    await ChangedScene();
                    // 创建玩家对象（参数为玩家配置ID，对应玩家基础配置表）
                    await DIContainer.GetInstance<IPlayerManager>().CreatePlayer(1001);
                    // 更新当前关卡活动数据，标记为完成
                    levelDataEntryData.isComplete = true;
                    // 更新当前活动数据
                    _activityData.CurrentPro += 1;
                    // 激活活动界面
                    await _uiManager.SetViewActive(_activeControllerId, true);
                });
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(BattleLevelUI)}.{nameof(EnterBattle)}：{e.Message}");
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
            await SceneGeneratorHelper.GetSceneGenerator().InitMainScene();
        }
    }
}
