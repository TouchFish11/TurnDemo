using System.Threading.Tasks;
using Core.AssetBundles.Update.Enum;

namespace Core.AssetBundles.Update.State
{
    /// <summary>
    /// 空状态类
    /// 作为更新流程的最终状态，无实际业务逻辑，仅标记流程结束
    /// </summary>
    public class NullState : UpdateState
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="updater">AssetBundle更新器实例</param>
        public NullState(AssetBundleUpdater updater) : base(updater)
        {

        }

        /// <summary>
        /// 执行空状态逻辑（无实际操作）
        /// </summary>
        /// <returns>固定返回true</returns>
        public override Task<bool> Execute()
        {
            IsSuceess = true;
            return Task.FromResult(IsSuceess);
        }

        /// <summary>
        /// 当前更新阶段标识
        /// </summary>
        public override EUpdatePhase UpdatePhase => EUpdatePhase.NullState;
    }
}