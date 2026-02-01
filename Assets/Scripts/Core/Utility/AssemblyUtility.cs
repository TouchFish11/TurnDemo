using System.Collections.Generic;
using System.Reflection;
using Core.HotUpdate;
using Core.Log;
using Core.Service;

namespace Core.Utility
{
    /// <summary>
    /// 程序集工具类
    /// </summary>
    public static class AssemblyUtility
    {
        /// <summary>
        /// 获取所有程序集
        /// </summary>
        /// <returns></returns>
        public static Assembly[] GetAssemblies()
        {
            var assemblies = new List<Assembly>
            {
                GetCoreAssembly(),
                GetConfigAssembly(),
                GetGameAssembly(),
            };
            
            // 获取所有热更后的程序集
            assemblies.AddRange(ServiceLocator.Get<IHotUpdateManager>().GetAssemblies()); 
            return assemblies.ToArray();
        }

        public static Assembly GetCoreAssembly()
        {
            return Assembly.Load("Assembly-CSharp-Core");
        }
        
        public static Assembly GetConfigAssembly()
        {
            return Assembly.Load("Assembly-CSharp-Config");
        }
        
        public static Assembly GetGameAssembly()
        {
            return Assembly.Load("Assembly-CSharp-Game");
        }
        
        /// <summary>
        /// 获取所有热更程序集
        /// </summary>
        /// <returns></returns>
        public static Assembly[] GetHotUpdateAssemblies()
        {
            var assemblys = new List<Assembly>(ServiceLocator.Get<IHotUpdateManager>().GetAssemblies());
            return assemblys.ToArray();
        }
    }
}
