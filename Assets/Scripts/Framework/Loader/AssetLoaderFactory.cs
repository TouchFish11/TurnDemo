using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 资源加载器工厂
    /// </summary>
    public class AssetLoaderFactory : Factory<IAssetLoader>
    {
        public override T GetTypeInstance<T>()
        {


            if (typeToITypeMap.TryGetValue(typeof(T), out var value))
            {
                return value as T;
            }

            LogManager.LogError($"未找到类型实例：{typeof(T)}");
            return default;
        }

        /// <summary>
        /// 获取精灵加载器
        /// </summary>
        /// <returns></returns>
        public ISpriteLoader GetSpriteLoader()
        {
            Type loaderType = null;
#if !UNITY_EDITOR || EDITOR_TEST_AB
            loaderType = typeof(SpriteLoader);
#else
            loaderType = typeof(MockSpriteLoader);
#endif
            return typeToITypeMap[loaderType] as ISpriteLoader;
        }
    }
}
