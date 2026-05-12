using System;

namespace HotUpdate.Game.Battle.Status
{
    /// <summary>
    /// ״̬����ID����
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class StatusTypeIdAttribute : Attribute
    {
        public int StatusId { get; }

        public StatusTypeIdAttribute(int statusId)
        {
            StatusId = statusId;
        }
    }
}
