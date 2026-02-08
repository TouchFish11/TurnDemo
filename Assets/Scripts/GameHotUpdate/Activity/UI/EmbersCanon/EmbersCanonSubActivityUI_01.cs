using System.Collections.Generic;
using System.Threading.Tasks;
using Config.ActivityConfigSO;
using Core.AssetBundles.Management;
using Core.Config;
using Core.DataPersistence.Json;
using Core.Loader;
using Core.Pool;
using Core.Reflection;
using Core.Service;
using Core.UI;
using Game.Manager;
using Game.Objects;
using GameHotUpdate.Activity.Core;
using GameHotUpdate.Activity.Data;
using UnityEngine;
using UnityEngine.UI;

namespace GameHotUpdate.Activity.UI.EmbersCanon
{
    /// <summary>
    /// 余烬圣典子界面UI01
    /// </summary>
    public class EmbersCanonSubActivityUI_01 : ActivityBase
    {
        [Inject] private ScrollRect svLevel;

        private readonly IList<BattleLevelUI> _battleLevelUis = new List<BattleLevelUI>();

        protected override async Task OnInit()
        {
            // 根据读取用户活动数据
            var activityDataCollection = ServiceLocator.Get<IGameManager>().GameDataManager.ActivityDataCollection as ActivityDataCollection;
            // 获取该活动数据
            var embersCanonData = activityDataCollection[activityInfo.f_id] as EmbersCanonData;
            // AB包加载配置
            var textAsset = await ServiceLocator.Get<IAssetBundleManager>().LoadAssetAsync<TextAsset>(EAssetBundleType.GameConfig,
                ResKeyCollection.BattleActivityConfig);
            // 解析该活动的关卡配置
            var battleConfigEntryColletion = ServiceLocator.Get<IJsonManager>().FromJson<BattleConfigEntryColletion>(textAsset.text);
            // 初始化关卡
            foreach (var battleConfigEntry in battleConfigEntryColletion.battleConfigs)
            {
                var battleLevelUI = await ServiceLocator.Get<IObjectBuilder>().GetHotfixUIObject<BattleLevelUI>(EAssetBundleType.UI,
                    ResKeyCollection.BattleLevelUI, svLevel.content);
                // 获取用户数据中的战斗关卡条目
                var levelEntryData = embersCanonData.GetLevelData(battleConfigEntry.levelId);
                // 新增数据
                if (levelEntryData == null)
                {
                    levelEntryData = new EmbersCanonLevelEntry { levelId = battleConfigEntry.levelId, isComplete = false };
                    embersCanonData.Add(levelEntryData);
                }

                // 根据是否完成使用不同的Sprite
                var levelTipIconRes = levelEntryData.isComplete ? ResKeyCollection.Icon_Common_Check : ResKeyCollection.Icon_Common_Battle;
                
                var icon = await ServiceLocator.Get<IFactoryManager>()
                    .GetFactory<IAssetLoaderFactory, AssetLoaderFactory>().GetSpriteLoader()
                    .GetSpriteAsync(ResKeyCollection.Atlas_Icon_Common, levelTipIconRes);
                // 初始化关卡UI
                battleLevelUI.Init(battleConfigEntry.levelName, icon, battleConfigEntry);
                // 缓存UI
                _battleLevelUis.Add(battleLevelUI);
            }
        }

        protected override void OnHide()
        {
            foreach (var battleLevelUi in _battleLevelUis)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(battleLevelUi.gameObject);
            }
            _battleLevelUis.Clear();
            
            ServiceLocator.Get<IPoolManager>().ClearTypes(typeof(BattleLevelUI), typeof(EmbersCanonSubActivityUI_01));
        }

        protected override void OnButtonClick(string btnName)
        {
            switch (btnName)
            {
                case "btnClose":
                    ServiceLocator.Get<IPoolManager>().PushObj(this.gameObject);
                    break;
            }
        }
    }
}
