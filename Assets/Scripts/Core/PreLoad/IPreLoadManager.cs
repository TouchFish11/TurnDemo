using System.Threading.Tasks;

namespace Core.PreLoad
{
    public interface IPreLoadManager
    {
        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="preLoadDatas">预加载数据</param>
        Task PreLoads(params PreLoadData[] preLoadDatas);
    }
}
