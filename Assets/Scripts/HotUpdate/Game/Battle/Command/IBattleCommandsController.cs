using System.Collections;
using System.Collections.Generic;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.UI;

namespace HotUpdate.Game.Battle.Command
{
    internal interface IBattleCommandsController
    {
        void Init(IBattleContext context, IBattleManager battleManager);

        /// <summary>
        /// 插入新的战斗指令到执行列表
        /// 1. 若当前无执行指令，直接赋值为当前指令
        /// 2. 若当前有执行指令，加入列表并按优先级排序
        /// </summary>
        /// <param name="command">待插入的战斗指令</param>
        void InsertCommand(ICommand command);

        /// <summary>
        /// 执行战斗指令的核心协程
        /// 循环执行指令列表中的指令，直到列表为空或战斗结束
        /// </summary>
        /// <returns>协程迭代器</returns>
        IEnumerator ExcuteCommand();

        /// <summary>
        /// 触发事件更新等待列表UI
        /// </summary>
        void UpdateWaitContent();

        /// <summary>
        /// 根据战斗状态构建等待列表
        /// </summary>
        /// <returns></returns>
        List<IDisplayPendingExecution> BuildPendingDisplayList();
    }
}
