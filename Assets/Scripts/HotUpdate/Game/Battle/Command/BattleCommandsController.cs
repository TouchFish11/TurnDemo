using System.Collections;
using System.Collections.Generic;
using Core.DI;
using HotUpdate.Base.Manager;
using HotUpdate.Base.UI;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Event.General;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.StateMeachine;

namespace HotUpdate.Game.Battle.Command
{
    /// <summary>
    /// 战斗指令控制器
    /// 负责管理战斗中所有指令的执行、排序、插入、过滤等核心逻辑
    /// </summary>
    public class BattleCommandsController
    {
        [Inject] private IUIService _uiService;
        [Inject] private IBattleCameraManager _battleCameraManager;
        [Inject] private IBattleManager _battleManager;
        
        // 战斗指令列表，存储待执行的战斗指令
        private readonly List<ICommand> _battleCommands = new();
        // 回合循环状态
        private readonly TurnLoopState _turnLoopState;
        // 战斗上下文
        private readonly IBattleContext _battleContext;
        // 是否退出战斗：标记战斗是否结束，用于终止指令执行循环
        private bool _isQuit;
        // 当前正在执行的指令
        private ICommand _currentCommand;

        public BattleCommandsController(TurnLoopState turnLoopState, IBattleContext context)
        {
            _turnLoopState = turnLoopState;
            _battleContext = context;
        }

        /// <summary>
        /// 执行战斗指令的核心协程
        /// 循环执行指令列表中的指令，直到列表为空或战斗结束
        /// </summary>
        /// <returns>协程迭代器</returns>
        public IEnumerator ExcuteCommand()
        {
            // 循环条件：有正在执行的指令 或 待执行列表有指令 且 未退出战斗
            while ((_currentCommand != null || _battleCommands.Count > 0) && !_isQuit)
            {
                // 获取列表首个指令作为当前执行命令
                TakeFirst();
                yield return ExecuteInternal();
                // 执行完指令后的处理逻辑
                yield return OnPostCommandExcute();
                // 判断是否退出战斗
                if (_isQuit) 
                    yield break;
                
                // 命令执行完后逻辑
                yield return _currentCommand.ExcutePostProcess(_battleContext);
                // 执行完成后清空当前命令
                _currentCommand = null;
            }
        }

        private IEnumerator ExecuteInternal()
        {
            // 压入执行者栈
            _battleContext.PushCommander(_currentCommand.Sender);
            // 执行当前指令
            yield return _currentCommand.Execute(_battleContext);
            // 弹出执行者栈
            _battleContext.PopCommander();
        }

        /// <summary>
        /// 指令执行后的后置处理逻辑
        /// 清理死亡怪物、检查战斗结束状态、过滤无效指令
        /// </summary>
        /// <returns></returns>
        private IEnumerator OnPostCommandExcute()
        {
            // 移除战斗中死亡的怪物
            yield return _turnLoopState.RemoveDeadMonster();
            // 检查当前波次是否结束，并更新退出标记
            _isQuit = _turnLoopState.CheckWaveOver();
            if (!_isQuit)
            {
                // 过滤列表中无效的指令
                FilterInvalidCommand();
                // 切换到下一波
                _turnLoopState.MoveWave();
            }
        }

        /// <summary>
        /// 过滤指令列表中的无效指令
        /// 反向遍历列表，移除IsValid为false的指令
        /// </summary>
        private void FilterInvalidCommand()
        {
            // 反向遍历：避免移除元素导致的索引错乱
            for (var i = _battleCommands.Count - 1; i >= 0; i--)
            {
                if (!_battleCommands[i].IsValid)
                {
                    _battleCommands.RemoveAt(i);
                }
            }
            // 更新UI：刷新等待指令的显示列表
            _battleContext.GetEventBus().TriggerEvent(new UpdateWaitCmdEvent(_battleContext, GetCommandSenders()));
        }

        /// <summary>
        /// 获取指令列表的首个指令
        /// 若列表有指令，将首个指令赋值给当前执行指令，并从列表移除
        /// </summary>
        public void TakeFirst()
        {
            if (_battleCommands.Count > 0)
            {
                _currentCommand = _battleCommands[0];
                RemoveFirst();
            }
        }

        /// <summary>
        /// 插入新的战斗指令到执行列表
        /// 1. 若当前无执行指令，直接赋值为当前指令
        /// 2. 若当前有执行指令，加入列表并按优先级排序
        /// </summary>
        /// <param name="command">待插入的战斗指令</param>
        public void InsertCommand(ICommand command)
        {
            if (_currentCommand == null)
            {
                // 当前无执行指令，直接执行新指令
                _currentCommand = command;
                return;
            }

            // 当前有执行指令，加入待执行列表
            _battleCommands.Add(command);
            // 按指令优先级重新排序列表
            SortCommand();
            // 更新UI：刷新等待指令的显示列表
            _battleContext.GetEventBus().TriggerEvent(new UpdateWaitCmdEvent(_battleContext, GetCommandSenders()));
        }

        /// <summary>
        /// 移除指令列表的首个指令
        /// 执行后更新UI等待指令显示
        /// </summary>
        public void RemoveFirst()
        {
            _battleCommands.RemoveAt(0);
            // 更新UI：刷新等待指令的显示列表
            _battleContext.GetEventBus().TriggerEvent(new UpdateWaitCmdEvent(_battleContext, GetCommandSenders()));
        }

        /// <summary>
        /// 对指令列表按优先级排序
        /// 优先级高（数值大）的指令排在列表前位
        /// </summary>
        private void SortCommand()
        {
            _battleCommands.Sort((c1, c2) =>
            {
                if (c1.Priority > c2.Priority)
                {
                    // c1优先级更高，排在前面（返回-1表示c1在c2前）
                    return -1;
                }

                if (c1.Priority < c2.Priority)
                {
                    // c2优先级更高，c1排在后面
                    return 1;
                }

                // 优先级相同，保持原有顺序
                return 0;
            });
        }

        /// <summary>
        /// 获取待执行指令列表中所有指令的发送者
        /// 用于UI层显示等待执行指令的实体列表
        /// </summary>
        /// <returns>指令发送者（战斗实体）列表</returns>
        public List<IBattleEntityObject> GetCommandSenders()
        {
            var battleEntities = new List<IBattleEntityObject>(_battleCommands.Count);
            foreach (var command in _battleCommands)
            {
                battleEntities.Add(command.Sender);
            }
            return battleEntities;
        }
    }
}