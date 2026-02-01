using System.Collections;
using System.Threading.Tasks;
using Game.Battle.Command;
using Game.Battle.Objects;

namespace Game.Battle.Turn
{
    /// <summary>
    /// 回合控制器接口
    /// </summary>
    public interface ITurnController
    {
        /// <summary>
        /// 战斗是否结束
        /// </summary>
        bool IsBattleOver { get; }

        /// <summary>
        /// 战斗循环
        /// </summary>
        /// <returns></returns>
        IEnumerator StartBattle();

        /// <summary>
        /// 更新实体看向
        /// </summary>
        /// <param name="target"></param>
        void UpdateEntityLookAt(IBattleEntityObject target);

        /// <summary>
        /// 插入队列
        /// </summary>
        /// <param name="actEndEntity"></param>
        void InsertOrder(IBattleEntityObject actEndEntity);

        /// <summary>
        /// 检查战斗是否结束
        /// </summary>
        /// <returns></returns>
        bool CheckBattleOver();

        /// <summary>
        /// 移除死亡怪物实体
        /// </summary>
        IEnumerator RemoveDeadMonster();

        /// <summary>
        /// 插入命令
        /// </summary>
        /// <param name="skill"></param>
        void InsertCommand(ICommand command);

        /// <summary>
        /// 战斗准备
        /// </summary>
        Task BattlePreparation();
    }
}
