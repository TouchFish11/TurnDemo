using Core.DI;
using HotUpdate.Base.ECModule;

namespace HotUpdate.Base.Utility
{
    public class EntityHelper
    {
        private static long s_idCounter;
        
        private static ComponentService s_service { get; }

        static EntityHelper()
        {
            s_service = DIContainer.Create<ComponentService>();
        }
        
        /// <summary>
        /// 初始化实体
        /// </summary>
        /// <param name="entityObject"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static void InitEntity<T>(T entityObject) where T : IEntityObject
        {
            entityObject.InitBase(s_idCounter++, s_service);
        }
    }
}
