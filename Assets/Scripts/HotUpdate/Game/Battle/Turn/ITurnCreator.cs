using System.Collections.Generic;
using System.Threading.Tasks;
using HotUpdate.Base;
using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Turn
{
    public interface ITurnCreator
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context"></param>
        /// <param name="totalTurnNum"></param>
        /// <param name="waves"></param>
        void Init(IBattleContext context, int totalTurnNum, List<List<int>> waves);

        /// <summary>
        /// 检查战斗是否结束
        /// true为结束
        /// </summary>
        /// <returns></returns>
        bool CheckBattleOver();

        /// <summary>
        /// 创建当前波次
        /// </summary>
        Task<List<IBattleEntityObject>> CreateWave();

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        Task<List<IBattleEntityObject>> CreateRoles(params int[] roleIds);
    }
}
