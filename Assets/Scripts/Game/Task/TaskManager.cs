using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using System;

/// <summary>
/// 任务管理器
/// </summary>
public class TaskManager : SingletonBase<TaskManager>
{
    // 当前任务信息
    private TaskInfo currentTaskInfo;
    // 当前任务完成条件信息
    private TaskConditionInfo currentConditionInfo;
    // 当前任务数据
    private TaskData currentTaskData;

    /// <summary>
    /// 更新任务事件
    /// </summary>
    public event Action<TaskInfo, TaskData> OnUpdateTask;

    /// <summary>
    /// 取消任务事件
    /// </summary>
    public event Action OnCancelTask;

    private TaskManager()
    {

    }

    /// <summary>
    /// 检查任务状态
    /// </summary>
    /// <returns></returns>
    public void CheckTaskState()
    {
        // 读取任务数据，是否有正在追踪的任务
        if (GameDataMgr.Instance.TaskDataCollection.IsTracking(out TaskData taskData))
        {
            currentTaskInfo = BinaryDataMgr.Instance.GetTable<TaskInfoContainer>().dataDic[taskData.currentTaskId];
            currentConditionInfo = BinaryDataMgr.Instance.GetTable<TaskConditionInfoContainer>().dataDic[currentTaskInfo.f_completionConditionId];
            currentTaskData = taskData;
            // 更新任务
            OnUpdateTask?.Invoke(currentTaskInfo, currentTaskData);
            // 监听事件
            ListenTaskEvent();
        }
    }

    /// <summary>
    /// 接收任务
    /// </summary>
    /// <param name="id"></param>
    public void AcceptTask(string id)
    {
        // 当前正在追踪其它任务
        if (currentTaskInfo != null && currentTaskData != null)
        {
            // 需要切换追踪，取消当前任务的追踪状态
            CancelTask();
        }

        currentTaskInfo = BinaryDataMgr.Instance.GetTable<TaskInfoContainer>().dataDic[id];
        currentConditionInfo = BinaryDataMgr.Instance.GetTable<TaskConditionInfoContainer>().dataDic[currentTaskInfo.f_completionConditionId];

        if (GameDataMgr.Instance.TaskDataCollection.TryGetValue(id, out TaskData taskData))
        {
            taskData.isTracking = true;
            currentTaskData = taskData;
        }
        else
        {
            TaskData newTaskData = new TaskData() { currentPro = default, currentTaskId = id, isCompleted = false, isTracking = true };
            GameDataMgr.Instance.TaskDataCollection.TryAdd(id, newTaskData);
            currentTaskData = newTaskData;
        }

        ListenTaskEvent();

        // 执行任务更新事件
        OnUpdateTask?.Invoke(currentTaskInfo, currentTaskData);
    }

    /// <summary>
    /// 监听任务事件
    /// </summary>
    private void ListenTaskEvent()
    {
        // 根据不同的任务内容监听不同的事件
        switch (currentConditionInfo.f_taskContentType.ToTaskContentType())
        {
            case E_TaskContentType.Dialogue:
                // 监听对话事件
                EventCenter.Instance.AddEventListener<DialogueEvent>(E_EventType.E_OnDialogue, OnDialogueEvent);
                break;
            case E_TaskContentType.Battle:
                // 监听战斗事件
                EventCenter.Instance.AddEventListener<BattleEvent>(E_EventType.E_OnDialogue, OnBattleEvent);
                break;
            case E_TaskContentType.Other:

                break;
        }
    }

    /// <summary>
    /// 取消监听任务事件
    /// </summary>
    private void RemoveListenTaskEvent()
    {
        // 根据不同的任务内容监听不同的事件
        switch (currentConditionInfo.f_taskContentType.ToTaskContentType())
        {
            case E_TaskContentType.Dialogue:
                // 移除监听对话事件
                EventCenter.Instance.RemoveEventListener<DialogueEvent>(E_EventType.E_OnDialogue, OnDialogueEvent);
                break;
            case E_TaskContentType.Battle:
                // 移除监听战斗事件
                EventCenter.Instance.RemoveEventListener<BattleEvent>(E_EventType.E_OnDialogue, OnBattleEvent);
                break;
            case E_TaskContentType.Other:

                break;
        }
    }

    private void OnDialogueEvent(DialogueEvent dialogueEvent)
    {
        bool has = false;
        int[] ids = TextUtility.SplitToIntArr(currentConditionInfo.f_npcId, 2);
        for (int i = 0; i < ids.Length; i++)
        {
            if (dialogueEvent.npcId == ids[i])
            {
                has = true;
                break;
            }
        }

        if (!has)
        {
            return;
        }

        UpdateTaskNodeProgress();
    }



    private void OnBattleEvent(BattleEvent battleEvent)
    {


        UpdateTaskNodeProgress();
    }

    /// <summary>
    /// 更新任务节点进度
    /// </summary>
    private void UpdateTaskNodeProgress()
    {
        // 当前节点任务进度增加
        currentTaskData.currentPro += 1;
        // 当前节点任务完成
        if (currentTaskData.currentPro == currentConditionInfo.f_maxPro)
        {
            // 更新为已完成
            currentTaskData.isCompleted = true;
            // 取消追踪状态
            currentTaskData.isTracking = false;
            RemoveListenTaskEvent();
            // 判断是否有下一节点
            int[] ids = TextUtility.SplitToIntArr(currentTaskInfo.f_nextTaskId, 7);
            // 切换为下一节点
            if (ids[1] != -1)
            {
                currentTaskInfo = BinaryDataMgr.Instance.GetTable<TaskInfoContainer>().dataDic[currentTaskInfo.f_nextTaskId];
                currentConditionInfo = BinaryDataMgr.Instance.GetTable<TaskConditionInfoContainer>().dataDic[currentTaskInfo.f_completionConditionId];
                TaskData nextTaskData = new TaskData() { currentTaskId = currentTaskInfo.f_id, currentPro = default, isCompleted = false, isTracking = true };
                // 缓存到任务集合数据
                GameDataMgr.Instance.TaskDataCollection.TryAdd(currentTaskInfo.f_id, nextTaskData);
                // 初始化为新任务节点信息
                currentTaskData = nextTaskData;
                ListenTaskEvent();
            }
        }
        // 执行任务更新事件
        OnUpdateTask?.Invoke(currentTaskInfo, currentTaskData);
    }

    /// <summary>
    /// 取消任务
    /// </summary>
    public void CancelTask()
    {
        // 取消追踪当前任务
        currentTaskData.isTracking = false;
        RemoveListenTaskEvent();
        currentTaskInfo = null;
        currentTaskData = null;
        OnCancelTask?.Invoke();
    }
}
