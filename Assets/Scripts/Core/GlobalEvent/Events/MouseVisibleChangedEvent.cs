namespace Core.GlobalEvent.Events
{
    /// <summary>
    /// ���ɼ��Ա仯�¼�
    /// </summary>
    public class MouseVisibleChangedEvent : Event
    {
        public string SourceName { get; set; }

        public bool IsVisible { get; set; }

    }
}
