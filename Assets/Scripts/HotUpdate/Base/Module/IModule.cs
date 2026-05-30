using System;
using System.Threading.Tasks;

namespace HotUpdate.Base.Module
{
    public interface IModule
    {
        /// <summary>
        /// 优先级，越大越优先
        /// </summary>
        [Obsolete]
        int Priority { get; }
        
        void Register();
        
        Task InitModuleAsync();
    }
}
