using System.Collections.Generic;
using Core.AssetBundles.Management;
using Core.Serialize.Json;
using Core.Service;
using Core.Tasks.Extensions;
using Core.UI;
using Core.Utility;
using HotUpdate.Activity.Core;
using HotUpdate.Activity.Data;
using HotUpdate.Common;
using HotUpdate.Config.Activity;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Activity.UI.EmbersCanon
{
    using Task = System.Threading.Tasks.Task;
    
    /// <summary>
    /// 余烬圣典子界面UI01
    /// </summary>
    public class EmbersCanonSubActivityUI_01 : ActivityUIBehaviourBase
    {
        [Inject] private ScrollRect svLevel;

        private readonly IList<BattleLevelUI> _battleLevelUis = new List<BattleLevelUI>();
        
        protected override async Task OnShow()
        {
            // 根据读取用户活动数据
            var activityDataCollection = ServiceLocator.Get<IGameManager>().GameDataManager.ActivityDataCollection as ActivityDataCollection;
            // 获取该活动数据
            var embersCanonData = activityDataCollection[activityInfo.f_id] as EmbersCanonData;
            // AB包加载配置
            var configAb = await ServiceLocator.Get<IAssetBundleManager>().LoadBundleAsync(AbKeyCollection.Gameconfig);
            var textAsset = await configAb.LoadAssetAsync<TextAsset>(ResKeyCollection.BattleActivityConfig).ToTask<TextAsset>();
            // 解析该活动的关卡配置
            var battleActivityConfig = ServiceLocator.Get<IJsonManager>().FromJson<BattleActivityConfig>(textAsset.text, settings: NewtonsoftJsonUtility.SerializerSettings);
            // 初始化关卡
            foreach (var battleConfigEntry in battleActivityConfig.BattleConfigEntryColletion.battleConfigs)
            {
                var battleLevelUI = await prefabLoader.GetObjectAsync<BattleLevelUI>(AbKeyCollection.Ui,
                    ResKeyCollection.BattleLevelUI, svLevel.content);
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
                
                var icon = await spriteLoader.LoadSpriteAsync(AbKeyCollection.Spriteatlas, ResKeyCollection.Atlas_Icon_Common, levelTipIconRes);
                // 初始化关卡UI
                battleLevelUI.Init(battleConfigEntry.levelName, icon, levelEntryData, battleConfigEntry);
                // 缓存UI
                _battleLevelUis.Add(battleLevelUI);
            }
        }

        protected override void OnHide()
        {
            foreach (var battleLevelUi in _battleLevelUis)
            {
                prefabLoader.CollectAsset(battleLevelUi.gameObject);
            }
            _battleLevelUis.Clear();
            prefabLoader.RealseAsset(AbKeyCollection.Ui, ResKeyCollection.BattleLevelUI);
        }

        protected override void OnButtonClick(string btnName)
        {
            switch (btnName)
            {
                case "btnClose":
                    prefabLoader.CollectAsset(GameObject);
                    prefabLoader.RealseAsset(AbKeyCollection.Ui, name);
                    break;
            }
        }
    }
}
