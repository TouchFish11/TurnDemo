using System;
using System.Collections.Generic;
using Core.DataPersistence.Binary;
using Core.EventCenter;
using Core.Log;
using Core.Service;
using Core.Singleton;
using Core.Utility;
using Game.Manager;
using Game.Tasks;

namespace GameHotUpdate.Tasks
{
    /// <summary>
    /// 任务管理器（单例模式）
    /// 负责任务的接受、追踪、进度更新、取消，以及任务相关事件的监听与处理
    /// </summary>
    public class TaskManager : SingletonBase<TaskManager>, ITaskManager
    {
        // 当前追踪的任务基础信息（配置表数据）
        private TaskInfo currentTaskInfo;
        // 当前任务的完成条件信息（配置表数据）
        private TaskConditionInfo currentConditionInfo;
        // 当前任务的运行时数据（进度、完成状态、追踪状态等）
        private TaskData currentTaskData;

        /// <summary>
        /// 任务更新事件（任务信息/进度变化时触发）
        /// 回调参数：当前任务信息、当前任务运行时数据
        /// </summary>
        public event Action<TaskInfo, TaskData> OnUpdateTask;

        /// <summary>
        /// 任务取消事件（取消当前追踪任务时触发）
        /// </summary>
        public event Action OnCancelTask;

        /// <summary>
        /// 私有构造函数（单例模式，禁止外部实例化）
        /// </summary>
        private TaskManager()
        {

        }

        /// <summary>
        /// 检查当前任务状态（初始化/恢复任务追踪）
        /// 从游戏管理器中获取正在追踪的任务，加载对应配置并监听事件
        /// </summary>
        public void CheckTaskState()
        {
            // 若没有正在追踪的任务，直接返回
            if (!ServiceLocator.Get<IGameManager>().TaskDataCollection.IsTracking(out var taskData))
            {
                LogManager.Log($"{nameof(TaskManager)}.{nameof(CheckTaskState)}，任务数据：{taskData}");
                return;
            }
            
            // 从配置表加载当前任务的基础信息和完成条件信息
            currentTaskInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<TaskInfoContainer>(EConfigLoadType.Excel).dataDic[taskData.currentTaskId];
            currentConditionInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<TaskConditionInfoContainer>(EConfigLoadType.Excel).dataDic[currentTaskInfo.f_completionConditionId];
            currentTaskData = taskData;
            
            // 触发任务更新事件，通知外部任务状态变化
            OnUpdateTask?.Invoke(currentTaskInfo, currentTaskData);
            
            // 注册任务相关事件监听（根据任务类型）
            ListenTaskEvent();
        }

        /// <summary>
        /// 接受指定ID的任务（开始追踪该任务）
        /// 若已有正在追踪的任务，先取消原有任务
        /// </summary>
        /// <param name="id">要接受的任务ID</param>
        public void AcceptTask(string id)
        {
            // 若当前已有正在追踪的任务，先取消该任务的追踪
            if (currentTaskInfo != null && currentTaskData != null)
            {
                CancelTask();
            }

            // 从配置表加载新任务的基础信息和完成条件信息
            currentTaskInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<TaskInfoContainer>(EConfigLoadType.Excel).dataDic[id];
            currentConditionInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<TaskConditionInfoContainer>(EConfigLoadType.Excel).dataDic[currentTaskInfo.f_completionConditionId];

            // 检查任务数据集合中是否已存在该任务数据
            if (ServiceLocator.Get<IGameManager>().TaskDataCollection.TryGetValue(id, out var taskData))
            {
                // 若存在，标记为正在追踪，并赋值给当前任务数据
                taskData.isTracking = true;
                currentTaskData = taskData;
            }
            else
            {
                // 若不存在，创建新的任务运行时数据（初始进度0、未完成、开始追踪）
                var newTaskData = new TaskData() 
                { 
                    currentPro = 0, 
                    currentTaskId = id, 
                    isCompleted = false, 
                    isTracking = true 
                };
                // 将新任务数据添加到任务集合中
                ServiceLocator.Get<IGameManager>().TaskDataCollection.TryAdd(id, newTaskData);
                currentTaskData = newTaskData;
            }

            // 注册该任务的相关事件监听
            ListenTaskEvent();

            // 触发任务更新事件，通知外部任务已接受并开始追踪
            OnUpdateTask?.Invoke(currentTaskInfo, currentTaskData);
        }

        /// <summary>
        /// 注册任务相关事件监听（根据任务内容类型区分）
        /// 不同类型的任务监听对应不同的事件（对话/战斗/其他）
        /// </summary>
        private void ListenTaskEvent()
        {
            // 将配置表中的任务内容类型转换为枚举，并注册对应事件
            switch (currentConditionInfo.f_taskContentType.ToTaskContentType())
            {
                case E_TaskContentType.Dialogue:
                    // 对话类任务：注册对话事件监听
                    EventCenter.Instance.SubscribeEvent<DialogueEvent>(OnDialogueEvent);
                    break;
                case E_TaskContentType.Battle:
                    // 战斗类任务：注册战斗事件监听
                    EventCenter.Instance.SubscribeEvent<BattleEvent>(OnBattleEvent);
                    break;
                case E_TaskContentType.Other:
                    // 其他类型任务：暂不处理
                    break;
            }
        }

        /// <summary>
        /// 移除任务相关事件监听（取消追踪时调用）
        /// 避免事件重复监听或内存泄漏
        /// </summary>
        private void RemoveListenTaskEvent()
        {
            // 根据任务内容类型移除对应事件监听
            switch (currentConditionInfo.f_taskContentType.ToTaskContentType())
            {
                case E_TaskContentType.Dialogue:
                    // 移除对话事件监听
                    EventCenter.Instance.UnsubscribeEvent<DialogueEvent>(OnDialogueEvent);
                    break;
                case E_TaskContentType.Battle:
                    // 移除战斗事件监听
                    EventCenter.Instance.UnsubscribeEvent<BattleEvent>(OnBattleEvent);
                    break;
                case E_TaskContentType.Other:
                    // 其他类型任务：暂不处理
                    break;
            }
        }

        /// <summary>
        /// 对话事件回调（处理对话类任务的进度更新）
        /// </summary>
        /// <param name="dialogueEvent">对话事件参数（包含NPC ID等信息）</param>
        private void OnDialogueEvent(DialogueEvent dialogueEvent)
        {
            // 将配置表中的目标NPC ID拆分为整数列表
            var ids = new List<int>(TextUtility.SplitToIntArr(currentConditionInfo.f_npcId, 2));
            // 检查当前对话的NPC是否是任务目标NPC
            var index = ids.FindIndex(id => id == dialogueEvent.NpcId);
            if (index != -1)
            {
                // 若是目标NPC，更新任务进度
                UpdateTaskNodeProgress();
            }
        }
        
        /// <summary>
        /// 战斗事件回调（处理战斗类任务的进度更新）
        /// </summary>
        /// <param name="battleEvent">战斗事件参数（暂未使用）</param>
        private void OnBattleEvent(BattleEvent battleEvent)
        {
            // 触发任务进度更新（战斗触发即更新，可根据需求扩展条件）
            UpdateTaskNodeProgress();
        }

        /// <summary>
        /// 更新任务节点进度
        /// 进度满则标记任务完成，并处理后续任务（如有）
        /// </summary>
        private void UpdateTaskNodeProgress()
        {
            // 当前任务进度+1
            currentTaskData.currentPro += 1;
            
            // 检查进度是否达到任务要求的最大进度
            if (currentTaskData.currentPro == currentConditionInfo.f_maxPro)
            {
                // 标记任务为已完成
                currentTaskData.isCompleted = true;
                // 取消当前任务的追踪状态
                currentTaskData.isTracking = false;
                // 移除当前任务的事件监听
                RemoveListenTaskEvent();
                
                // 拆分当前任务的下一个任务ID（配置表数据）
                var ids = TextUtility.SplitToIntArr(currentTaskInfo.f_nextTaskId, 7);
                // 检查是否存在下一个任务（ID不等于-1表示有后续任务）
                if (ids[1] != -1)
                {
                    // 加载下一个任务的配置信息
                    currentTaskInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<TaskInfoContainer>(EConfigLoadType.Excel).dataDic[currentTaskInfo.f_nextTaskId];
                    currentConditionInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<TaskConditionInfoContainer>(EConfigLoadType.Excel).dataDic[currentTaskInfo.f_completionConditionId];
                    
                    // 创建下一个任务的运行时数据（初始进度0、未完成、开始追踪）
                    var nextTaskData = new TaskData
                    { 
                        currentTaskId = currentTaskInfo.f_id, 
                        currentPro = 0, 
                        isCompleted = false, 
                        isTracking = true 
                    };
                    // 将下一个任务数据添加到任务集合
                    ServiceLocator.Get<IGameManager>().TaskDataCollection.TryAdd(currentTaskInfo.f_id, nextTaskData);
                    // 更新当前任务数据为下一个任务
                    currentTaskData = nextTaskData;
                    
                    // 注册下一个任务的事件监听
                    ListenTaskEvent();
                }
            }
            
            // 触发任务更新事件，通知外部任务进度/状态变化
            OnUpdateTask?.Invoke(currentTaskInfo, currentTaskData);
        }

        /// <summary>
        /// 取消当前追踪的任务
        /// 移除事件监听、重置任务数据、触发取消事件
        /// </summary>
        public void CancelTask()
        {
            // 取消当前任务的追踪状态
            currentTaskData.isTracking = false;
            // 移除任务相关事件监听
            RemoveListenTaskEvent();
            // 重置当前任务的配置和运行时数据
            currentTaskInfo = null;
            currentTaskData = null;
            // 触发任务取消事件
            OnCancelTask?.Invoke();
        }
    }
}