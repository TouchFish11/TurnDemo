namespace Core.EventCenter.Events
{
    /// <summary>
    /// �򿪽����¼�
    /// </summary>
    public class OpenViewEvent : Event
    {
        /// <summary>
        /// �������������
        /// </summary>
        public string ControllerName { get; set; }
    }
}
