using System;
using Core.Collection;

namespace HotUpdate.Activity.Data
{
    /// <summary>
    /// 活动数据容器
    /// </summary>
    [Serializable]
    public class ActivityDataCollection : Collection<int, ActivityData>, IActivityDataCollection
    {
        
    }
}
