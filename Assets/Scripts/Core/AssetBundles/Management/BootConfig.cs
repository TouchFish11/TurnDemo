using System;

namespace Core.AssetBundles.Management
{
    [Serializable]
    public class BootConfig
    {
        /// 补充元数据DLL包名
        public string aotDllBundleName;
        /// 热更 DLL 所在的 AB 包名（带 .assetBundle 后缀）
        public string hotfixDllBundleName;
        /// 热更入口对象Key
        public string hotfixObjKey;
        /// 版本号，用于调试
        public string version;
    }
}