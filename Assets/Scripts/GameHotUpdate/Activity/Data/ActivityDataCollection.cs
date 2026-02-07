using System;
using Core.AssetBundles.Update.Collection;
using Game.Activity;

namespace GameHotUpdate.Activity.Data
{
    /// <summary>
    /// 活动数据容器
    /// </summary>
    [Serializable]
    public class ActivityDataCollection : Collection<int, ActivityData>, IActivityDataCollection
    {
        
    }
}
