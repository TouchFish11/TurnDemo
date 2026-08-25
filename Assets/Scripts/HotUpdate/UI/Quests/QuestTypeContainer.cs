using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.UI;
using HotUpdate.Common.Config.Quest;
using HotUpdate.Game.Quests;
using TMPro;
using UnityEngine.UI;

namespace HotUpdate.UI.Quests
{
    /// <summary>
    /// 任务类型容器
    /// </summary>
    public class QuestTypeContainer : UIBehaviourBase
    {
        [InjectUI] private TextMeshProUGUI txtTaskName;
        [InjectUI] private Button btnTaskSummary;
        
        private readonly List<QuetstItem> _questItems = new();
        private readonly Dictionary<int, QuetstItem> _idToItemMap = new();
        private EQuestType _questType;
        private bool _isExpand = true;   // 默认展开
        
        protected override void OnButtonClick(string btnName)
        {
            switch (btnName)
            {
                case nameof(btnTaskSummary):
                    if(_isExpand)
                    {
                        Fold();
                    }
                    else
                    {
                        Expand();
                    }
                    _isExpand = !_isExpand;
                    break;
            }
        }

        /// <summary>
        /// 初始化容器
        /// </summary>
        /// <param name="questType"></param>
        public void Init(EQuestType questType)
        {
            this._questType = questType;
            txtTaskName.text = QuestUtil.ConvertQuestTypeToStr(questType);
        }

        /// <summary>
        /// 是否包含该ID的任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ContainQuest(int id)
        {
            return _idToItemMap.ContainsKey(id);
        }

        /// <summary>
        /// 添加任务对象
        /// </summary>
        /// <param name="quetstItem"></param>
        public void AddQuestItem(QuetstItem quetstItem)
        {
            _questItems.Add(quetstItem);
            _idToItemMap.Add(quetstItem.QuestId, quetstItem);
        }

        /// <summary>
        /// 选择第一个任务项
        /// </summary>
        public Task SelectFirstQuest()
        {
            if (_questItems.Count > 0)
            {
                return _questItems[0].Select();
            }

            return Task.CompletedTask;
        }

        public bool TryGetQuest(int id, out QuetstItem questItem)
        {
            return _idToItemMap.TryGetValue(id, out questItem);
        }
        
        /// <summary>
        /// 选中该ID的任务对象
        /// </summary>
        /// <param name="id"></param>
        public bool SelectQuest(int id)
        {
            if (!_idToItemMap.TryGetValue(id, out var taskItem)) 
                return false;
            
            taskItem.Select();
            return true;
        }

        /// <summary>
        /// 折叠隐藏该类型的任务项
        /// </summary>
        private void Fold()
        {
            foreach (var taskItem in _questItems)
            {
                taskItem.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 拓展显示该类型的任务项
        /// </summary>
        private void Expand()
        {
            foreach (var poolObject in _questItems)
            {
                poolObject.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 清理任务项
        /// </summary>
        /// <param name="spawner"></param>
        public void ClearItem(ObjectSpawner spawner)
        {
            foreach (var taskItem in _questItems)
            {
                spawner.Release(taskItem);
            }
            _questItems.Clear();
            _idToItemMap.Clear();
        }
    }
}
