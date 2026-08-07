using Core.Singleton;

namespace Core.Global
{
    /// <summary>
    /// 全局设置
    /// </summary>
    public class GlobalSettings : SingletonSOBase<GlobalSettings>
    {
        /// <summary>
        /// 日志模块配置
        /// </summary>
        public LogModuleConfig logModuleConfig;
        
        /// <summary>
        /// 事件模块配置
        /// </summary>
        public EventModuleConfig eventModuleConfig;
        
        /// <summary>
        /// 对象池模块配置
        /// </summary>
        public PoolModuleConfig poolModuleConfig;

        /// <summary>
        /// 资源加载模块配置
        /// </summary>
        public ResourcesModuleConfig resourcesModuleConfig;
        
        /// <summary>
        /// 上传模块配置
        /// </summary>
        public UploadModuleConfig uploadModuleConfig;
        
        /// <summary>
        /// 更新模块配置
        /// </summary>
        public UpdateModuleConfig updateModuleConfig;

        /// <summary>
        /// 网络模块配置
        /// </summary>
        public NetModuleConfig netModuleConfig;
        
        /// <summary>
        /// 用户模块配置
        /// </summary>
        public UserModuleConfig userModuleConfig;
    }
}
