namespace Core.Editor.Generation
{
    /// <summary>
    /// 访问修饰符枚举
    /// 定义代码生成时可使用的类/字段访问修饰符类型
    /// </summary>
    internal enum E_AccessModifier
    {
        /// <summary>无修饰符</summary>
        None,
        /// <summary>公共访问修饰符</summary>
        Public,
        /// <summary>受保护访问修饰符</summary>
        Protected,
        /// <summary>私有访问修饰符</summary>
        Private,
        /// <summary>内部访问修饰符</summary>
        Internal,
    }
}