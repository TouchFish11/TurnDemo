using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Core.HotUpdate
{
    /// <summary>
    /// 热更新程序集设置
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class HotUpdateAssemblySettings
    {
        [JsonProperty] public Dictionary<string, List<string>> dllDependencies = new();
    }
}
