using System.Collections.Generic;
using Game.ActivityConfigSO;
using UnityEngine;

namespace Generic
{
    /// <summary>
    /// AOT生成引用
    /// </summary>
    public class AOTGenericReferences
    {
        // 覆盖ActivityConfig的ScriptableObject创建
        private static ActivityConfig activityConfig = ScriptableObject.CreateInstance<ActivityConfig>();

        /// <summary>
        /// 引用方法
        /// </summary>
        public static void RefMethods()
        {
            // 调用List.Add()

        }
    }
}
