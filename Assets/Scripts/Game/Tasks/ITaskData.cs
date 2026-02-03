using Game.Data;

namespace Game.Tasks
{
    public interface ITaskData : IData
    {
        public string CurrentTaskId { get; }
        // ��ǰ�������
        public int CurrentPro { get; set; }
        // �Ƿ����
        public bool IsCompleted { get; set; }
        // �Ƿ�����׷��
        public bool IsTracking { get; set; }
    }
}
