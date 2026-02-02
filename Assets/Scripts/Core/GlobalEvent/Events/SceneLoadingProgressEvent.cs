namespace Core.GlobalEvent.Events
{
    /// <summary>
    /// �������ؽ����¼�
    /// </summary>
    public class SceneLoadingProgressEvent : Event
    {
        /// <summary>
        /// ���ؽ���
        /// ��Χ0~1
        /// </summary>
        public float Progress { get; set; }
    }
}
