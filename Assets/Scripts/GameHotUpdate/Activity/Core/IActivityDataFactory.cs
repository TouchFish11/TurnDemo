using Core.Reflection;
using GameHotUpdate.Activity.Data;

namespace GameHotUpdate.Activity.Core
{
    public interface IActivityDataFactory : IFactory
    {
        /// <summary>
        /// 获取活动数据
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        ActivityData GetData(int activityId);
    }
}
