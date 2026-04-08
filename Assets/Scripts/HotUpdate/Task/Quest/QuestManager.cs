using System.Collections.Generic;
using HotUpdate.Config.Quest;
using HotUpdate.Config.Quest.Config;
using HotUpdate.Core.Task;

namespace HotUpdate.Task.Quest
{
    /// <summary>
    /// 任务管理器
    /// </summary>
    public class QuestManager : IQuestManager
    {
        // 缓存所有的任务对象
        private readonly Dictionary<int, IQuest> _quests = new();
        // 当前接取的任务对象
        private IQuest _currentQuest;
        
        /// <summary>
        /// 初始化任务
        /// </summary>
        /// <param name="questConfig"></param>
        /// <param name="questCollection"></param>
        public void InitQuests(QuestConfig questConfig, IQuestCollection questCollection)
        {
            // 创建任务对象，只创建没有完成的任务
            foreach (var questConfigQuestItem in questConfig.questItems)
            {
                Quest quest;
                if (questCollection.TryGetValue(questConfigQuestItem.id, out var questData))
                {
                    // 有且完成不创建对象
                    if (questData.IsComplete)
                    {
                        continue;
                    }

                    // 有且未完成，仍要创建对象
                    quest = new Quest(questConfigQuestItem, questData);
                    quest.OnQuestComplete += OnQuestComplete;
                    _quests.Add(questConfigQuestItem.id, quest);
                    continue;
                }

                // 没有也要创建，构建任务节点数据
                var nodeDatas = new List<QuestNodeData>();
                foreach (var questItemNodeConfig in questConfigQuestItem.nodeConfigs)
                {
                    nodeDatas.Add(new QuestNodeData(questItemNodeConfig.nodeId, EQuestPhase.NoReceive, 0, questItemNodeConfig.nextNodeId));
                }
                
                var newQuestData = new QuestData(questConfigQuestItem.id, nodeDatas);
                // 保存到数据
                questCollection.AddQuestData(newQuestData);
                quest = new Quest(questConfigQuestItem, newQuestData);
                quest.OnQuestComplete += OnQuestComplete;
                _quests.Add(questConfigQuestItem.id, quest);
            }

            // 主动判断当前是否有接取的任务，用于逻辑状态恢复
            foreach (var quests in _quests.Values)
            {
                if (!quests.IsTracking) continue;
                _currentQuest = quests;
                break;
            }
        }

        public void AcceptQuest(int questId)
        {
            // 若当前已有正在追踪的任务，先取消该任务的追踪
            _currentQuest?.CancelAccept();
            if (!_quests.TryGetValue(questId, out var quest))
                throw new KeyNotFoundException($"{nameof(QuestManager)}:Quest {questId} not found");
            
            quest.Accept();
            _currentQuest = quest;
        }

        public void CancelQuest()
        {
            _currentQuest?.CancelAccept();
            _currentQuest = null;
        }

        private void OnQuestComplete(int questId)
        {
            _quests.Remove(questId);
        }
        
        public IEnumerable<IQuest> GetQuests()
        {
            foreach (var questsValue in _quests.Values)
            {
                yield return questsValue;
            }
        }
    }
}
