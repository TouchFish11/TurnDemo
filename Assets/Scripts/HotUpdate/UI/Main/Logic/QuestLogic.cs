using System;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Serialize.Json;
using Core.Utility;
using HotUpdate.Base.Manager;
using HotUpdate.Common.Config.Quest.Config;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.UI.Main.Logic
{
    /// <summary>
    /// 任务逻辑
    /// </summary>
    public class QuestLogic : MainLogic
    {
        [Inject] private IJsonManager _jsonManager;
        [Inject] private IQuestDataManager _questDataManager;
        [Inject] private IQuestManager _questManager;
        
        private QuestViewModel _questViewModel;
        
        protected override async void OnInit()
        {
            try
            {
                using var handle = await GameAsset.LoadAssetAsync<TextAsset>(AssetKeys.QuestConfig);
                var questConfig = _jsonManager.FromJson<QuestConfig>(handle.Asset.text, settings: NewtonsoftJsonUtility.SerializerSettings);
                
                // 初始化任务管理器
                _questManager.InitQuests(questConfig, _questDataManager.QuestCollection);
                // 获取最新的任务数据列表
                var questDatas = _questDataManager.QuestCollection.GetQuestDatas();
                // 当前追踪的任务节点数据初始化VM
                _questViewModel = DIContainer.Create<QuestViewModel>(parameterValues: new object[] { questConfig, questDatas });
                _questViewModel.IsActiveQuestbar.Subscribe(isActive => mainView.SetTaskbarActive(isActive));
                _questViewModel.QuestTitleName.Subscribe(titleName => mainView.SetQuestbarTitle(titleName));
                _questViewModel.QuestTip.Subscribe(tip => mainView.SetQuestbarTip(tip));
                _questViewModel.QuestProgress.Subscribe(progress => mainView.SetQuestbarProgress(progress));
                
                // 主动拉取UI更新
                _questViewModel.RefleshUI(_questDataManager.QuestCollection.TryGetTrackQuest(out var questData) ? questData : null);
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(QuestLogic)}: QuestLogic init error,{e.Message}");
            }
        }

        public override void ResetData()
        {
            _questViewModel = null;
            base.ResetData();
        }
    }
}
