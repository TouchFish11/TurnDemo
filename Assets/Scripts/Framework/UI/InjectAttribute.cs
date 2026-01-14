using System;

/// <summary>
/// 注入特性
/// 继承BaseUIBehaviour类的字段/属性可被标记，自动写入值，无需手动查找
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class InjectAttribute : Attribute
{

}
