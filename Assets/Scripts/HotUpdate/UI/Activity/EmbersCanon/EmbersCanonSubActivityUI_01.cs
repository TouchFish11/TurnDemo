using System.Collections.Generic;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Serialize.Json;
using Core.UI;
using Core.Utility;
using HotUpdate.Base.Manager;
using HotUpdate.Common;
using HotUpdate.Common.Config.Activity;
using HotUpdate.Game.Activity.Core;
using HotUpdate.UI.Activity.Base;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI.Activity.EmbersCanon
{
    using Task = System.Threading.Tasks.Task;
    
    /// <summary>
    /// 余烬圣典子界面UI01
    /// </summary>
    public class EmbersCanonSubActivityUI_01 : ActivityUIBehaviourBase
    {
        [Inject] private IActivityDataManager _activityDataManager;
        [Inject] private IUIManager _uiManager;
        
        [InjectUI] private ScrollRect svLevel;

        private readonly IList<PoolObject<BattleLevelUI>> _battleLevelUis = new List<PoolObject<BattleLevelUI>>();
        
        protected override async Task OnShow()
        {
            // 根据读取用户活动数据
            var activityDataCollection = _activityDataManager.ActivityDataCollection as ActivityDataCollection;
            // 获取该活动数据
            var embersCanonData = activityDataCollection[activityInfo.f_id] as EmbersCanonData;
            using var handle = await GameAsset.LoadAssetAsync<TextAsset>(ResKeyCollection.BattleActivityConfig);
            // 解析该活动的关卡配置
            var battleActivityConfig = DIContainer.GetInstance<IJsonManager>().FromJson<BattleActivityConfig>(handle.Asset.text, settings: NewtonsoftJsonUtility.SerializerSettings);
            // 初始化关卡
            foreach (var battleConfigEntry in battleActivityConfig.BattleConfigEntryColletion.battleConfigs)
            {
                var battleLevelUI = await _objectSpawner.SpawnAsync<BattleLevelUI>(ResKeyCollection.BattleLevelUI, svLevel.content);
                // 获取用户数据中的战斗关卡条目
                var levelEntryData = embersCanonData.GetLevelData(battleConfigEntry.levelId);
                // 新增数据
                if (levelEntryData == null)
                {
                    levelEntryData = new EmbersCanonLevelEntryData { levelId = battleConfigEntry.levelId, isComplete = false };
                    embersCanonData.Add(levelEntryData);
                }

                // 根据是否完成使用不同的Sprite
                var levelTipIconRes = levelEntryData.isComplete ? ResKeyCollection.Icon_Common_Check : ResKeyCollection.Icon_Common_Battle;
                var icon = await GameAsset.LoadAssetAsync<Sprite>(levelTipIconRes);
                // 初始化关卡UI
                battleLevelUI.Obj.Init(battleConfigEntry.levelName, icon.Asset, levelEntryData.isComplete);
                battleLevelUI.Obj.OnEnterBattle += async () => 
                await _uiManager.GetController<ActivityController>().EnterActivityBattle(battleConfigEntry, embersCanonData.ActivityId, () =>
                {
                    levelEntryData.isComplete = true;
                });
                // 缓存UI
                _battleLevelUis.Add(battleLevelUI);
            }
        }

        protected override void OnHide()
        {
            foreach (var battleLevelUi in _battleLevelUis)
            {
                battleLevelUi.Collect();
            }
            _battleLevelUis.Clear();
        }

        protected override void OnButtonClick(string btnName)
        {
            switch (btnName)
            {
                case "btnClose":
                    _objectSpawner.Dispose();
                    break;
            }
        }
    }
}
