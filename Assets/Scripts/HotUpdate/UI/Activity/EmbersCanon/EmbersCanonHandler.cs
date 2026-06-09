using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Serialize.Json;
using Core.Utility;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Scene;
using HotUpdate.Base.UI;
using HotUpdate.Common.Config.Activity;
using HotUpdate.Game.Activity.Core;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Turn;
using HotUpdate.UI.Activity.Base;
using UnityEngine;

namespace HotUpdate.UI.Activity.EmbersCanon
{
    public class EmbersCanonHandler : ActivityContentHandler<EmbersCanonActivityUI>
    {
        [Inject] private IBattleManager _battleManager;
        [Inject] private ISceneGenerator _sceneGenerator;
        [Inject] private IUIService _uiService;
        [Inject] private IPlayerManager _playerManager;
        [Inject] private IJsonManager _jsonManager;
        [Inject] private IActivityDataManager _activityDataManager;

        /// <summary>
        /// 初始化关卡
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public async Task<(BattleActivityConfig battleActivityConfig, EmbersCanonData embersCanonData)> InitLevels(int activityId)
        {
            // 根据读取用户活动数据
            var activityDataCollection = _activityDataManager.ActivityDataCollection as ActivityDataCollection;
            // 获取该活动数据
            var embersCanonData = activityDataCollection[activityId] as EmbersCanonData;
            // 解析该活动的关卡配置
            using var handle = await GameAsset.LoadAssetAsync<TextAsset>(AssetKeys.BattleActivityConfig);
            var battleActivityConfig = _jsonManager.FromJson<BattleActivityConfig>(handle.Asset.text, settings: NewtonsoftJsonUtility.SerializerSettings);

            foreach (var battleConfigEntry in battleActivityConfig.BattleConfigEntryColletion.battleConfigs)
            {
                // 获取用户数据中的战斗关卡条目
                var levelEntryData = embersCanonData.GetLevelData(battleConfigEntry.levelId);
                // 新增数据
                if (levelEntryData == null)
                {
                    levelEntryData = new EmbersCanonLevelEntryData { levelId = battleConfigEntry.levelId, isComplete = false };
                    embersCanonData.Add(levelEntryData);
                }
            }

            return (battleActivityConfig, embersCanonData);
        }
        
        /// <summary>
        /// 进入战斗活动
        /// </summary>
        /// <param name="configEntry"></param>
        /// <param name="activityId"></param>
        /// <param name="onLevelComplete"></param>
        public async Task EnterActivityBattle(BattleConfigEntry configEntry, int activityId, Action onLevelComplete)
        {
            var turnData = new TurnData
            {
                TotalTurnNumber = configEntry.battleWave,
                Waves = new List<List<int>> { configEntry.monsterIds }
            };

            await _battleManager.EnterBattle(turnData,
                OnPreEnter: async () =>
                {
                    _sceneGenerator.ClearMainScene();
                    await _uiService.CloseAsync(_uiService.GetPanel(EUIPanelId.ActivityPanel).PanelId, false);
                },
                onBattleOver: async () =>
                {
                    // TODO:待处理
                    await _sceneGenerator.InitMainScene(-1);
                    await _playerManager.CreatePlayer(1001);
                    onLevelComplete?.Invoke();
                    if(_activityDataManager.TryGetData(activityId, out var activityData))
                        activityData.CurrentPro += 1;
                    await _uiService.ShowAsync(_uiService.GetPanel(EUIPanelId.ActivityPanel).PanelId);
                });
        }
    }
}
