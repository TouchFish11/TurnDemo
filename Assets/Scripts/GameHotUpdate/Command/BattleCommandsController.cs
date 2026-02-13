using System.Collections;
using System.Collections.Generic;
using Core.Log;
using Core.Service;
using Core.UI;
using Core.Utility;
using Game.Battle.Command;
using Game.Battle.Objects;
using GameHotUpdate.Battle.Context;
using GameHotUpdate.Battle.Event.General;
using GameHotUpdate.Battle.StateMeachine;
using GameHotUpdate.Battle.UI.Base;
using GameHotUpdate.Battle.Utility;
using GameHotUpdate.Cameras;
using GameHotUpdate.Manager;
using UnityEngine;

namespace GameHotUpdate.Command
{
    /// <summary>
    /// 战斗指令控制器
    /// 负责管理战斗中所有指令的执行、排序、插入、过滤等核心逻辑
    /// </summary>
    public class BattleCommandsController
    {
        // 战斗指令队列：存储待执行的战斗指令
        private readonly List<ICommand> _battleCommands = new();
        // 回合循环状态
        private TurnLoopState _turnLoopState;
        // 是否退出战斗：标记战斗是否结束，用于终止指令执行循环
        private bool _isQuit;
        // 当前正在执行的指令
        private ICommand _command;

        public BattleCommandsController(TurnLoopState turnLoopState)
        {
            _turnLoopState = turnLoopState;
        }

        /// <summary>
        /// 执行战斗指令的核心协程
        /// 循环执行指令队列中的指令，直到队列为空或战斗结束
        /// </summary>
        /// <returns>协程迭代器</returns>
        public IEnumerator ExcuteCommand()
        {
            // 循环条件：有正在执行的指令 或 待执行队列有指令 且 未退出战斗
            while ((_command != null || _battleCommands.Count > 0) && !_isQuit)
            {
                // 获取队列首个指令作为当前执行命令
                GetFirst();
                // 执行当前命令（指令自身的执行逻辑）
                yield return _command.Execute(_turnLoopState.Context);
                // 执行完命令内容后的处理逻辑
                yield return OnPostCommandExcute();

                // 判断是否退出战斗
                if (_isQuit)
                {
                    yield break;
                }
                
                // 命令执行完后逻辑
                yield return _command.ExcutePostProcess(_turnLoopState.Context);
                // 执行完成后清空当前命令
                _command = null;
            }
        }

        /// <summary>
        /// 指令执行后的后置处理逻辑
        /// 包含：清理死亡怪物、检查战斗结束状态、过滤无效指令
        /// </summary>
        /// <returns>协程迭代器</returns>
        private IEnumerator OnPostCommandExcute()
        {
            // 移除战斗中死亡的怪物
            yield return _turnLoopState.RemoveDeadMonster();
            // 检查战斗是否结束，并更新退出标记
            _isQuit = _turnLoopState.CheckBattleOver();
            // 过滤队列中无效的指令（如执行者已死亡的指令）
            FilterInvalidCommand();
            if (!_isQuit)
            {
                // 当前波次是否结束，即判断当前怪物是否全部死亡
                if (_turnLoopState.Context.GetAliveMonsterEntityCount() != 0)
                {
                    yield break;
                }
                
                // 相机视角
                yield return TaskUtility.WaitForTask(ServiceLocator.Get<IBattleCameraManager>()
                    .CreateCamera(null, new Vector3(0, 1, -3.5f), Quaternion.identity));
            
                // 显示战斗开始协程
                // TODO：可拓展ShowBattleStart方法，显示当前是第几回合的文本
                var controller = ServiceLocator.Get<IUIManager>().GetController<BattleController>();
                controller.BattleUiManager.ShowBattleStart();
            
                // 创建入场特效
                // ...
            
                // 创建怪物并缓存
                List<IBattleEntityObject> monsters = null;
                yield return TaskUtility.WaitForTask(ServiceLocator.Get<IBattleManager>().GetTurnCreator().CreateWave(), list => monsters = list);
                foreach (var battleEntityObject in monsters)
                {
                    _turnLoopState.Context.AddBattleEntity(battleEntityObject);
                    _turnLoopState.Context.AddSceneMonster(battleEntityObject);
                }
            
                // 初始化行动顺序
                BattleUtility.InitOrder(_turnLoopState.Context);
                yield return new WaitForSeconds(1f);
            }
        }

        /// <summary>
        /// 过滤指令队列中的无效指令
        /// 反向遍历队列，移除IsValid为false的指令
        /// </summary>
        private void FilterInvalidCommand()
        {
            // 反向遍历：避免移除元素导致的索引错乱
            for (var i = _battleCommands.Count - 1; i >= 0; i--)
            {
                if (!_battleCommands[i].IsValid)
                {
                    LogManager.Log($"已过滤无效指令：{_battleCommands[i]}");
                    _battleCommands.RemoveAt(i);
                }
            }
            // 更新UI：刷新等待指令的显示列表
            _turnLoopState.Context.GetEventBus().TriggerEvent(new UpdateWaitCmdEvent(_turnLoopState.Context, GetCommandSenders()));
        }

        /// <summary>
        /// 获取指令队列的首个指令
        /// 若队列有指令，将首个指令赋值给当前执行指令，并从队列移除
        /// </summary>
        public void GetFirst()
        {
            if (_battleCommands.Count > 0)
            {
                _command = _battleCommands[0];
                RemoveFirst();
            }
        }

        /// <summary>
        /// 插入新的战斗指令到执行队列
        /// 1. 若当前无执行指令，直接赋值为当前指令
        /// 2. 若当前有执行指令，加入队列并按优先级排序
        /// </summary>
        /// <param name="command">待插入的战斗指令</param>
        public void InsertCommand(ICommand command)
        {
            if (_command == null)
            {
                // 当前无执行指令，直接执行新指令
                _command = command;
                return;
            }

            // 当前有执行指令，加入待执行队列
            _battleCommands.Add(command);
            // 按指令优先级重新排序队列
            SortCommand();
            // 更新UI：刷新等待指令的显示列表
            _turnLoopState.Context.GetEventBus().TriggerEvent(new UpdateWaitCmdEvent(_turnLoopState.Context, GetCommandSenders()));
        }

        /// <summary>
        /// 移除指令队列的首个指令
        /// 执行后更新UI等待指令显示
        /// </summary>
        public void RemoveFirst()
        {
            _battleCommands.RemoveAt(0);
            // 更新UI：刷新等待指令的显示列表
            _turnLoopState.Context.GetEventBus().TriggerEvent(new UpdateWaitCmdEvent(_turnLoopState.Context, GetCommandSenders()));
        }

        /// <summary>
        /// 对指令队列按优先级排序
        /// 优先级高（数值大）的指令排在队列前位
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
                else if (c1.Priority < c2.Priority)
                {
                    // c2优先级更高，c1排在后面
                    return 1;
                }
                else
                {
                    // 优先级相同，保持原有顺序
                    return 0;
                }
            });
        }

        /// <summary>
        /// 获取待执行指令队列中所有指令的发送者
        /// 用于UI层显示等待执行指令的实体列表
        /// </summary>
        /// <returns>指令发送者（战斗实体）列表</returns>
        public List<IBattleEntityObject> GetCommandSenders()
        {
            List<IBattleEntityObject> battleEntities = new List<IBattleEntityObject>(_battleCommands.Count);
            foreach (ICommand command in _battleCommands)
            {
                battleEntities.Add(command.Sender);
            }
            return battleEntities;
        }
    }
}