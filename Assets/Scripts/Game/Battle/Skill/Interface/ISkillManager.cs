using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillManager
{
    void AddSkillCommand(ISkill skill);
    void AddUltimateSkillCommand(ISkill skill);

    /// <summary>
    /// 初始化技能目标
    /// </summary>
    /// <param name="skill"></param>
    void InitSkillTarget(ISkill skill);
}
