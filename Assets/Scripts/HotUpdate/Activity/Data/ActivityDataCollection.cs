using System;
using Core.Collection;
using HotUpdate.Core.Activity;

namespace HotUpdate.Activity.Data
{
    /// <summary>
    /// 活动数据容器
    /// </summary>
    [Serializable]
    public class ActivityDataCollection : Collection<int, IActivityData>, IActivityDataCollection
    {
        
    }
}
