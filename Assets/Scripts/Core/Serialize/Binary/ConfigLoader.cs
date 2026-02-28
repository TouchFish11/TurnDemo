using System;
using System.Threading.Tasks;

namespace Core.Serialize.Binary
{
    /// <summary>
    /// 配置加载器
    /// </summary>
    public abstract class ConfigLoader : IConfigLoader
    {
        protected string assetbundleName;
        
        public event Func<IConfigLoader, Task> OnConfigLoaded;
        
        public async Task LoadConfig(string abName)
        {
            assetbundleName = abName;
            if (OnConfigLoaded != null)
            {
                await OnConfigLoaded.Invoke(this); 
                OnConfigLoaded = null;
            }
        }
        
        public abstract Task LoadConfigAsync<T, K>();
        
        public abstract T GetConfig<T>() where T : class;
    }
}
