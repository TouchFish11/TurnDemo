using Core.DI;
using HotUpdate.Base.Component;
using HotUpdate.Base.Object;

namespace HotUpdate.Base.Utility
{
    public class EntityHelper
    {
        private static long s_idCounter;
        private static readonly ComponentService s_service;

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
