using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillManager
{
    /// <summary>
    /// 添加技能命令到回合队列
    /// </summary>
    /// <param name="skill"></param>
    void AddSkillCommand(ISkill skill);

    /// <summary>
    /// 初始化技能目标
    /// 通过目标选择管理器初始化技能目标
    /// </summary>
    /// <param name="skill"></param>
    void InitSkillTarget(ISkill skill);
}
