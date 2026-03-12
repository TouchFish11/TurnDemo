using System.Collections.Generic;
using Core.Types;
using Shared.ActivityConfigSO;
using UnityEngine;

namespace Generic
{
    /// <summary>
    /// AOT生成引用
    /// </summary>
    public class AOTGenericReferences
    {
        // 覆盖List<ValueTuple<...>>的定义
        private static List<(TypeIdentifier, object)> list = new();
        // 覆盖ValueTuple<...>的构造函数
        private static (TypeIdentifier, object) tuple = (new TypeIdentifier(), new object());
        // 覆盖ActivityConfig的ScriptableObject创建
        private static ActivityConfig activityConfig = ScriptableObject.CreateInstance<ActivityConfig>();

        /// <summary>
        /// 引用方法
        /// </summary>
        public static void RefMethods()
        {
            // 调用List.Add()
            list.Add(tuple);
            list.Remove(tuple);
            list.Contains(tuple);
        }
    }
}
