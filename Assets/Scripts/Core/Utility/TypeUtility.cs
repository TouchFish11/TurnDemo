using System;
using Core.Types;

namespace Core.Utility
{
    /// <summary>
    /// Type工具类
    /// 提供跨程序集的Type比较方法，避免直接比较Type导致不相同
    /// </summary>
    public static class TypeUtility
    {
        /// <summary>
        /// 是否相等
        /// 自定义比较两个不同的程序集Type是否相等。通过程序集名称和Type的FullName二者判断
        /// </summary>
        /// <returns>相等为true，不相等为false</returns>
        public static bool Equals(Type t1, Type t2)
        {
            var type1AssemblyName = t1.Assembly.GetName().Name;
            var type1FullName = t1.FullName;
            
            var type2AssemblyName = t2.Assembly.GetName().Name;
            var type2FullName = t2.FullName;
            
            return  type1AssemblyName == type2AssemblyName && type1FullName == type2FullName;
        }

        /// <summary>
        /// 转换为类型定义符
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TypeIdentifier ToIdentifier(this Type type)
        {
            return TypeIdentifier.FromType(type);
        }
    }
}
