using Framework;
using System.Collections;

namespace Game.Battle
{
    /// <summary>
    /// 战斗命令接口
    /// </summary>
    public interface ICommand : IPoolData
    {
        /// <summary>
        /// 命令优先级
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IEnumerator Excute(IBattleContext context);
    }
}
