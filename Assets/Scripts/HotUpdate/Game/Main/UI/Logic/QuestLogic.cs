using System;
using Core.DI;
using Core.Serialize.Json;
using Core.Utility;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Quest;
using HotUpdate.Common;
using HotUpdate.Common.Config.Quest.Config;

namespace HotUpdate.Game.Main.UI.Logic
{
    /// <summary>
    /// 任务逻辑
    /// </summary>
    public class QuestLogic : MainLogic
    {
        private QuestViewModel _questViewModel;
        
        protected override async void OnInit()
        {
            try
            {
                var textAsset = await DIContainer.GetInstance<ITextLoader>().LoadAssetAsync(AbKeyCollection.Gameconfig, ResKeyCollection.QuestConfig);
                var questConfig = DIContainer.GetInstance<IJsonManager>().FromJson<QuestConfig>(textAsset.text, settings: NewtonsoftJsonUtility.SerializerSettings);
                var provider = DIContainer.GetInstance<IGameManager>().GameDataManager.GetProvider<ITaskDataProvider>();
                
                // 初始化任务管理器
                DIContainer.GetInstance<IQuestManager>().InitQuests(questConfig, provider.QuestCollection);
                // 获取最新的任务数据列表
                var questDatas = provider.QuestCollection.GetQuestDatas();
                // 当前追踪的任务节点数据初始化VM
                _questViewModel = new QuestViewModel(questConfig, questDatas);
            
                _questViewModel.IsActiveQuestbar.Subscribe(isActive => mainView.SetTaskbarActive(isActive));
                _questViewModel.QuestTitleName.Subscribe(titleName => mainView.SetQuestbarTitle(titleName));
                _questViewModel.QuestTip.Subscribe(tip => mainView.SetQuestbarTip(tip));
                _questViewModel.QuestProgress.Subscribe(progress => mainView.SetQuestbarProgress(progress));
                
                // 主动拉取UI更新
                _questViewModel.RefleshUI(provider.QuestCollection.TryGetTrackQuest(out var questData) ? questData : null);
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(QuestLogic)}.{nameof(OnInit)}: {e.Message}");
            }
        }

        public override void ResetData()
        {
            _questViewModel = null;
            base.ResetData();
        }
    }
}
