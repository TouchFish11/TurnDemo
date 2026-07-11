using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Pool;
using Core.PreLoad;
using Core.Serialize.Json;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Scene;
using HotUpdate.Base.Service;
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
        [Inject] private ISceneGenerator _sceneGenerator;
        [Inject] private IUIService _uiService;
        [Inject] private IPlayerManager _playerManager;
        [Inject] private IJsonManager _jsonManager;
        [Inject] private IActivityDataManager _activityDataManager;
        [Inject] private ObjectSpawner _objectSpawner;
        [Inject] private IIconService _iconService;
        [Inject] private IPoolManager _poolManager;
        
        /// <summary>
        /// 初始化关卡
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public async Task<(BattleConfigEntryColletion battleConfigEntryColletion, EmbersCanonData embersCanonData)> InitLevels(int activityId)
        {
            // 根据读取用户活动数据
            var activityDataCollection = _activityDataManager.ActivityDataCollection as ActivityDataCollection;
            // 获取该活动数据
            var embersCanonData = activityDataCollection[activityId] as EmbersCanonData;
            // 解析该活动的关卡配置
            using var handle = await GameAsset.LoadAssetAsync<TextAsset>(AssetKeys.BattleActivityConfig);
            var battleConfigEntryColletion = _jsonManager.FromJson<BattleConfigEntryColletion>(handle.Asset.text, settings: NewtonsoftJsonUtility.SerializerSettings);

            foreach (var battleConfigEntry in battleConfigEntryColletion.battleConfigs)
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

            return (battleConfigEntryColletion, embersCanonData);
        }
        
        /// <summary>
        /// 进入战斗活动
        /// </summary>
        /// <param name="configEntry"></param>
        /// <param name="activityId"></param>
        /// <param name="onLevelComplete"></param>
        public async Task EnterActivityBattle(BattleConfigEntry configEntry, int activityId, Action onLevelComplete)
        {
            var waveDatas = new List<WaveData>
            {
                // 测试数据
                new(waveId: 1, victoryConditionType: EWaveVictoryConditionType.EliminateAllEnemies, monsterIds: configEntry.monsterIds),
            };

            var battleStartupParams= new BattleStartupParams
            {
                WaveDatas = waveDatas,
                OnPreEnter = async () =>
                {
                    _sceneGenerator.ClearMainScene();
                    await _uiService.CloseAsync(_uiService.GetPanel(EUIPanelId.ActivityPanel).PanelId, false);
                    await PreLoad();
                },
                OnBattleOver = async result =>
                {
                    BattleEntry.EndBattle();
                    _poolManager.ClearAll();
                    // TODO:待处理
                    await _sceneGenerator.InitMainScene(-1);
                    await _playerManager.CreatePlayer(1001);

                    if (result.IsWin)
                    {
                        onLevelComplete?.Invoke();
                        if(_activityDataManager.TryGetData(activityId, out var activityData))
                            activityData.CurrentPro += 1;
                    }
                    
                    await _uiService.ShowAsync(_uiService.GetPanel(EUIPanelId.ActivityPanel).PanelId);
                }
            };

            await BattleEntry.StartBattle(battleStartupParams);
        }
        
        /// <summary>
        /// 战斗资源预加载
        /// </summary>
        private async Task PreLoad()
        {
            // TODO：暂时写死，可优化为配置
            var preLoadDatas = new PreLoadData[]
            {
                // GameObject
                new(AssetKeys.Prefab_Warrior),
                new(AssetKeys.Prefab_Wizard),
                new(AssetKeys.Prefab_Slime),
                new(AssetKeys.Prefab_TurtleShell),
                new(AssetKeys.Prefab_TurtleShell),
                
                // UI
                new(AssetKeys.SelectMarkerUI),
                new(AssetKeys.MonsterStateUI),
                new(AssetKeys.RoleStateUI),
                new(AssetKeys.ActionGridUI),
                new(AssetKeys.WaitingActUI),
                new(AssetKeys.SkillKeyUI),
            };
            
            await _objectSpawner.PreLoadAsync(preLoadDatas);
            
            // 图集预加载
            await _iconService.PreLoadAtlasAsync(
                AssetKeys.Atlas_Icon_BattleEntity,
                AssetKeys.Atlas_Icon_Common,
                AssetKeys.Atlas_Default);
        }
    }
}
