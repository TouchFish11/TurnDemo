using System;
using System.Collections.Generic;
using System.Reflection;

namespace Core.DI
{
    /// <summary>
    /// 类型构造信息
    /// </summary>
    internal class TypeConstructorInfo
    {
        public List<ConstructorInfo> ConstructorInfos { get; private set; } = new();
        
        public ConstructorInfo FirstInfo { get; private set; }
        
        /// <summary>
        /// 根据类型数组设置首选构造
        /// </summary>
        /// <param name="types">传null则设置FirstInfo为null，types长度为0则设置为无参构造</param>
        public void SetFirstInfo(params Type[] types)
        {
            if (types == null)
            {
                FirstInfo = null;
                return;
            }

            if (types.Length == 0)
            {
                FirstInfo = ConstructorInfos.Find(info => info.GetParameters().Length == 0);
                return;
            }
            
            foreach (var constructorInfo in ConstructorInfos)
            {
                var parameterInfos = constructorInfo.GetParameters();
                var match = true;
                for (var i = 0; i < types.Length; i++)
                {
                    var type = types[i];
                    var parameterType = parameterInfos[i].ParameterType;
                    if (type != parameterType)
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    FirstInfo = constructorInfo;
                    break;
                }
            }
        }
    }
}
