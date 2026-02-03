using System;
using Game.Tasks;

namespace GameHotUpdate.Tasks
{
    /// <summary>
    /// ��������
    /// </summary>
    [Serializable]
    public class TaskData : ITaskData
    {
        // ��ǰ����Id
        public string currentTaskId;
        // ��ǰ�������
        public int currentPro;
        // �Ƿ����
        public bool isCompleted;
        // �Ƿ�����׷��
        public bool isTracking;

        
        public string CurrentTaskId => currentTaskId;

        public int CurrentPro
        {
            get => currentPro;
            set => currentPro = value;
        }

        public bool IsCompleted
        {
            get => isCompleted;
            set => isCompleted = value;
        }

        public bool IsTracking
        {
            get => isTracking;
            set => isTracking = value;
        }
    }
}
