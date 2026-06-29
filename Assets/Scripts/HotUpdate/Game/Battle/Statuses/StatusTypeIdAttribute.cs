using System;

namespace HotUpdate.Game.Battle.Statuses
{
    /// <summary>
    /// 状态类型ID特性
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
