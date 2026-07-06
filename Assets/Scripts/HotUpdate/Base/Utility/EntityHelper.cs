using Core.DI;
using HotUpdate.Base.Component;
using HotUpdate.Base.Object;

namespace HotUpdate.Base.Utility
{
    public class EntityHelper
    {
        private static long s_idCounter;
        
        public static ComponentService Service { get; }

        static EntityHelper()
        {
            Service = DIContainer.Create<ComponentService>();
        }
        
        /// <summary>
        /// 初始化实体
        /// </summary>
        /// <param name="entityObject"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static void InitEntity<T>(T entityObject) where T : IEntityObject
        {
            entityObject.InitBase(s_idCounter++, Service);
        }
    }
}
