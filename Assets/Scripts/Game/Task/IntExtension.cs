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

    /// <summary>
    /// 转换技能范围类型为文本
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public static string ToSkillRangeTypeText(this int i)
    {
        E_SkillRangeType skillRangeType = (E_SkillRangeType)i;
        return skillRangeType switch
        {
            E_SkillRangeType.Singel => "单体",
            E_SkillRangeType.Diffusion => "扩散",
            E_SkillRangeType.All => "全体",
            _ => "None"
        };
    }

    /// <summary>
    /// 转换为技能类型枚举
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public static E_SkillType ToSkillType(this int i)
    {
        return (E_SkillType)i;
    }
}
