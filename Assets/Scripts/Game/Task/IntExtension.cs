using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IntExtension
{
    /// <summary>
    /// 任务类型转换为字符串
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public static string TaskTypeToStr(this int i)
    {
        E_TaskType taskType = (E_TaskType)i;
        return taskType switch
        {
            E_TaskType.MainStory => "主线",
            E_TaskType.SideStroy => "支线",
            _ => "转换失败"
        };
    }

    /// <summary>
    /// 任务内容类型转换为枚举
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public static E_TaskContentType ToTaskContentType(this int i)
    {
        return (E_TaskContentType)i;
    }

}
