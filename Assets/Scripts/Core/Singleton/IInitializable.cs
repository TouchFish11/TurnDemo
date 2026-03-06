using System.Threading.Tasks;

namespace Core.Singleton
{
    public interface IInitializable
    {
        /// <summary>
        /// 初始化优先级
        /// 数值越小，越先初始化
        /// </summary>
        public int Priority { get; }
        
        /// <summary>
        /// 异步初始化
        /// </summary>
        /// <returns></returns>
        Task InitAsync();
    }
}
