using System;

namespace HotUpdate.Game.Activity.Core
{
    /// <summary>
    /// 活动ID特性
    /// </summary>
    public class ActivityIdAttribute : Attribute
    {
        public int ActivityId { get; set; }
    }
}
