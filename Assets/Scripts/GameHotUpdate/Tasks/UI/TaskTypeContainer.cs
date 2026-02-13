using System.Collections.Generic;
using Core.UI;
using Core.Utility;
using TMPro;

namespace GameHotUpdate.Tasks.UI
{
    /// <summary>
    /// ������������
    /// </summary>
    public class TaskTypeContainer : UIBehaviourBase
    {
        [Inject] private TextMeshProUGUI txtTaskName;

        private readonly List<TaskItem> taskItems = new();
        private readonly Dictionary<string, TaskItem> idToItemMap = new();

        private int taskType;
        private bool isExpand = true;

        protected override void OnButtonClick(string btnName)
        {
            switch (btnName)
            {
                case "btnTaskSummary":
                    if(isExpand)
                    {
                        Fold();
                    }
                    else
                    {
                        Expand();
                    }
                    isExpand = !isExpand;
                    break;
            }
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="taskType"></param>
        public void Init(int taskType)
        {
            this.taskType = taskType;
            txtTaskName.text = taskType.TaskTypeToStr();
        }

        public bool ContainTask(string id)
        {
            foreach (var cacheId in idToItemMap.Keys)
            {
                if (TextUtility.Split(cacheId, 7)[0] == TextUtility.Split(id, 7)[0])
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="taskItem"></param>
        public void AddItem(TaskItem taskItem)
        {
            //taskDatas.Add(taskData);
            taskItems.Add(taskItem);
            idToItemMap.Add(taskItem.TaskId, taskItem);
        }

        /// <summary>
        /// Ĭ��ѡ�е�һ������
        /// </summary>
        public void DefaultSelectFirstTask()
        {
            if (taskItems.Count > 0)
            {
                taskItems[0].Select();
            }
        }

        /// <summary>
        /// ѡ��ָ������
        /// </summary>
        /// <param name="id"></param>
        public void SelectTask(string id)
        {
            if (idToItemMap.TryGetValue(id, out var taskItem))
            {
                taskItem.Select();
            }
        }

        /// <summary>
        /// �۵�
        /// </summary>
        private void Fold()
        {
            foreach (var taskItem in taskItems)
            {
                taskItem.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// չ��
        /// </summary>
        private void Expand()
        {
            foreach (var taskItem in taskItems)
            {
                taskItem.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// ���������
        /// </summary>
        public void ClearItem()
        {
            taskItems.Clear();
        }
    }
}
