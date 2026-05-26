using System;
using System.Collections.Generic;
using Core.DI;
using Core.HotUpdate;
using Core.Log;
using HotUpdate.Base.Utility;

namespace HotUpdate.Base.Factory
{
    /// <summary>
    /// 工厂基类
    /// </summary>
    /// <typeparam name="TIValue">接口类型</typeparam>
    public abstract class Factory<TIValue> : IFactory where TIValue : class
    {
        [Inject] private IHotUpdateManager _hotUpdateManager;
        // 具体类型到接口的映射
        protected readonly Dictionary<Type, TIValue> typeToInterfaceMap = new();

        public virtual void InitFactory()
        {
            FactoryUtility.ScanAllType(typeToInterfaceMap, _hotUpdateManager.GetAssemblies());
        }

        public virtual TInterface GetTypeInstance<TInterface, TInstance>() where TInterface : class where TInstance : TInterface
        {
            if (typeToInterfaceMap.TryGetValue(typeof(TInstance), out var instance))
            {
                return instance as TInterface;
            }
            
            Logger.LogError($"未找到类型实例：{typeof(TInstance)}");
            return null;
        }
    }
}
