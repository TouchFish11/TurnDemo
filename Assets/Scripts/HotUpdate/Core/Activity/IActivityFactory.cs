using System;
using Core.Reflection;

namespace HotUpdate.Core.Activity
{
    public interface IActivityFactory : IFactory
    {
        /// <summary>
        /// 获取活动类型
        /// </summary>
        /// <param name="activityKey"></param>
        /// <returns></returns>
        Type GetActivity(string activityKey);
    }
}
