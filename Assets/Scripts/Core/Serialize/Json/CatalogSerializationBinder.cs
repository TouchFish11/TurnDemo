using System;
using System.Collections.Generic;
using Core.AssetBundles.Collection;
using Core.AssetBundles.Management;
using Newtonsoft.Json.Serialization;

namespace Core.Serialize.Json
{
    public class CatalogSerializationBinder : DefaultSerializationBinder
    {
        // 白名单：只允许反序列化的类型全名
        private static readonly HashSet<Type> AllowedTypes = new()
        {
            typeof(AssetEntry),
            typeof(SpriteAssetEntry),
            typeof(AssetCatalog),
            typeof(Dictionary<string, AssetEntry>),
            typeof(Dictionary<string, SpriteAssetEntry>),
            typeof(ABPackageCollection),
            typeof(Dictionary<string, ABPackageInfo>),
            typeof(ABPackageInfo),
            typeof(string[]),
            typeof(Dictionary<string, List<string>>),
            typeof(List<string>),
            // 未来新增的Entry类型在这里加一行即可
        };

        public override Type BindToType(string assemblyName, string typeName)
        {
            var resolveType = base.BindToType(assemblyName, typeName);
            
            // 检查类型全名是否在白名单中
            if (!AllowedTypes.Contains(resolveType))
                throw new InvalidOperationException($"Invalid deserialization type: {typeName}");
            
            // 白名单通过，正常解析
            return resolveType;
        }
    }
}
