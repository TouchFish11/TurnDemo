using System.IO;
using System.Threading.Tasks;
using Core.AssetBundles.Update.Enum;
using Core.Utility;

namespace Core.AssetBundles.Update.State
{
    /// <summary>
    /// 更新完成状态类
    /// 处理更新完成后的收尾逻辑，标记更新结束并切换到空状态
    /// </summary>
    public class FinishState : UpdateState
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="updater">AssetBundle更新器实例</param>
        public FinishState(AssetBundleUpdater updater) : base(updater)
        {

        }

        /// <summary>
        /// 执行更新完成收尾逻辑
        /// </summary>
        /// <returns>是否执行成功（固定返回true）</returns>
        public override async Task<UpdateResult> Execute()
        {
            await Task.Delay(1000);
            
            // 删除缓存文件
            if (File.Exists(PathUtility.GetAbLoadPath(FileUtility.CacheDefaultName)))
            {
                File.Delete(PathUtility.GetAbLoadPath(FileUtility.CacheDefaultName));
            }
            
            // 触发更新完成回调
            assetBundleUpdater.GetContext().UpdateFinish();
            return UpdateResult.CreateSuccess();
        }

        /// <summary>
        /// 当前更新阶段标识
        /// </summary>
        public override EUpdatePhase UpdatePhase => EUpdatePhase.Finished;
    }
}