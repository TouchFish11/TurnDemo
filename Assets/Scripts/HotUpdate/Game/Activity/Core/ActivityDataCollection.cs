using System;
using Core.Collection;
using HotUpdate.Base.Collection;
using HotUpdate.Base.Data;

namespace HotUpdate.Game.Activity.Core
{
    /// <summary>
    /// 活动数据容器
    /// </summary>
    [Serializable]
    public class ActivityDataCollection : Collection<int, ActivityData>, IActivityDataCollection
    {
        
    }
}
