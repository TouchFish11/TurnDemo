using System;

namespace Core.Types
{
    /// <summary>
    /// 类型定义符
    /// 用于唯一标识程序集中的某个类型，包含程序集名称和类型完全限定名
    /// 实现 IEquatable接口以支持值相等性比较
    /// </summary>
    public readonly struct TypeIdentifier : IEquatable<TypeIdentifier>
    {
        /// <summary>
        /// 类型
        /// </summary>
        public Type Type { get; }
        
        /// <summary>
        /// 程序集名称（仅包含程序集名称，不包含版本、文化、公钥令牌等完整信息）
        /// </summary>
        public string AssemblyName { get; }

        /// <summary>
        /// 类型的完全限定名（包含命名空间和类型名）
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// 使用指定的程序集名称和类型完全限定名初始化 TypeIdentifier 类的新实例
        /// </summary>
        /// <param name="assemblyName">程序集的完整名称</param>
        /// <param name="typeName">类型的完全限定名</param>
        /// <param name="type">类型</param>
        public TypeIdentifier(string assemblyName, string typeName, Type type)
        {
            Type = type;
            AssemblyName = assemblyName;
            FullName = typeName;
        }

        /// <summary>
        /// 从 System.Type 实例创建 TypeIdentifier 实例
        /// </summary>
        /// <param name="type">要转换的 System.Type 实例</param>
        /// <returns>
        /// 若 type 为 null，则返回 null；否则返回包含该类型程序集名称和完全限定名的 TypeIdentifier 实例
        /// </returns>
        public static TypeIdentifier FromType(Type type)
        {
            return new TypeIdentifier(type.Assembly.FullName, type.FullName, type);
        }
        
        /// <summary>
        /// 重载相等运算符，判断两个 TypeIdentifier 实例是否相等
        /// </summary>
        /// <param name="a">第一个 TypeIdentifier 实例</param>
        /// <param name="b">第二个 TypeIdentifier 实例</param>
        /// <returns>
        /// 当且仅当 a 和 b 均不为 null，且程序集名称、类型完全限定名均相等时返回 true；否则返回 false
        /// </returns>
        public static bool operator ==(TypeIdentifier a, TypeIdentifier b)
        {
            return a.AssemblyName == b.AssemblyName && a.FullName == b.FullName;
        }

        /// <summary>
        /// 重载不等运算符，判断两个 TypeIdentifier 实例是否不相等
        /// </summary>
        /// <param name="a">第一个 TypeIdentifier 实例</param>
        /// <param name="b">第二个 TypeIdentifier 实例</param>
        /// <returns>若 a 和 b 相等则返回 false，否则返回 true</returns>
        public static bool operator !=(TypeIdentifier a, TypeIdentifier b)
        {
            return !(a == b);
        }

        /// <summary>
        /// 判断当前实例是否与另一个 TypeIdentifier 实例相等
        /// </summary>
        /// <param name="other">要比较的另一个 TypeIdentifier 实例</param>
        /// <returns>若 other 与当前实例相等则返回 true，否则返回 false</returns>
        public bool Equals(TypeIdentifier other)
        {
            return other == this;
        }

        /// <summary>
        /// 判断当前实例是否与指定对象相等
        /// </summary>
        /// <param name="obj">要比较的对象</param>
        /// <returns>
        /// 若 obj 是 TypeIdentifier 实例且与当前实例相等，则返回 true；否则返回 false
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is TypeIdentifier other && Equals(other);
        }

        /// <summary>
        /// 获取当前实例的哈希码（基于程序集名称和类型完全限定名计算）
        /// </summary>
        /// <returns>当前实例的哈希码值</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(AssemblyName, FullName);
        }

        /// <summary>
        /// 返回表示当前 TypeIdentifier 实例的字符串
        /// </summary>
        /// <returns>包含程序集名称和类型完全限定名的格式化字符串</returns>
        public override string ToString()
        {
            return $"Assembly：{AssemblyName}, FullName：{FullName}";
        }
    }
}