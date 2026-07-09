using System.Collections;
using System.Collections.Generic;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Event.General;
using HotUpdate.Game.Battle.Event.Turn;
using HotUpdate.Game.Battle.UI;

namespace HotUpdate.Game.Battle.Command
{
    /// <summary>
    /// 战斗指令控制器
    /// 负责管理战斗中所有指令的执行、排序、插入、过滤等核心逻辑
    /// </summary>
    public class BattleCommandsController : IBattleCommandsController
    {
        // 战斗管理器
        private IBattleManager _battleManager;
        // 战斗上下文
        private IBattleContext _context;
        // 是否退出战斗：标记战斗是否结束，用于终止指令执行循环
        private bool _isQuit;

        private BattleCommandsController()
        {

        }

        public void Init(IBattleContext context, IBattleManager battleManager)
        {
            _context = context;
            _battleManager = battleManager;
        }
        
        /// <summary>
        /// 插入新的战斗指令到执行列表
        /// 1. 若当前无执行指令，直接赋值为当前指令
        /// 2. 若当前有执行指令，加入列表并按优先级排序
        /// </summary>
        /// <param name="command">待插入的战斗指令</param>
        public void InsertCommand(ICommand command)
        {
            if (_context.CurrentCommand == null)
            {
                // 当前无执行指令，直接执行新指令
                _context.CurrentCommand = command;
            }
            else
            {
                // 当前有执行指令，加入待执行列表
                _context.BattleCommands.Add(command);
                // 按指令优先级重新排序列表
                SortCommand();
            }

            // 更新等待列表UI
            UpdateWaitContent();
        }

        /// <summary>
        /// 执行战斗指令的核心协程
        /// 循环执行指令列表中的指令，直到列表为空或战斗结束
        /// </summary>
        /// <returns>协程迭代器</returns>
        public IEnumerator ExcuteCommand()
        {
            // 循环条件：有正在执行的指令 或 待执行列表有指令 且 未退出战斗
            while ((_context.CurrentCommand != null || _context.BattleCommands.Count > 0) && !_isQuit)
            {
                // 获取列表首个指令作为当前执行命令
                TakeFirst();
                yield return ExecuteInternal();
                // 执行完指令后的处理逻辑
                yield return ExcuteCommandEnd();
                // 判断是否退出战斗
                if (_isQuit) 
                    yield break;
                
                // 命令执行完后逻辑
                yield return _context.CurrentCommand.ExcutePostProcess(_context);
                // 执行完成后清空当前命令
                _context.CurrentCommand = null;
            }
        }

        private IEnumerator ExecuteInternal()
        {
            // 触发当前指令执行事件
            _context.EventBus.TriggerEvent(new CommandExecuteEvent(_context, _context.CurrentCommand));
            // 执行当前指令
            yield return _context.CurrentCommand.Execute(_context);
        }

        /// <summary>
        /// 指令执行后的后置处理逻辑
        /// 清理死亡怪物、检查战斗结束状态、过滤无效指令
        /// </summary>
        /// <returns></returns>
        private IEnumerator ExcuteCommandEnd()
        {
            var battleService  = _battleManager.BattleService;
            // 移除战斗中死亡的怪物
            yield return battleService.HandleDeadEntity();
            // 检查当前波次是否结束，并更新退出标记
            _isQuit = battleService.CheckWaveOver();
            if (_isQuit)
            {
                // 过滤列表中无效的指令
                FilterInvalidCommand();
                // 切换到下一波
                battleService.MoveWave();
            }
        }

        /// <summary>
        /// 过滤指令列表中的无效指令
        /// 反向遍历列表，移除IsValid为false的指令
        /// </summary>
        private void FilterInvalidCommand()
        {
            // 反向遍历：避免移除元素导致的索引错乱
            for (var i = _context.BattleCommands.Count - 1; i >= 0; i--)
            {
                var cmd =  _context.BattleCommands[i];
                if (!cmd.IsValid)
                {
                    _context.BattleCommands.RemoveAt(i);
                }
            }
            
            // 更新UI：刷新等待指令的显示列表
            UpdateWaitContent();
        }

        /// <summary>
        /// 获取指令列表的首个指令
        /// 若列表有指令，将首个指令赋值给当前执行指令，并从列表移除
        /// </summary>
        private void TakeFirst()
        {
            if (_context.BattleCommands.Count > 0)
            {
                var nextCommand = _context.BattleCommands[0];
                _context.BattleCommands.RemoveAt(0);
                _context.CurrentCommand = nextCommand;
                // 更新UI：刷新等待指令的显示列表
                UpdateWaitContent();
            }
        }

        /// <summary>
        /// 对指令列表按优先级排序
        /// 优先级高（数值大）的指令排在列表前位
        /// </summary>
        private void SortCommand()
        {
            _context.BattleCommands.Sort((c1, c2) =>
            {
                if (c1.Priority < c2.Priority)
                {
                    // c1优先级更高，排在前面（返回-1表示c1在c2前）
                    return -1;
                }

                if (c1.Priority > c2.Priority)
                {
                    // c2优先级更高，c1排在后面
                    return 1;
                }

                // 优先级相同，保持原有顺序
                return 0;
            });
        }

        /// <summary>
        /// 触发事件更新等待列表UI
        /// </summary>
        public void UpdateWaitContent()
        {
            var displayobjs = BuildPendingDisplayList();
            _context.EventBus.TriggerEvent(new UpdateWaitUiEvent(_context, displayobjs));
        }
        
        /// <summary>
        /// 根据战斗状态构建等待列表
        /// </summary>
        /// <returns></returns>
        public List<IDisplayPendingExecution> BuildPendingDisplayList()
        {
            // 转存显示待执行逻辑对象，包含排序后的所有等待指令
            var displayobjs = new List<IDisplayPendingExecution>(_context.BattleCommands.ConvertAll(cmd => (IDisplayPendingExecution)cmd));
            // 当前角色回合被其它逻辑插队的情况，CurrentCommander为null说明没有指令执行
            var currentCommander = _context.CurrentCommand?.Sender;
            if (currentCommander != _context.CurrentTurnOwner && _context.CurrentTurnOwner.CanAct)
            {
                // 显示持有当前回合被插队的角色的等待UI
                displayobjs.Add((IDisplayPendingExecution)_context.CurrentTurnOwner);
            }
            // 当前持有回合的角色正在执行自己的命令，然后有其它逻辑插队，这时不需要插入自己
            return displayobjs;
        }

        public void Reset()
        {
            _battleManager = null;
            _context = null;
            _isQuit = false;
        }
    }
}