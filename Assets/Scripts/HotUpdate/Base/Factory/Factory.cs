using System;
using System.Collections.Generic;
using Core.HotUpdate;
using HotUpdate.Base.Utility;

namespace HotUpdate.Base.Factory
{
    /// <summary>
    /// 工厂基类
    /// </summary>
    /// <typeparam name="TIValue">接口类型</typeparam>
    public abstract class Factory<TIValue> : IFactory where TIValue : class
    {
        // 具体类型到接口的映射
        protected readonly Dictionary<Type, TIValue> typeToInterfaceMap = new();

        protected Factory(IHotUpdateManager hotUpdateManager)
        {
            FactoryUtility.ScanAllType(typeToInterfaceMap, hotUpdateManager.GetHotAssemblies());
        }
    }
}
