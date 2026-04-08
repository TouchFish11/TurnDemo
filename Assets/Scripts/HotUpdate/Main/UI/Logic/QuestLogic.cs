using System;
using Core.Loader.Text;
using Core.Log;
using Core.Serialize.Json;
using Core.Service;
using Core.Utility;
using HotUpdate.Common;
using HotUpdate.Config.Quest.Config;
using HotUpdate.Core.Manager;
using HotUpdate.Core.Task;

namespace HotUpdate.Main.UI.Logic
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
                var textAsset = await ServiceLocator.Get<ITextLoader>().LoadAssetAsync(AbKeyCollection.Gameconfig, ResKeyCollection.QuestConfig);
                var questConfig = ServiceLocator.Get<IJsonManager>().FromJson<QuestConfig>(textAsset.text, settings: NewtonsoftJsonUtility.SerializerSettings);
                var provider = ServiceLocator.Get<IGameManager>().GameDataManager.GetProvider<ITaskDataProvider>();
                var questDatas = provider.QuestCollection.GetQuestDatas();
                // 当前追踪的任务节点数据初始化VM
                _questViewModel = new QuestViewModel(questConfig, questDatas);
            
                _questViewModel.IsActiveQuestbar.Subscribe(isActive => mainView.SetTaskbarActive(isActive));
                _questViewModel.QuestTitleName.Subscribe(titleName => mainView.SetQuestbarTitle(titleName));
                _questViewModel.QuestTip.Subscribe(tip => mainView.SetQuestbarTip(tip));
                _questViewModel.QuestProgress.Subscribe(progress => mainView.SetQuestbarProgress(progress));
                
                // 初始化任务管理器
                ServiceLocator.Get<IQuestManager>().InitQuests(questConfig, provider.QuestCollection);
                var nodeData = provider.QuestCollection.TryGetTrackQuest(out var questData) ? questData.GetNodeData(questData.CurActiveNodeId) : null;
                // 主动拉取UI更新
                _questViewModel.RefleshUI(questConfig, nodeData);
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
