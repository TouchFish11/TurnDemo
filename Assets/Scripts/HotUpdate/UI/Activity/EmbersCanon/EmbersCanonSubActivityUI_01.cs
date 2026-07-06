using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.UI;
using HotUpdate.Base.Service;

using UnityEngine.UI;

namespace HotUpdate.UI.Activity.EmbersCanon
{
    /// <summary>
    /// 余烬圣典子界面UI01
    /// </summary>
    public class EmbersCanonSubActivityUI_01 : UIBehaviourBase
    {
        [Inject] private ObjectSpawner _objectSpawner;
        [Inject] private IIconService _iconService;
        
        [InjectUI] private ScrollRect svLevel;

        private readonly List<BattleLevelUI> _battleLevelUis = new();
        private EmbersCanonHandler _embersCanonHandler;
        private ActivityInfo _activityInfo;

        public event Action OnClose;
        
        public async Task Init(ActivityInfo activityInfo, EmbersCanonHandler embersCanonHandler)
        {
            _activityInfo = activityInfo;
            _embersCanonHandler = embersCanonHandler;
            await UpdateInfo();
        }
        
        public Task Activate()
        {
            return UpdateInfo();
        }

        private async Task UpdateInfo()
        {
            // 初始化关卡
            var (battleConfigEntryColletion, embersCanonData) = await _embersCanonHandler.InitLevels(_activityInfo.f_id);
            foreach (var battleConfigEntry in battleConfigEntryColletion.battleConfigs)
            {
                var battleLevelUI = await _objectSpawner.SpawnAsync<BattleLevelUI>(AssetKeys.BattleLevelUI, svLevel.content);
                // 获取用户数据中的战斗关卡条目
                var levelEntryData = embersCanonData.GetLevelData(battleConfigEntry.levelId);
                // 根据是否完成使用不同的Sprite
                var levelTipIconRes = levelEntryData.isComplete ? AssetKeys.Icon_Common_Check : AssetKeys.Icon_Common_Battle;
                var icon = await _iconService.LoadIconAsync(levelTipIconRes);
                // 初始化关卡UI
                battleLevelUI.Init(battleConfigEntry.levelName, icon, levelEntryData.isComplete);
                battleLevelUI.OnEnterBattle += async () => 
                await _embersCanonHandler.EnterActivityBattle(battleConfigEntry, embersCanonData.ActivityId, () =>
                {
                    levelEntryData.isComplete = true;
                });
                // 缓存UI
                _battleLevelUis.Add(battleLevelUI);
            }
        }
        
        protected override void OnButtonClick(string btnName)
        {
            switch (btnName)
            {
                case "btnClose":
                    OnClose?.Invoke();
                    break;
            }
        }

        public void Deactivate()
        {
            _objectSpawner.Release(_battleLevelUis);
            _battleLevelUis.Clear();
            _iconService.ReleaseAll();
        }

        public void Destroy()
        {
            _objectSpawner.Dispose();
            _objectSpawner = null;
            _iconService.Dispose();
            _iconService = null;
            OnClose = null;
            _embersCanonHandler = null;
            _activityInfo = null;
        }
    }
}
