using System;

namespace Core.UI
{
    /// <summary>
    /// 注入UI特性
    /// 继承BaseUIBehaviour类的UIBehaviour字段/属性可被标记，自动写入值，无需手动查找
    /// 对于RectTransform，只能找到属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class InjectUIAttribute : Attribute
    {
        /// <summary>
        /// RectTransform的标记
        /// 默认为0，0则不作为RectTransform赋值，若为1，则将RectTransform赋值到该字段/属性
        /// UIBehaviour字段/属性忽略该属性
        /// </summary>
        public byte RectTransformFlag { get; }

        public InjectUIAttribute()
        {

        }

        /// <summary>
        /// 构造函数
        /// 用于初始化RectTransform
        /// </summary>
        /// <param name="rectTransformFlag">默认为0，0则不作为RectTransform赋值，若为1，则将RectTransform赋值到该字段/属性</param>
        public InjectUIAttribute(byte rectTransformFlag)
        {
            RectTransformFlag = rectTransformFlag;
        }
    }
}
