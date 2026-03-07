using Core.Reflection;

namespace HotUpdate.Core.Activity
{
    public interface IActivityDataFactory : IFactory
    {
        /// <summary>
        /// 获取活动数据
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        IActivityData GetData(int activityId);
    }
}
