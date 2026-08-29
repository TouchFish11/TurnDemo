using Core.DI;
using HotUpdate.Base.ECModule;

namespace HotUpdate.Base.Utility
{
    /// <summary>
    /// 实体辅助器
    /// </summary>
    public class EntityHelper
    {
        private static long s_idCounter;
        
        private static ComponentService s_service { get; }

        static EntityHelper()
        {
            s_service = DIContainer.Create<ComponentService>();
        }
        
        /// <summary>
        /// 初始化实体，会调用实体的<see cref="IEntityObject.InitBase"/>方法
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
