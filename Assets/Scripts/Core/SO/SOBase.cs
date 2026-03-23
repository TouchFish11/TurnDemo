using System;
using Newtonsoft.Json;

namespace Core.SO
{
    /// <summary>
    /// SO基类，所有需要序列化ScriptableObject的类都要继承该类
    /// 在各自的OnValidate中将序列化的目标类型赋值到target中
    /// 该设计避免了直接序列化SO导致运行时依赖SO，分离配置数据和Unity对象，现在运行时只需直接反序列化配置数据即可
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class SOBase : UnityEngine.ScriptableObject
    {
        /// <summary>
        /// 要序列化的目标对象
        /// </summary>
        [JsonProperty] public object target;
    }
}
