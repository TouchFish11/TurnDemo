using System;

namespace Game.Tasks
{
    /// <summary>
    /// ��������
    /// </summary>
    [Serializable]
    public class TaskData
    {
        // ��ǰ����Id
        public string currentTaskId;
        // ��ǰ�������
        public int currentPro;
        // �Ƿ����
        public bool isCompleted;
        // �Ƿ�����׷��
        public bool isTracking;
    }
}
